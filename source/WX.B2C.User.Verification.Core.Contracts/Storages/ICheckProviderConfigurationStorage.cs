using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.Core.Contracts.Storages
{
    public class CheckProviderConfigurationDto
    {
        public CheckType CheckType { get; set; }

        public CheckProviderType ProviderType { get; set; }

        public string Config { get; set; }
    }

    public interface ICheckProviderConfigurationStorage
    {
        Task<CheckProviderConfigurationDto> GetAsync(Guid configurationId);
    }
}
