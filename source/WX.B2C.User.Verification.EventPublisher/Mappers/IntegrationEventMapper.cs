using System;
using System.Collections.Generic;
using System.Linq;
using WX.B2C.User.Verification.Domain;
using WX.B2C.User.Verification.Domain.DataCollection;
using WX.B2C.User.Verification.Domain.Events;
using WX.B2C.User.Verification.Domain.XPath;
using WX.B2C.User.Verification.Events.Dtos;
using WX.B2C.User.Verification.Events.Enums;
using WX.B2C.User.Verification.Events.EventArgs;
using WX.B2C.User.Verification.Events.Events;
using WX.B2C.User.Verification.Extensions;
using WX.B2C.User.Verification.Infrastructure.Contracts;
using WX.Messaging.Core;

namespace WX.B2C.User.Verification.EventPublisher.Mappers
{
    internal class IntegrationEventMapper : IIntegrationEventMapper
    {
        private readonly IOperationContextProvider _operationContextProvider;
        private readonly ICollectionStepEventMapper _collectionStepEventMapper;

        public IntegrationEventMapper(IOperationContextProvider operationContextProvider,
                                      ICollectionStepEventMapper collectionStepEventMapper)
        {
            _operationContextProvider = operationContextProvider ?? throw new ArgumentNullException(nameof(operationContextProvider));
            _collectionStepEventMapper = collectionStepEventMapper ?? throw new ArgumentNullException(nameof(collectionStepEventMapper));
        }

        private Guid CausationId => _operationContextProvider.GetContextOrDefault().OperationId;

        private Guid CorrelationId => _operationContextProvider.GetContextOrDefault().CorrelationId;

        public Event Map(ApplicationRegistered domainEvent)
        {
            if (domainEvent == null)
                throw new ArgumentNullException(nameof(domainEvent));

            var args = VerificationApplicationRegisteredEventArgs.Create(domainEvent.ApplicationId,
                                                                         domainEvent.UserId,
                                                                         domainEvent.PolicyId,
                                                                         (ProductType)domainEvent.ProductType,
                                                                         Map(domainEvent.Initiation));

            return new ApplicationRegisteredEvent(args.UserId.ToString(), args, CausationId, CorrelationId);
        }

        public Event Map(ApplicationStateChanged domainEvent)
        {
            if (domainEvent == null)
                throw new ArgumentNullException(nameof(domainEvent));

            var args = ApplicationStateChangedEventArgs.Create(domainEvent.UserId,
                                                               domainEvent.ApplicationId,
                                                               (ApplicationState)domainEvent.PreviousState,
                                                               (ApplicationState)domainEvent.NewState,
                                                               domainEvent.DecisionReasons,
                                                               Map(domainEvent.Initiation));

            return new ApplicationStateChangedEvent(args.UserId.ToString(), args, CausationId, CorrelationId);
        }

        public Event Map(CheckCompleted domainEvent)
        {
            if (domainEvent == null)
                throw new ArgumentNullException(nameof(domainEvent));

            var args = CheckCompletedEventArgs.Create(domainEvent.UserId,
                                                      domainEvent.Id,
                                                      domainEvent.VariantId,
                                                      domainEvent.Type.To<CheckType>(),
                                                      domainEvent.Result.To<CheckResult>());

            return new CheckCompletedEvent(args.CheckId.ToString(), args, CausationId, CorrelationId);
        }

        public Event Map(CollectionStepCompleted domainEvent)
        {
            if (domainEvent == null)
                throw new ArgumentNullException(nameof(domainEvent));

            var eventArgs = _collectionStepEventMapper.Map(domainEvent);
            if (eventArgs == null)
                return null;

            return new CollectionStepCompletedEvent(domainEvent.Id.ToString(),
                                                    eventArgs,
                                                    CausationId,
                                                    CorrelationId);
        }

        public Event Map(CollectionStepReadyForReview domainEvent)
        {
            if (domainEvent == null)
                throw new ArgumentNullException(nameof(domainEvent));

            var eventArgs = _collectionStepEventMapper.Map(domainEvent);
            if (eventArgs == null)
                return null;

            return new CollectionStepReadyForReviewEvent(domainEvent.Id.ToString(),
                                                         eventArgs,
                                                         CausationId,
                                                         CorrelationId);
        }

        public Event Map(CollectionStepRequested domainEvent)
        {
            if (domainEvent == null)
                throw new ArgumentNullException(nameof(domainEvent));

            var eventArgs = _collectionStepEventMapper.Map(domainEvent);
            if (eventArgs == null)
                return null;

            return new CollectionStepRequestedEvent(domainEvent.Id.ToString(),
                                                    eventArgs,
                                                    CausationId,
                                                    CorrelationId);
        }

        public Event Map(VerificationDetailsUpdated domainEvent)
        {
            var xPathToPropertyMapping = new Dictionary<string, string>
            {
                { XPathes.VerifiedNationality, nameof(VerificationDetailsDto.Nationality) },
                { XPathes.PoiIssuingCountry, nameof(VerificationDetailsDto.PoiIssuingCountry) },
                { XPathes.PlaceOfBirth, nameof(VerificationDetailsDto.PlaceOfBirth) },
                { XPathes.IsPep, nameof(VerificationDetailsDto.IsPep) },
                { XPathes.IsSanctioned, nameof(VerificationDetailsDto.IsSanctioned) },
                { XPathes.IsAdverseMedia, nameof(VerificationDetailsDto.IsAdverseMedia) },
                { XPathes.ComprehensiveIndex, nameof(VerificationDetailsDto.ComprehensiveIndex) },
                { XPathes.IsIpMatched, nameof(VerificationDetailsDto.IsIpMatched) },
                { XPathes.ResolvedCountryCode, nameof(VerificationDetailsDto.ResolvedCountryCode) },
                { XPathes.Tin, nameof(VerificationDetailsDto.Tin) },
            };

            var changes = domainEvent.Changes
                                     .Where(change => !change.IsReset)
                                     .Select(change => change.PropertyName)
                                     .Intersect(xPathToPropertyMapping.Keys)
                                     .Select(xPath => xPathToPropertyMapping[xPath])
                                     .ToArray();

            if (!changes.Any())
                return null;

            var args = new VerificationDetailsUpdatedEventArgs
            {
                UserId = domainEvent.UserId,
                Changes = changes
            };

            return new VerificationDetailsUpdatedEvent(domainEvent.UserId.ToString(), args, CausationId, CorrelationId);
        }

        private static InitiationDto Map(Initiation initiation)
        {
            if (initiation == null)
                throw new ArgumentNullException(nameof(initiation));

            return new InitiationDto
            {
                Initiator = initiation.Initiator,
                Reason = initiation.Reason,
            };
        }
    }
}