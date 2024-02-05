using FluentValidation;
using WX.B2C.User.Verification.Onfido.Client.Models;

namespace WX.B2C.User.Verification.Onfido.Processors.Validators
{
    internal abstract class BaseReportValidator<TReport> : AbstractValidator<TReport> where TReport: Report
    {
    }
}