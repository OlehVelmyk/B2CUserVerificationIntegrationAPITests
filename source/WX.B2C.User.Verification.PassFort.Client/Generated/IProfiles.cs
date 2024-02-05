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
    /// Profiles operations.
    /// </summary>
    public partial interface IProfiles
    {
        /// <summary>
        /// Create a profile
        /// </summary>
        /// <remarks>
        /// Create a new profile. You will need specify the _role_ of the
        /// customer, which will define what rules apply to that customer
        /// </remarks>
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
        Task<HttpOperationResponse<ProfileResource>> CreateWithHttpMessagesAsync(ProfileResource body = default(ProfileResource), Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));
        /// <summary>
        /// Get a profile
        /// </summary>
        /// <remarks>
        /// Returns detailed information a profile. This includes the collected
        /// data, details on the checks that have been run, and information on
        /// a profile's ongoing applications and tasks
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
        Task<HttpOperationResponse<ProfileResource>> GetWithHttpMessagesAsync(string profileId, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));
        /// <summary>
        /// Get collected data
        /// </summary>
        /// <remarks>
        /// Returns the data which has been collected from the user
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
        Task<HttpOperationResponse<IndividualData,ProfilesGetCollectedDataHeaders>> GetCollectedDataWithHttpMessagesAsync(string profileId, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));
        /// <summary>
        /// Update collected data
        /// </summary>
        /// <remarks>
        /// This allows you to update the collected data on the profile. This
        /// may be to either add more information collected from the customer,
        /// or to amend previously submitted information.
        ///
        /// PassFort does **not** merge data submitted. New data will
        /// **replace** the profile's existing `collected_data`.
        ///
        /// Note that updating data (e.g. amending a date of birth) may
        /// invalidate the results of your previously run checks. PassFort does
        /// not currently handle this for you, and you will need to explicitly
        /// detect and rerun these checks
        /// </remarks>
        /// <param name='profileId'>
        /// The identifier for the profile
        /// </param>
        /// <param name='body'>
        /// Request
        /// </param>
        /// <param name='ifMatch'>
        /// The current ETag of the collected data to be updated. If this value
        /// does not match the current ETag, a `412` status is returned.
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
        Task<HttpOperationResponse<IndividualData,ProfilesUpdateCollectedDataHeaders>> UpdateCollectedDataWithHttpMessagesAsync(string profileId, IndividualData body = default(IndividualData), string ifMatch = default(string), Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));
        /// <summary>
        /// Add a document
        /// </summary>
        /// <remarks>
        /// This is a convenience method to add a document to collected_data
        /// without having to `POST` the entirety of the collected_data
        /// </remarks>
        /// <param name='profileId'>
        /// The identifier for the profile
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
        Task<HttpOperationResponse<Document>> AddDocumentsWithHttpMessagesAsync(string profileId, DocumentPost body = default(DocumentPost), Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));
        /// <summary>
        /// Get a list of collection steps
        /// </summary>
        /// <remarks>
        /// Get a list of collection steps on a profile
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
        Task<HttpOperationResponse<IList<CollectionStep>>> GetCollectionStepsWithHttpMessagesAsync(string profileId, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));
        /// <summary>
        /// Update profile collection steps
        /// </summary>
        /// <remarks>
        /// Replace the collection steps on a profile
        /// </remarks>
        /// <param name='profileId'>
        /// The identifier for the profile
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
        Task<HttpOperationResponse<IList<CollectionStep>>> UpdateCollectionStepsWithHttpMessagesAsync(string profileId, IList<CollectionStep> body = default(IList<CollectionStep>), Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));
        /// <summary>
        /// Permanently delete a profile
        /// </summary>
        /// <remarks>
        /// Permantently delete a profile and all associated data. This action
        /// cannot be undone.
        /// </remarks>
        /// <param name='profileId'>
        /// The identifier for the profile.
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
        /// <exception cref="Microsoft.Rest.ValidationException">
        /// Thrown when a required parameter is null
        /// </exception>
        Task<HttpOperationResponse> DeleteWithHttpMessagesAsync(string profileId, ProfileDeletionRequest body = default(ProfileDeletionRequest), Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));
    }
}
