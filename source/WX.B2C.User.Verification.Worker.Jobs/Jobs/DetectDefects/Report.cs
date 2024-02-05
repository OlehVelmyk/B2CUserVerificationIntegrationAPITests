using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation.Results;

namespace WX.B2C.User.Verification.Worker.Jobs.Jobs.DetectDefects
{
    internal class Report
    {
        private readonly Dictionary<string, object[]> _defects;

        private Report(Guid userId)
        {
            UserId = userId;
        }

        public Report(Guid userId, List<ValidationFailure> validationFailures)
        {
            if (validationFailures == null)
                throw new ArgumentNullException(nameof(validationFailures));
            if (validationFailures.Count == 0)
                throw new ArgumentException("Value cannot be an empty collection.", nameof(validationFailures));

            UserId = userId;
            _defects = validationFailures.GroupBy(failure => failure.ErrorCode)
                                         .ToDictionary(failures => failures.Key,
                                                       failures => failures.Where(failure => failure.CustomState is not null)
                                                                           .Select(failure => failure.CustomState)
                                                                           .ToArray());
        }

        public static Report NoDefects(Guid userId) =>
            new(userId);

        public Guid UserId { get; }

        public bool HasDefects => _defects != null;

        public Dictionary<string, object[]> GetDefects() =>
            _defects;
    }
}