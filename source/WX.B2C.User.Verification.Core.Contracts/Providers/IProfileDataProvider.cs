using System.Collections.Generic;
using System.Threading.Tasks;

namespace WX.B2C.User.Verification.Core.Contracts
{
    public interface IProfileDataProvider
    {
        Task<IProfileDataCollection> ReadNotRequestedAsync(IEnumerable<string> xPathes);

        Task<IProfileDataCollection> ReadAsync(IEnumerable<string> xPathes);
    }

    public interface IProfileDataCollection : IReadOnlyDictionary<string, object>
    {
        bool HasValue(string xPath);

        object ValueOrNull(string xPath);
    }
}