using System.Threading.Tasks;

namespace WX.B2C.User.Verification.Domain.Shared
{
    public interface IEventPublisher
    {
        Task PublishAsync(params DomainEvent[] events);
    }
}
