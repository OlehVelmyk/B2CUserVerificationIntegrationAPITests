using WX.B2C.User.Verification.Provider.Contracts.Models;

namespace WX.B2C.User.Verification.Provider.Contracts
{
    public interface ICheckOutputDataSerializer
    {
        string Serialize(CheckOutputData outputData);

        TData Deserialize<TData>(string json) where TData: CheckOutputData;
    }
}
