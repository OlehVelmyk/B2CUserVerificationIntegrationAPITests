using System;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Worker.Jobs.Jobs.DetectDefects.Models;
using Application = WX.B2C.User.Verification.Worker.Jobs.Jobs.DetectDefects.Models.Application;

namespace WX.B2C.User.Verification.Unit.Tests.Jobs.Builders
{
    internal interface IApplicationBuilder : IUserConsistencyBuilder
    {
        IApplicationBuilder With(Guid id);

        IApplicationBuilder With(ApplicationState state);
    }

    internal class ApplicationBuilder : UserConsistencyBuilder, IApplicationBuilder
    {
        private readonly Application _application;

        public ApplicationBuilder(UserConsistency user)
            : base(user)
        {
            _application = _user.Application ??= new Application();
        }

        public IApplicationBuilder With(Guid id) =>
            Update(application => application.Id = id);

        public IApplicationBuilder With(ApplicationState state) =>
            Update(application => application.State = state);

        private IApplicationBuilder Update(Action<Application> update)
        {
            update(_application);
            return this;
        }
    }
}
