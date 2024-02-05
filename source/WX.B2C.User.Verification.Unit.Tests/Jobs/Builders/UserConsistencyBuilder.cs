using System;
using WX.B2C.User.Verification.Worker.Jobs.Jobs.DetectDefects.Models;

namespace WX.B2C.User.Verification.Unit.Tests.Jobs.Builders
{
    internal interface IUserConsistencyBuilder
    {
        IUserConsistencyBuilder With(Guid userId);

        IProfileDataExistenceBuilder Exists();

        IApplicationBuilder WithApplication(Guid id);

        IUserConsistencyBuilder WithPassFortProfile(string id);

        IUserConsistencyBuilder WithOnfidoApplicant(string id);

        IUserConsistencyBuilder With(Region region);

        ITaskBuilder AddTask(Guid id);

        ICollectionStepBuilder AddStep(Guid id, string xPath);

        ICheckBuilder AddCheck();

        UserConsistency Build();
    }

    internal class UserConsistencyBuilder : IUserConsistencyBuilder
    {
        protected UserConsistency _user;

        public UserConsistencyBuilder(Guid userId)
        {
            _user = new UserConsistency
            {
                UserId = userId
            };
        }

        protected UserConsistencyBuilder(UserConsistency user)
        {
            _user = new UserConsistency
            {
                UserId = user.UserId,
                ProfileDataExistence = user.ProfileDataExistence,
                Application = user.Application,
                PassFortProfileId = user.PassFortProfileId,
                OnfidoApplicationId = user.OnfidoApplicationId,
                Region = user.Region,
                Tasks = user.Tasks,
                Checks = user.Checks,
                CollectionSteps = user.CollectionSteps
            };
        }

        public IUserConsistencyBuilder With(Guid userId) =>
            Update(user => user.UserId = userId);

        public IProfileDataExistenceBuilder Exists() =>
            new ProfileDataExistenceBuilder(_user);

        public IApplicationBuilder WithApplication(Guid id) =>
            new ApplicationBuilder(_user).With(id);

        public IUserConsistencyBuilder WithPassFortProfile(string id) =>
            Update(user => user.PassFortProfileId = id);

        public IUserConsistencyBuilder WithOnfidoApplicant(string id) =>
            Update(user => user.OnfidoApplicationId = id);

        public IUserConsistencyBuilder With(Region region) =>
            Update(user => user.Region = region);

        public ITaskBuilder AddTask(Guid id) =>
            new TaskBuilder(_user).With(id);

        public ICollectionStepBuilder AddStep(Guid id, string xPath) =>
            new CollectionStepBuilder(_user).With(id).With(xPath);

        public ICheckBuilder AddCheck() =>
            new CheckBuilder(_user);

        public UserConsistency Build() =>
            _user;

        private IUserConsistencyBuilder Update(Action<UserConsistency> update)
        {
            update(_user);
            return this;
        }
    }
}
