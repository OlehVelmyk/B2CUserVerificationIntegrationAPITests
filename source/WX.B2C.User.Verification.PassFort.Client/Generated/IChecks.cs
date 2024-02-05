// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace WX.B2C.User.Verification.PassFort.Client
{
    using Microsoft.Rest;
    using Models;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Checks operations.
    /// </summary>
    public partial interface IChecks
    {
        /// <summary>
        /// Get all checks run on a profile
        /// </summary>
        /// <remarks>
        /// Returns a list of checks associated with the profile
        /// </remarks>
        /// <param name='profileId'>
        /// The identifier for the profile
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        /// <exception cref="Microsoft.Rest.HttpOperationException">
        /// Thrown when the operation returned an invalid status code
        /// </exception>
        /// <exception cref="Microsoft.Rest.SerializationException">
        /// Thrown when unable to deserialize the response
        /// </exception>
        /// <exception cref="Microsoft.Rest.ValidationException">
        /// Thrown when a required parameter is null
        /// </exception>
        Task<HttpOperationResponse<IList<CheckResource>>> ListWithHttpMessagesAsync(string profileId, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));
        /// <summary>
        /// Run a check
        /// </summary>
        /// <remarks>
        /// Instruct a check on a profile
        /// </remarks>
        /// <param name='profileId'>
        /// The identifier for the profile
        /// </param>
        /// <param name='body'>
        /// Request
        /// </param>
        /// <param name='mode'>
        /// Whether the check should run async (respond immediately) or sync
        /// (wait for check completion). If unspecified, async for all checks
        /// except IDENTITY_CHECK. Possible values include: 'sync', 'async'
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        /// <exception cref="Microsoft.Rest.HttpOperationException">
        /// Thrown when the operation returned an invalid status code
        /// </exception>
        /// <exception cref="Microsoft.Rest.SerializationException">
        /// Thrown when unable to deserialize the response
        /// </exception>
        /// <exception cref="Microsoft.Rest.ValidationException">
        /// Thrown when a required parameter is null
        /// </exception>
        Task<HttpOperationResponse<CheckResource>> RunWithHttpMessagesAsync(string profileId, CheckRequest body = default(CheckRequest), string mode = default(string), Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));
        /// <summary>
        /// Get results of a specific check
        /// </summary>
        /// <remarks>
        /// Returns the current status and result of an instructed check
        /// </remarks>
        /// <param name='profileId'>
        /// The identifier for the profile
        /// </param>
        /// <param name='checkId'>
        /// The identifier for the specific check instance
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        /// <exception cref="Microsoft.Rest.HttpOperationException">
        /// Thrown when the operation returned an invalid status code
        /// </exception>
        /// <exception cref="Microsoft.Rest.SerializationException">
        /// Thrown when unable to deserialize the response
        /// </exception>
        /// <exception cref="Microsoft.Rest.ValidationException">
        /// Thrown when a required parameter is null
        /// </exception>
        Task<HttpOperationResponse<CheckResource>> GetWithHttpMessagesAsync(string profileId, string checkId, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));
        /// <summary>
        /// Update the tasks associated with a check
        /// </summary>
        /// <remarks>
        /// Checks are immutable and their results cannot be edited. This
        /// endpoint will update the `task_ids` on a check, all other fields
        /// are ignored
        /// </remarks>
        /// <param name='profileId'>
        /// The identifier for the profile
        /// </param>
        /// <param name='checkId'>
        /// The identifier for the specific check instance
        /// </param>
        /// <param name='body'>
        /// Request
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        /// <exception cref="Microsoft.Rest.HttpOperationException">
        /// Thrown when the operation returned an invalid status code
        /// </exception>
        /// <exception cref="Microsoft.Rest.SerializationException">
        /// Thrown when unable to deserialize the response
        /// </exception>
        /// <exception cref="Microsoft.Rest.ValidationException">
        /// Thrown when a required parameter is null
        /// </exception>
        Task<HttpOperationResponse<CheckResource>> UpdateAssociatedTasksWithHttpMessagesAsync(string profileId, string checkId, CheckResource body = default(CheckResource), Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));
        /// <summary>
        /// Manually approve a check
        /// </summary>
        /// <remarks>
        /// Generates a new check (checks are immutable and therefore not
        /// edited) from the specified check and sets the result to manually
        /// approved
        /// </remarks>
        /// <param name='profileId'>
        /// The identifier for the profile
        /// </param>
        /// <param name='checkId'>
        /// The identifier for the specific check instance
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        /// <exception cref="Microsoft.Rest.HttpOperationException">
        /// Thrown when the operation returned an invalid status code
        /// </exception>
        /// <exception cref="Microsoft.Rest.ValidationException">
        /// Thrown when a required parameter is null
        /// </exception>
        Task<HttpOperationResponse> ApproveWithHttpMessagesAsync(string profileId, string checkId, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));
        /// <summary>
        /// Manually reject a check
        /// </summary>
        /// <remarks>
        /// Generates a new check (checks are immutable and therefore not
        /// edited) from the specified check and sets the result to manually
        /// rejected
        /// </remarks>
        /// <param name='profileId'>
        /// The identifier for the profile
        /// </param>
        /// <param name='checkId'>
        /// The identifier for the specific check instance
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        /// <exception cref="Microsoft.Rest.HttpOperationException">
        /// Thrown when the operation returned an invalid status code
        /// </exception>
        /// <exception cref="Microsoft.Rest.ValidationException">
        /// Thrown when a required parameter is null
        /// </exception>
        Task<HttpOperationResponse> RejectWithHttpMessagesAsync(string profileId, string checkId, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));
    }
}