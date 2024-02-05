using System;
using System.Threading.Tasks;
using WX.Commands;

namespace WX.B2C.User.Verification.Facade.Commands.Middleware
{
    public interface ICommandHandlerDecorator
    {
        public Task HandleAsync<T>(T command, Func<T, Task> processor) where T:Command;
    }
}