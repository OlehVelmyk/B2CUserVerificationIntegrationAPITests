using System.Collections.Generic;

namespace WX.B2C.User.Verification.Provider.Contracts
{
    public interface ICheckOutputDataExtractor
    {
        IReadOnlyDictionary<string, object> Extract(string checkDtoOutputData);
    }
}