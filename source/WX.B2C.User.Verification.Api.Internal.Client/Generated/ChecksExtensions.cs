// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace WX.B2C.User.Verification.Api.Internal.Client
{
    using Models;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Extension methods for Checks.
    /// </summary>
    public static partial class ChecksExtensions
    {
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
            public static async Task RunFacialSimilarityCheckAsync(this IChecks operations, FacialSimilarityCheckRequest body, System.Guid userId, CancellationToken cancellationToken = default(CancellationToken))
            {
                (await operations.RunFacialSimilarityCheckWithHttpMessagesAsync(body, userId, null, cancellationToken).ConfigureAwait(false)).Dispose();
            }

    }
}