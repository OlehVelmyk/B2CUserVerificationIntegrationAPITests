using FluentValidation;
using WX.B2C.User.Verification.DataAccess.Seed.Models;

namespace WX.B2C.User.Verification.DataAccess.Seed.Validators
{
    internal class TaskValidator : BaseSeedValidator<Task>
    {
        public TaskValidator()
        {
            RuleFor(task => task.Id).NotEmpty();
            RuleFor(task => task.Name).NotEmpty();
            RuleFor(task => task.Type).IsInEnum();
            RuleFor(task => task.Priority).GreaterThan(0);
        }
    }
}