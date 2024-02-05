using System.Collections.Generic;
using System.Linq;
using System.Net;
using MbDotNet.Enums;
using MbDotNet.Models.Stubs;
using WX.B2C.User.Verification.Component.Tests.Models;
using WX.B2C.User.Verification.Component.Tests.Mountebank.Constants;
using WX.B2C.User.Verification.Component.Tests.Mountebank.Extensions;
using WX.B2C.User.Verification.Component.Tests.Mountebank.Factories;
using WX.B2C.User.Verification.Extensions;
using WX.B2C.User.Verification.Onfido.Client.Models;

namespace WX.B2C.User.Verification.Component.Tests.Mountebank.Providers
{
    internal static class OnfidoFileStubProvider
    {
        public static HttpStub CreateDocumentListStub(string applicantId, (string id, FileData content)[] documents)
        {
            const string pathTemplate = "/{apiVersion}/documents";
            var path = pathTemplate.Replace("{apiVersion}", ApiVersions.Onfido);

            var queryParameters = new Dictionary<string, object>() { { "applicant_id", applicantId } };

            var documentList = new DocumentList(new List<Document>());
            documents.Select(document => new Dictionary<string, object>() { { Parameters.DocumentId, document.id } })
                     .Select(parameters => SourceProvider.GetTemplate<Document>(Templates.OnfidoDocumentResponse, parameters))
                     .Foreach(document => documentList.Documents.Add(document));

            var predicate = PredicateFactory.CreateDeepEqualsPredicate(Method.Get, path, queryParameters: queryParameters);
            return new HttpStub().On(predicate).ReturnsJson(HttpStatusCode.OK, documentList);
        }

        public static IEnumerable<HttpStub> CreateDownloadDocumentsStub((string id, FileData content)[] documents)
        {
            foreach (var (id, content) in documents)
            {
                const string pathTemplate = "/{apiVersion}/documents/{documentId}/download";
                var path = pathTemplate.Replace("{apiVersion}", ApiVersions.Onfido)
                                       .Replace("{documentId}", id);

                var predicate = PredicateFactory.CreateDeepEqualsPredicate(Method.Get, path);
                yield return new HttpStub().On(predicate).ReturnsBinary(HttpStatusCode.OK, content.GetRaw(), content.ContentType);
            }
        }

        public static HttpStub CreateLivePhotoListStub(string applicantId, (string id, FileData content)[] livePhotos)
        {
            const string pathTemplate = "/{apiVersion}/live_photos";
            var path = pathTemplate.Replace("{apiVersion}", ApiVersions.Onfido);
            var queryParameters = new Dictionary<string, object>() { { "applicant_id", applicantId } };

            var livePhotoList = new LivePhotoList(new List<LivePhoto>());
            livePhotos.Select(CreateBodyParameters)
                      .Select(parameters => SourceProvider.GetTemplate<LivePhoto>(Templates.OnfidoLivePhotoResponse, parameters))
                      .Foreach(livePhoto => livePhotoList.LivePhotos.Add(livePhoto));

            var predicate = PredicateFactory.CreateDeepEqualsPredicate(Method.Get, path, queryParameters: queryParameters);
            return new HttpStub().On(predicate).ReturnsJson(HttpStatusCode.OK, livePhotoList);

            IDictionary<string, object> CreateBodyParameters((string id, FileData content) livePhoto) =>
                new Dictionary<string, object>()
                {
                    { Parameters.PhotoId, livePhoto.id },
                    { Parameters.ContentType, livePhoto.content.ContentType }
                };
        }

        public static IEnumerable<HttpStub> CreateDownloadLivePhotosStub((string id, FileData content)[] livePhotos)
        {
            foreach (var (id, content) in livePhotos)
            {
                const string pathTemplate = "/{apiVersion}/live_photos/{photoId}/download";
                var path = pathTemplate.Replace("{apiVersion}", ApiVersions.Onfido)
                                       .Replace("{photoId}", id);

                var predicate = PredicateFactory.CreateDeepEqualsPredicate(Method.Get, path);
                yield return new HttpStub().On(predicate).ReturnsBinary(HttpStatusCode.OK, content.GetRaw(), content.ContentType);
            }
        }

        public static HttpStub CreateLiveVideoListStub(string applicantId, (string id, FileData content)[] liveVideos)
        {
            const string pathTemplate = "/{apiVersion}/live_videos";
            var path = pathTemplate.Replace("{apiVersion}", ApiVersions.Onfido);
            var queryParameters = new Dictionary<string, object>() { { "applicant_id", applicantId } };

            var liveVideoList = new LiveVideoList(new List<LiveVideo>());
            liveVideos.Select(CreateBodyParameters)
                      .Select(parameters => SourceProvider.GetTemplate<LiveVideo>(Templates.OnfidoLiveVideoResponse, parameters))
                      .Foreach(livePhoto => liveVideoList.LiveVideos.Add(livePhoto));

            var predicate = PredicateFactory.CreateDeepEqualsPredicate(Method.Get, path, queryParameters: queryParameters);
            return new HttpStub().On(predicate).ReturnsJson(HttpStatusCode.OK, liveVideoList);

            IDictionary<string, object> CreateBodyParameters((string id, FileData content) liveVideo) =>
                new Dictionary<string, object>()
                {
                    { Parameters.VideoId, liveVideo.id },
                    { Parameters.ContentType, liveVideo.content.ContentType }
                };
        }

        public static IEnumerable<HttpStub> CreateDownloadLiveVideosStub((string id, FileData content)[] liveVideos)
        {
            foreach (var (id, content) in liveVideos)
            {
                const string pathTemplate = "/{apiVersion}/live_videos/{videoId}/download";
                var path = pathTemplate.Replace("{apiVersion}", ApiVersions.Onfido)
                                       .Replace("{videoId}", id);

                var predicate = PredicateFactory.CreateDeepEqualsPredicate(Method.Get, path);
                yield return new HttpStub().On(predicate).ReturnsBinary(HttpStatusCode.OK, content.GetRaw(), content.ContentType);
            }
        }

        public static HttpStub CreateUpladDocumentStub()
        {
            const string pathTemplate = "/{apiVersion}/documents";
            var path = pathTemplate.Replace("{apiVersion}", ApiVersions.Onfido);

            var document = SourceProvider.GetTemplate(Templates.OnfidoDocumentResponse);
            var decorateFunction = SourceProvider.GetDecorateFunction(DecorateFunctions.OnfidoUploadDocumentDecorator);
            var predicate = PredicateFactory.CreateDeepEqualsPredicate(Method.Post, path);
            return new HttpStub().On(predicate).ReturnsBody(HttpStatusCode.Created, document, decorateFunction);
        }

        public static HttpStub CreateUploadLivePhotoStub()
        {
            const string pathTemplate = "/{apiVersion}/live_photos";
            var path = pathTemplate.Replace("{apiVersion}", ApiVersions.Onfido);

            var document = SourceProvider.GetTemplate(Templates.OnfidoLivePhotoResponse);
            var decorateFunction = SourceProvider.GetDecorateFunction(DecorateFunctions.OnfidoUploadLivePhotoDecorator);
            var predicate = PredicateFactory.CreateDeepEqualsPredicate(Method.Post, path);
            return new HttpStub().On(predicate).ReturnsBody(HttpStatusCode.Created, document, decorateFunction);
        }
    }
}
