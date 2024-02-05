using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Commands;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Extensions;
using WX.Commands;

namespace WX.B2C.User.Verification.Facade.Commands
{
    internal class ExternalProfileCommandHandler : ICommandHandler<CreateExternalProfileCommand>
    {
        private readonly IExternalProfileProvider _externalProfileProvider;

        public ExternalProfileCommandHandler(IExternalProfileProvider externalProfileProvider)
        {
            _externalProfileProvider = externalProfileProvider ?? throw new ArgumentNullException(nameof(externalProfileProvider));
        }

        public Task HandleAsync(CreateExternalProfileCommand command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            return _externalProfileProvider.GetOrCreateAsync(command.UserId, command.Type.To<ExternalProviderType>());
        }
    }
}
