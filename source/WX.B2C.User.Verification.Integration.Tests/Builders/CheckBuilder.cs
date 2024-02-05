using System;
using System.Collections.Generic;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Integration.Tests.Models;

namespace WX.B2C.User.Verification.Integration.Tests.Builders
{
    internal class CheckBuilder
    {
        private CheckSpecimen _checkSpecimen;
        private readonly List<Guid> _relatedTasks = new();

        public CheckBuilder From(CheckSpecimen specimen)
        {
            _checkSpecimen = specimen;
            return this;
        }

        public CheckBuilder WithRelatedTasks(params Guid[] taskIds)
        {
            _relatedTasks.AddRange(taskIds);
            return this;
        }

        public CheckBuilder WithUserId(Guid userId)
        {
            _checkSpecimen.UserId = userId;
            return this;
        }

        public Check Build() =>
            new(_checkSpecimen.Id, 
                _checkSpecimen.UserId,
                _checkSpecimen.Type,
                _checkSpecimen.Variant, 
                _checkSpecimen.State,
                _checkSpecimen.ExecutionContext,
                _checkSpecimen.ProcessingResult,
                null,
                _checkSpecimen.StartedAt,
                _checkSpecimen.PerformedAt,
                _checkSpecimen.CompletedAt, 
                _relatedTasks.ToArray());
    }
}