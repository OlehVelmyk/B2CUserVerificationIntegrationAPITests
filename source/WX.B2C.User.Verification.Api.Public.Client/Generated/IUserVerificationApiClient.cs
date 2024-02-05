// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace WX.B2C.User.Verification.Api.Public.Client
{
    using Microsoft.Rest;
    using Models;
    using Newtonsoft.Json;

    /// <summary>
    /// </summary>
    public partial interface IUserVerificationApiClient : System.IDisposable
    {
        /// <summary>
        /// The base URI of the service.
        /// </summary>
        System.Uri BaseUri { get; set; }

        /// <summary>
        /// Gets or sets json serialization settings.
        /// </summary>
        JsonSerializerSettings SerializationSettings { get; }

        /// <summary>
        /// Gets or sets json deserialization settings.
        /// </summary>
        JsonSerializerSettings DeserializationSettings { get; }

        /// <summary>
        /// Unique identifier value that is attached to requests and messages
        /// that allow reference to a particular transaction or event chain
        /// </summary>
        System.Guid CorrelationId { get; set; }

        /// <summary>
        /// Request id/operation id to identify operation ordering in a chain
        /// </summary>
        System.Guid? OperationId { get; set; }

        /// <summary>
        /// Subscription credentials which uniquely identify client
        /// subscription.
        /// </summary>
        ServiceClientCredentials Credentials { get; }


        /// <summary>
        /// Gets the IActions.
        /// </summary>
        IActions Actions { get; }

        /// <summary>
        /// Gets the IApplications.
        /// </summary>
        IApplications Applications { get; }

        /// <summary>
        /// Gets the IDocuments.
        /// </summary>
        IDocuments Documents { get; }

        /// <summary>
        /// Gets the IProfile.
        /// </summary>
        IProfile Profile { get; }

        /// <summary>
        /// Gets the IProviders.
        /// </summary>
        IProviders Providers { get; }

        /// <summary>
        /// Gets the IValidation.
        /// </summary>
        IValidation Validation { get; }

    }
}