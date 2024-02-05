// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace WX.B2C.User.Verification.PassFort.Client
{
    using Models;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Extension methods for Documents.
    /// </summary>
    public static partial class DocumentsExtensions
    {
            /// <summary>
            /// Get list of document images
            /// </summary>
            /// <remarks>
            /// Get list of document images uploaded on a profile
            /// </remarks>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='profileId'>
            /// The identifier for the profile
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<IList<DocumentImageResourceMinimal>> ListAsync(this IDocuments operations, string profileId, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.ListWithHttpMessagesAsync(profileId, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <summary>
            /// Upload a new document image
            /// </summary>
            /// <remarks>
            /// This method allows you to submit a documnt image. It returns an `id` which
            /// can be used to reference the image in other data structures (typically as
            /// part of the document data structure)
            /// </remarks>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='profileId'>
            /// The identifier for the profile
            /// </param>
            /// <param name='body'>
            /// Request
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<DocumentImageResourceMinimal> UploadAsync(this IDocuments operations, string profileId, DocumentImageResource body = default(DocumentImageResource), CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.UploadWithHttpMessagesAsync(profileId, body, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <summary>
            /// Get a document image
            /// </summary>
            /// <remarks>
            /// Get information on a specific document image
            /// </remarks>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='profileId'>
            /// The identifier for the profile
            /// </param>
            /// <param name='documentImageId'>
            /// The identifier for the document image
            /// </param>
            /// <param name='full'>
            /// If false, will return only `id`, `image_type` and `upload_date`
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<DocumentImageResourceGet> GetAsync(this IDocuments operations, string profileId, string documentImageId, bool? full = default(bool?), CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.GetWithHttpMessagesAsync(profileId, documentImageId, full, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <summary>
            /// Upload a new file
            /// </summary>
            /// <remarks>
            /// This method allows you to submit a file
            /// </remarks>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='profileId'>
            /// The identifier for the profile
            /// </param>
            /// <param name='body'>
            /// Request
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<FileResource> UploadFileAsync(this IDocuments operations, string profileId, CreateFileResource body = default(CreateFileResource), CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.UploadFileWithHttpMessagesAsync(profileId, body, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <summary>
            /// Get a file by ID
            /// </summary>
            /// <remarks>
            /// Request a file by ID
            /// </remarks>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='profileId'>
            /// The identifier for the profile
            /// </param>
            /// <param name='fileId'>
            /// The identifier for the file
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<FileResource> DownloadFileAsync(this IDocuments operations, string profileId, string fileId, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.DownloadFileWithHttpMessagesAsync(profileId, fileId, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

    }
}