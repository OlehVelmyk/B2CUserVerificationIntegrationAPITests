using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using WX.B2C.User.Profile.Events.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Configurations.Options;

namespace WX.B2C.User.Verification.Facade.EventHandlers.Validators
{
    public class AddressValidator : AbstractValidator<AddressDto>
    {
        private readonly IOptionProvider<SupportedStatesOption> _optionProvider;

        public AddressValidator(IOptionProvider<SupportedStatesOption> optionProvider)
        {
            _optionProvider = optionProvider ?? throw new ArgumentNullException(nameof(optionProvider));

            RuleFor(dto => dto.Country).NotEmpty().Length(2);
            RuleFor(dto => dto.City).NotEmpty();
            RuleFor(dto => dto.Line1).NotEmpty();
            RuleFor(dto => dto.ZipCode).NotEmpty();
            RuleFor(dto => dto.StateCode).NotEmpty().WhenAsync(IsStateRequiredAsync);
        }

        private async Task<bool> IsStateRequiredAsync(AddressDto address, CancellationToken cancellationToken)
        {
            if (address.Country is null)
                return false;

            var countryWithStates = (await _optionProvider.GetAsync()).CountrySupportedStates.Keys;
            return countryWithStates.Contains(address.Country);
        }
    }
}
