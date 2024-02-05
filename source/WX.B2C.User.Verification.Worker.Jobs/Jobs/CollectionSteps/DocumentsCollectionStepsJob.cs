using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Quartz;
using Serilog;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.Domain.DataCollection;
using WX.B2C.User.Verification.Domain.Enums;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Domain.XPath;
using WX.B2C.User.Verification.Extensions;
using WX.B2C.User.Verification.Worker.Jobs.Models;
using WX.B2C.User.Verification.Worker.Jobs.Models.CollectionSteps;
using WX.B2C.User.Verification.Worker.Jobs.Models.Verification;
using WX.B2C.User.Verification.Worker.Jobs.Providers.CollectionSteps;
using WX.B2C.User.Verification.Worker.Jobs.Services;
using WX.B2C.User.Verification.Worker.Jobs.Settings;

namespace WX.B2C.User.Verification.Worker.Jobs.Jobs.CollectionSteps
{
    internal class DocumentsCollectionStepsJob : BatchJob<DocumentChecks, CollectionStepsJobSettings>
    {
        private static readonly string[] ProcessableXPathes =
        {
            XPathes.ProofOfIdentityDocument,
            XPathes.ProofOfAddressDocument,
            XPathes.ProofOfFundsDocument,
            XPathes.W9Form,
            XPathes.SelfieVideo,
            XPathes.SelfiePhoto,
        };

        private readonly IApplicationStorage _applicationStorage;
        private readonly IDocumentStorage _documentStorage;
        private readonly ICollectionStepCreatorFactory _creatorFactory;
        private ICollectionStepsCreator _creator;

        public DocumentsCollectionStepsJob(IDocumentsChecksProvider dataProvider,
                                           IApplicationStorage applicationStorage,
                                           IDocumentStorage documentStorage,
                                           ICollectionStepCreatorFactory creatorFactory,
                                           ILogger logger) : base(dataProvider, logger)
        {
            _applicationStorage = applicationStorage ?? throw new ArgumentNullException(nameof(applicationStorage));
            _documentStorage = documentStorage ?? throw new ArgumentNullException(nameof(documentStorage));
            _creatorFactory = creatorFactory ?? throw new ArgumentNullException(nameof(creatorFactory));
        }

        public static string Name => "documents-collection-step-backfill";

        public static JobBuilderFactory Builder =>
            _ => JobBuilder
                 .Create<DocumentsCollectionStepsJob>()
                 .WithIdentity(Name, "Backfill")
                 .WithDescription("Backfill collection steps according to documents checks and verification stop reason");

        protected override async Task Execute(Batch<DocumentChecks> batch, 
                                              CollectionStepsJobSettings settings, 
                                              CancellationToken cancellationToken)
        {
            var logger = Logger.ForContext("JobName", Name);
            _creator ??= _creatorFactory.Create(Name, (xPathes) => Filter(xPathes, settings.ExcludedXPathes), settings.UseActors);
            foreach (var data in batch.Items)
            {
                var userId = data.UserId;
                logger = logger.ForContext(nameof(data.UserId), userId);

                var application = await _applicationStorage.FindAsync(userId, ProductType.WirexBasic);
                if (application == null)
                {
                    logger.Warning("No application found for user. Creation collection steps skipped.");
                    continue;
                }
                    
                logger = logger.ForContext(nameof(application.PolicyId), application.PolicyId);
                
                var documents = await _documentStorage.FindSubmittedDocumentsAsync(userId);
                var expectedSteps = GetExpectedSteps(data, documents, logger);
                
                var tasks = application.Tasks.Select(dto => new CollectionStepsCreator.TaskInfo
                {
                    Id = dto.Id,
                    Type = dto.Type,
                    VariantId = dto.VariantId
                }).ToArray();

                try
                {
                    await _creator.CreateStepsAsync(userId, application.PolicyId, expectedSteps, tasks);
                }
                catch (Exception e)
                {
                    logger.Error(e, "Exception during creating collection steps");
                    IncrementErrorCount();
                }
            }
        }

