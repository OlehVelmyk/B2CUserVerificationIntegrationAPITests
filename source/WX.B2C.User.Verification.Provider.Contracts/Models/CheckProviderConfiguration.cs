using System.Collections.Generic;

namespace WX.B2C.User.Verification.Provider.Contracts.Models
{
    public abstract class CheckProviderConfiguration
    {
        public abstract IEnumerable<CheckInputParameter> CheckParameters { get; }
    }
}