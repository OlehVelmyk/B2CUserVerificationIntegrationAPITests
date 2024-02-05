// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace WX.B2C.User.Verification.Api.Admin.Client
{
    using Models;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Extension methods for CollectionStep.
    /// </summary>
    public static partial class CollectionStepExtensions
    {
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='userId'>
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<IList<CollectionStepDto>> GetAllAsync(this ICollectionStep operations, System.Guid userId, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.GetAllWithHttpMessagesAsync(userId, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='body'>
            /// </param>
            /// <param name='userId'>
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task RequestAsync(this ICollectionStep operations, CollectionStepRequest body, System.Guid userId, CancellationToken cancellationToken = default(CancellationToken))
            {
                (await operations.RequestWithHttpMessagesAsync(body, userId, null, cancellationToken).ConfigureAwait(false)).Dispose();
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='userId'>
            /// </param>
            /// <param name='stepId'>
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<CollectionStepDto> GetAsync(this ICollectionStep operations, System.Guid userId, System.Guid stepId, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.GetWithHttpMessagesAsync(userId, stepId, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='body'>
            /// </param>
            /// <param name='userId'>
            /// </param>
            /// <param name='stepId'>
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task DeleteAsync(this ICollectionStep operations, ReasonDto body, System.Guid userId, System.Guid stepId, CancellationToken cancellationToken = default(CancellationToken))
            {
                (await operations.DeleteWithHttpMessagesAsync(body, userId, stepId, null, cancellationToken).ConfigureAwait(false)).Dispose();
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='body'>
            /// </param>
            /// <param name='userId'>
            /// </param>
            /// <param name='stepId'>
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task UpdateAsync(this ICollectionStep operations, UpdateCollectionStepRequest body, System.Guid userId, System.Guid stepId, CancellationToken cancellationToken = default(CancellationToken))
            {
                (await operations.UpdateWithHttpMessagesAsync(body, userId, stepId, null, cancellationToken).ConfigureAwait(false)).Dispose();
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='body'>
            /// </param>
            /// <param name='userId'>
            /// </param>
            /// <param name='stepId'>
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task ReviewAsync(this ICollectionStep operations, ReviewCollectionStepRequest body, System.Guid userId, System.Guid stepId, CancellationToken cancellationToken = default(CancellationToken))
            {
                (await operations.ReviewWithHttpMessagesAsync(body, userId, stepId, null, cancellationToken).ConfigureAwait(false)).Dispose();
            }

    }
}
