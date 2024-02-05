using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MbDotNet;
using MbDotNet.Models.Stubs;
using WX.B2C.User.Verification.Api.Admin.Client.Models;
using WX.B2C.User.Verification.Component.Tests.Models;
using WX.B2C.User.Verification.Component.Tests.Mountebank.Abstraction;
using WX.B2C.User.Verification.Component.Tests.Mountebank.Constants;
using WX.B2C.User.Verification.Component.Tests.Mountebank.Options;
using WX.B2C.User.Verification.Component.Tests.Mountebank.Providers;
using WX.B2C.User.Verification.Extensions;

namespace WX.B2C.User.Verification.Component.Tests.Mountebank
{
    /// <summary>
    /// TODO: 
    /// - Extract interfaces for services in Mountebank module 
    /// - Remove static keyword from classes 
    /// - Move module dependencies to IoC
    /// - Create module own IoC
    /// - Try to move url-pathes to constants, maybe using builder pattern
    /// TODO 2: Mock Applicants and Sdk-tokens resources
    /// </summary>
    internal interface IOnfidoImposter : IHttpImposter
    {
        Task ConfigureCheckAsync(OnfidoCheckOptions checkOptions);

        Task ConfigureCheckAsync(OnfidoGroupedCheckOptions checkOptions);

        Task ConfigureDownloadDocumentsAsync(string applicantId, (string id, FileData content)[] documents);

        Task ConfigureDownloadLivePhotosAsync(string applicantId, (string id, FileData content)[] livePhotos);

        Task ConfigureDownloadLiveVideosAsync(string applicantId, (string id, FileData content)[] liveVideos);
    }

    internal class OnfidoImposter : ProxyHttpImposter, IOnfidoImposter
    {
        private readonly string _webhookUrl; // TODO: Move to 'OnfidoCheckStubProvider'

        private readonly CheckType[] _groupedCheckParts = new[]
        {
            CheckType.IdentityDocument,
            CheckType.FaceDuplication,
            CheckType.FacialSimilarity
        };

        private readonly Dictionary<CheckType, string[]> _reports = new()
        {
            [CheckType.FacialSimilarity] = new[] { OnfidoReports.FacialSimilarityPhoto, OnfidoReports.FacialSimilarityVideo },
            [CheckType.FaceDuplication] = new[] { OnfidoReports.KnownFaces },
            [CheckType.IdentityDocument] = new[] { OnfidoReports.Document },
            [CheckType.IdentityEnhanced] = new[] { OnfidoReports.IdentityEnhanced }
        };

        public OnfidoImposter(MountebankClient client, string onfidoApiUrl, string proxyUrl, string webhookUrl)
            : base(client, onfidoApiUrl, proxyUrl)
        {
            _webhookUrl = webhookUrl ?? throw new ArgumentNullException(nameof(webhookUrl));
        }

        protected override IEnumerable<HttpStub> CreateCustomDefaultStubs()
        {
            var stubs = new List<HttpStub>();
            var bias = Guid.NewGuid().ToString();

            stubs.Add(OnfidoFileStubProvider.CreateUpladDocumentStub());
            stubs.Add(OnfidoFileStubProvider.CreateUploadLivePhotoStub());

            foreach (var (checkType, reports) in _reports)
            {
                var check = OnfidoCheckOption.Passed(checkType);
                stubs.AddRange(CreateCheckStubs(null, bias, check, reports));
            }

            var checks = _groupedCheckParts.Select(checkType => OnfidoCheckOption.Passed(checkType)).ToArray();
            var groupedChecks = GetGroupedChecks(checks);
            stubs.InsertRange(0, CreateGroupedCheckStubs(null, bias, groupedChecks));

            return stubs;
        }

        /// <summary>
        /// Configure run of a single check
        /// </summary>
        /// <remarks>
        /// Internally uses bias to configure check endpoints.
        /// Essentially bias is represented as OnfidoCheckOptions.ApplicationId 
        /// And it is used to create checkId that should correlate with bias.
        /// See cref="CreateCheckIdTemplate" or cref="CreateCheckIdPattern"
        /// </remarks>
        public Task ConfigureCheckAsync(OnfidoCheckOptions checkOptions)
        {
            if(checkOptions is null)
                throw new ArgumentNullException(nameof(checkOptions));
            if (checkOptions.ApplicantId is null)
                throw new ArgumentNullException(nameof(checkOptions.ApplicantId));

            var applicantId = checkOptions.ApplicantId;
            var reports = _reports[checkOptions.Check.Type];

            var stubs = CreateCheckStubs(applicantId, applicantId, checkOptions.Check, reports);
            return AppendStubsAsync(stubs);
        }

