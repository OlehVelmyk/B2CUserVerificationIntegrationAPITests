using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Events.Internal.Events;
using WX.B2C.User.Verification.Extensions;
using WX.Messaging.Subscriber.HandlerResolving;

namespace WX.B2C.User.Verification.Facade.EventHandlers
{
    internal class FileEventHandler : BaseEventHandler,
                                      IEventHandler<DocumentSubmittedEvent>
    {
        private readonly IFileService _fileService;

        public FileEventHandler(
            IFileService fileService,
            EventHandlingContext context) : base(context)
        {
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
        }

        public Task HandleAsync(DocumentSubmittedEvent @event) =>
            Handle(@event, args =>
            {
                return args.FilesIds.ForeachConsistently(SubmitAsync);

                Task SubmitAsync(Guid fileId) => _fileService.SubmitAsync(args.UserId, fileId);
            });
    }
}
