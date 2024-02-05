// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace WX.B2C.User.Verification.Api.Admin.Client
{
    using Microsoft.Rest;
    using Models;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// AuditEntries operations.
    /// </summary>
    public partial interface IAuditEntries
    {
        /// <param name='userId'>
        /// </param>
        /// <param name='filter'>
        /// </param>
        /// <param name='orderby'>
        /// </param>
        /// <param name='top'>
        /// </param>
        /// <param name='skip'>
        /// </param>
        /// <param name='select'>
        /// </param>
        /// <param name='expand'>
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        /// <exception cref="ErrorResponseException">
        /// Thrown when the operation returned an invalid status code
        /// </exception>
        /// <exception cref="Microsoft.Rest.SerializationException">
        /// Thrown when unable to deserialize the response
        /// </exception>
        Task<HttpOperationResponse<AuditEntryDtoPageResult>> GetWithHttpMessagesAsync(System.Guid userId, string filter = default(string), string orderby = default(string), int? top = default(int?), int? skip = default(int?), string select = default(string), string expand = default(string), Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));
    }
}