        /// <summary>
        /// Configure run of multiple checks in one Onfido check
        /// Onfido grouped check consists of IdentityDocument, FaceDuplication, FacialSimilarity at least
        /// If option (OnfidoCheckOption) is not presented for some check then it will be assumed as passed
        /// </summary>
        /// <remarks>
        /// Internally uses bias to configure check endpoints.
        /// Essentially bias is represented as OnfidoCheckOptions.ApplicationId 
        /// And it is used to create checkId that should correlate with bias.
        /// See cref="CreateCheckIdTemplate" or cref="CreateCheckIdPattern"
        /// </remarks>
        public Task ConfigureCheckAsync(OnfidoGroupedCheckOptions checkOptions)
        {
            if (checkOptions is null)
                throw new ArgumentNullException(nameof(checkOptions));
            if (checkOptions.ApplicantId is null)
                throw new ArgumentNullException(nameof(checkOptions.ApplicantId));

            var applicantId = checkOptions.ApplicantId;
            var checks = _groupedCheckParts.Except(checkOptions.Checks.Select(c => c.Type))
                                           .Select(type => OnfidoCheckOption.Passed(type))
                                           .Concat(checkOptions.Checks)
                                           .ToArray();

            var groupedChecks = GetGroupedChecks(checks);
            var stubs = CreateGroupedCheckStubs(applicantId, applicantId, groupedChecks);

            return AppendStubsAsync(stubs);
        }

        public Task ConfigureDownloadDocumentsAsync(string applicantId, (string id, FileData content)[] documents)
        {
            if (applicantId is null)
                throw new ArgumentNullException(nameof(applicantId));
            if (documents is null)
                throw new ArgumentNullException(nameof(documents));

            var stubs = new List<HttpStub>();
            stubs.Add(OnfidoFileStubProvider.CreateDocumentListStub(applicantId, documents));
            stubs.AddRange(OnfidoFileStubProvider.CreateDownloadDocumentsStub(documents));

            return AppendStubsAsync(stubs);
        }

        public Task ConfigureDownloadLivePhotosAsync(string applicantId, (string id, FileData content)[] livePhotos)
        {
            if (applicantId is null)
                throw new ArgumentNullException(nameof(applicantId));
            if (livePhotos is null)
                throw new ArgumentNullException(nameof(livePhotos));

            var stubs = new List<HttpStub>();
            stubs.Add(OnfidoFileStubProvider.CreateLivePhotoListStub(applicantId, livePhotos));
            stubs.AddRange(OnfidoFileStubProvider.CreateDownloadLivePhotosStub(livePhotos));

            return AppendStubsAsync(stubs);
        }

        public Task ConfigureDownloadLiveVideosAsync(string applicantId, (string id, FileData content)[] liveVideos)
        {
            if (applicantId is null)
                throw new ArgumentNullException(nameof(applicantId));
            if (liveVideos is null)
                throw new ArgumentNullException(nameof(liveVideos));

            var stubs = new List<HttpStub>();
            stubs.Add(OnfidoFileStubProvider.CreateLiveVideoListStub(applicantId, liveVideos));
            stubs.AddRange(OnfidoFileStubProvider.CreateDownloadLiveVideosStub(liveVideos));

            return AppendStubsAsync(stubs);
        }

        private IEnumerable<HttpStub> CreateCheckStubs(string applicantId, string bias, OnfidoCheckOption check, string[] reports)
        {
            var stubs = new List<HttpStub>();

            foreach (var report in reports)
            {
                stubs.Add(OnfidoCheckStubProvider.CreateCheckStub(applicantId, bias, _webhookUrl, (check, report)));
                stubs.Add(OnfidoCheckStubProvider.CreateGetCheckStub(applicantId, bias, (check, report)));
                stubs.Add(OnfidoCheckStubProvider.CreateGetReportsStub(applicantId, bias, (check, report)));
            }

            return stubs;
        }

        private IEnumerable<HttpStub> CreateGroupedCheckStubs(string applicantId, string bias, (OnfidoCheckOption check, string report)[][] groupedChecks)
        {
            var stubs = new List<HttpStub>();

            foreach (var check in groupedChecks)
            {
                stubs.Add(OnfidoCheckStubProvider.CreateCheckStub(applicantId, bias, _webhookUrl, check));
                stubs.Add(OnfidoCheckStubProvider.CreateGetCheckStub(applicantId, bias, check));
                if (check.Any(c => c.check.Type is CheckType.IdentityEnhanced))
                    stubs.Add(OnfidoCheckStubProvider.CreateGetReportsStub(applicantId, bias, check));
                else
                {
                    var option = (OnfidoCheckOption.Passed(CheckType.IdentityEnhanced), OnfidoReports.IdentityEnhanced);
                    var extendedCheck = check.Append(option).ToArray();
                    stubs.Add(OnfidoCheckStubProvider.CreateGetReportsStub(applicantId, bias, extendedCheck));
                }
            }

            return stubs;
        }

        /// <summary>
        /// Build possible combination of checks\reports
        /// Right now returns:
        /// [ "document", "known_faces", "facial_similarity_photo" ],
        /// [ "document", "known_faces", "facial_similarity_video" ]
        /// </summary>
        private (OnfidoCheckOption check, string report)[][] GetGroupedChecks(OnfidoCheckOption[] checks)
        {
            var groupedChecks = new List<List<(OnfidoCheckOption check, string report)>>();
            groupedChecks.Add(new());

            foreach (var check in checks)
            {
                var reports = _reports[check.Type];
                if (reports.Length > 1)
                {
                    var copy = groupedChecks.Select(reports => reports.ToList()).ToArray();
                    groupedChecks.AddRange(copy);
                }
                for (int i = 0; i < groupedChecks.Count; i++)
                    groupedChecks[i].Add((check, reports[i % reports.Length]));
            }

            return groupedChecks.Select(check => check.ToArray()).ToArray();
        }
    }
}