        private IEnumerable<string> Filter(IEnumerable<string> xPathes, string[] excludedXPathes)
        {
            if (excludedXPathes.IsNullOrEmpty())
                return xPathes.Intersect(ProcessableXPathes);
            return xPathes.Intersect(ProcessableXPathes).Except(excludedXPathes);
        }

        private Dictionary<string, NewCollectionStep> GetExpectedSteps(DocumentChecks checksData,
                                                                       DocumentDto[] documents,
                                                                       ILogger logger)
        {
            var expectedStepsState = new Dictionary<string, NewCollectionStep>();
            
            IdentifyPoIState(documents, expectedStepsState);
            IdentifyPoAState(checksData, documents, expectedStepsState, logger);
            IdentifyPoFState(checksData, documents, expectedStepsState, logger);
            IdentifyW9FormState(checksData, documents, expectedStepsState, logger);
            IdentifySelfieState(expectedStepsState);

            return expectedStepsState;
        }

        private void IdentifyPoIState(DocumentDto[] documents, Dictionary<string, NewCollectionStep> expectedStepsState)
        {
            expectedStepsState[XPathes.ProofOfIdentityDocument] =
                documents.Any(dto => dto.Category == DocumentCategory.ProofOfIdentity)
                    ? NewCollectionStep.Completed()
                    : NewCollectionStep.Incompleted(CollectionStepState.Requested);
        }

