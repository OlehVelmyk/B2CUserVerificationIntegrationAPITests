using System;
using System.Linq;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.PassFort.Mappers;

namespace WX.B2C.User.Verification.PassFort
{
    internal interface IPassFortApplicationMapper
    {
        PassFortApplication Map(Client.Models.ProductApplication application);
    }

    internal class PassFortApplicationMapper : IPassFortApplicationMapper
    {
        private readonly IPassFortEnumerationsMapper _enumerationsMapper;

        public PassFortApplicationMapper(IPassFortEnumerationsMapper enumerationsMapper)
        {
            _enumerationsMapper = enumerationsMapper ?? throw new ArgumentNullException(nameof(enumerationsMapper));
        }

        public PassFortApplication Map(Client.Models.ProductApplication application)
        {
            if (application is null)
                throw new ArgumentNullException(nameof(application));
            if (application.Product is null)
                throw new ArgumentNullException(nameof(application.Product));

            var tasks = application.RequiredTasks?
                                   .Select(task => _enumerationsMapper.Map(task.TaskType))
                                   .ToArray();

            return new PassFortApplication
            {
                Id = application.Id,
                State = _enumerationsMapper.Map(application.Status),
                IsApproveBlocked = application.ApprovalBlockers is { Count: > 0 },
                ProductId = application.Product.Id,
                RequiredTasks = tasks ?? Array.Empty<TaskType>()
            };
        }
    }
}