        private static void IdentifyPoAState(DocumentChecks data,
                                             DocumentDto[] documents,
                                             Dictionary<string, NewCollectionStep> expectedStepsState,
                                             ILogger logger)
        {
            var document = documents.FirstOrDefault(dto => dto.Category == DocumentCategory.ProofOfAddress);
            var xPath = XPathes.ProofOfAddressDocument;
            var checkType = "ProofOfAddress";

            switch (data.PoAStatus)
            {
                case ProofOfAddressCheckStatus.Required:
                    var isOptional = data.VerificationStatus == VerificationStatus.Verified; 
                    if (document != null)
                    {
                        logger.Warning("{CheckType} check is required, but document is already submitted", checkType);
                        expectedStepsState[xPath] = NewCollectionStep.Incompleted(CollectionStepState.InReview, isOptional);
                        break;
                    }
                    expectedStepsState[xPath] = NewCollectionStep.Incompleted(CollectionStepState.Requested, isOptional);
                    if (isOptional)
                        logger.Warning("{CheckType} check is required in old verification, but user is verified. "
                                     + "Collection step created in optional state", checkType);
                    break;
                case ProofOfAddressCheckStatus.Requested:
                    if (document != null)
                    {
                        logger.Warning("{CheckType} check is requested, but document is already submitted", checkType);
                        expectedStepsState[xPath] = NewCollectionStep.Incompleted(CollectionStepState.InReview, false);
                        break;
                    }
                    expectedStepsState[xPath] = NewCollectionStep.Incompleted(CollectionStepState.Requested, false);
                    break;
                case ProofOfAddressCheckStatus.Completed:
                    if (document == null)
                        logger.Warning("{CheckType} check is completed, but document is not submitted yet", checkType);
                    expectedStepsState[xPath] = NewCollectionStep.ReviewCompleted(CollectionStepReviewResult.Approved);
                    break;
                case ProofOfAddressCheckStatus.Failed:
                    if (document == null)
                        logger.Warning("{CheckType} check is failed, but document is not submitted yet", checkType);
                    expectedStepsState[xPath] = NewCollectionStep.ReviewCompleted(CollectionStepReviewResult.Rejected);
                    break;
                case ProofOfAddressCheckStatus.Canceled:
                case null:
                    logger.Warning("Unexpected check status {CheckStatus}. Check type {CheckType}", data.PoAStatus, checkType);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void IdentifyPoFState(DocumentChecks data,
                                             DocumentDto[] documents,
                                             Dictionary<string, NewCollectionStep> expectedStepsState,
                                             ILogger logger)
        {
            var document = documents.FirstOrDefault(dto => dto.Category == DocumentCategory.ProofOfFunds);
            var xPath = XPathes.ProofOfFundsDocument;
            var checkType = "ProofOfFunds";
            
            switch (data.PoFStatus)
            {
                case ProofOfFundsCheckStatus.Required:
                    var isOptional = data.VerificationStatus == VerificationStatus.Verified;
                    if (document != null)
                    {
                        logger.Warning("{CheckType} check is required, but document is already submitted", checkType);
                        expectedStepsState[xPath] = NewCollectionStep.Incompleted(CollectionStepState.InReview, isOptional);
                        break;
                    }
                    expectedStepsState[xPath] = NewCollectionStep.Incompleted(CollectionStepState.Requested, isOptional);
                    if (isOptional)
                        logger.Warning("{CheckType} check is required in old verification, but user is verified. "
                                     + "Collection step created in optional state", checkType);
                    break;
                case ProofOfFundsCheckStatus.Requested:
                    if (document != null)
                    {
                        logger.Warning("{CheckType} check is requested, but document is already submitted", checkType);
                        expectedStepsState[xPath] = NewCollectionStep.Incompleted(CollectionStepState.InReview, false);
                        break;
                    }
                    expectedStepsState[xPath] = NewCollectionStep.Incompleted(CollectionStepState.Requested, false);
                    break;
                case ProofOfFundsCheckStatus.Completed:
                    if (document == null)
                        logger.Warning("{CheckType} check is completed, but document is not submitted yet", checkType);
                    expectedStepsState[xPath] = NewCollectionStep.ReviewCompleted(CollectionStepReviewResult.Approved);
                    break;
                case ProofOfFundsCheckStatus.Failed:
                    if (document == null)
                        logger.Warning("{CheckType} check is failed, but document is not submitted yet", checkType);
                    expectedStepsState[xPath] = NewCollectionStep.ReviewCompleted(CollectionStepReviewResult.Rejected);
                    break;
                case ProofOfFundsCheckStatus.Canceled:
                case null:
                    logger.Warning("Unexpected check status {CheckStatus}. Check type {CheckType}", data.PoFStatus, checkType);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void IdentifyW9FormState(DocumentChecks data,
                                         DocumentDto[] documents,
                                         Dictionary<string, NewCollectionStep> expectedStepsState,
                                         ILogger logger)
        {
            var document = documents.FirstOrDefault(dto => dto.Type == TaxationDocumentType.W9Form.Value);
            var xPath = XPathes.W9Form;
            var checkType = "W9Form";
            var stopReason = data.VerificationStopReason;

            if (stopReason.HasFlag(VerificationStopReason.UsaCitizenFormReviewed))
            {
                if (document == null)
                    logger.Warning("{CheckType} check is reviewed, but document is not submitted yet", checkType);
                expectedStepsState[xPath] = NewCollectionStep.ReviewCompleted(CollectionStepReviewResult.Approved);
            }
            else if (stopReason.HasFlag(VerificationStopReason.UsaCitizenFormDone))
            {
                if (document == null)
                    logger.Warning("{CheckType} check is submitted, but document is not submitted yet", checkType);
                expectedStepsState[xPath] = NewCollectionStep.Incompleted(CollectionStepState.InReview);
            }
            else if (stopReason.HasFlag(VerificationStopReason.UsaCitizenFormSubmission))
            {
                if (document != null)
                    logger.Warning("{CheckType} check is not submitted, but document is already submitted", checkType);
                expectedStepsState[xPath] = NewCollectionStep.Incompleted(CollectionStepState.Requested);
            }
        }
        
        private void IdentifySelfieState(Dictionary<string, NewCollectionStep> expectedStepsState)
        {
            // As we migrate users only in pending state and more it's already indicates that user submit selfie
            // CollectionStepCreator will not create step which is not needed according to policy,
            // Therefore we just can mark that both collection step completed.
            expectedStepsState[XPathes.SelfieVideo] = NewCollectionStep.Completed();
            expectedStepsState[XPathes.SelfiePhoto] = NewCollectionStep.Completed();
        }
    }
}