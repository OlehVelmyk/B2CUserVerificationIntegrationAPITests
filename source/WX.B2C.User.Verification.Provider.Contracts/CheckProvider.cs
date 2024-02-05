using System;
using System.Threading.Tasks;
using Optional;
using Optional.Async.Extensions;
using WX.B2C.User.Verification.Extensions;
using WX.B2C.User.Verification.Provider.Contracts.Models;

namespace WX.B2C.User.Verification.Provider.Contracts
{
    public abstract class CheckProvider
    {
        public static CheckProvider<TData> Create<TData>(BaseCheckInputValidator<TData> validator,
                                                         CheckRunner<TData> checkRunner) =>
            new(validator, checkRunner);

        public abstract Task<Option<CheckRunningResult, Exception>> RunAsync(CheckInputData inputData);

        public abstract Task<Option<CheckProcessingResult, Exception>> GetResultAsync(CheckProcessingContext context);
    }

    public sealed class CheckProvider<TData> : CheckProvider
    {
        private readonly BaseCheckInputValidator<TData> _checkInputValidator;
        private readonly CheckRunner<TData> _checkRunner;

        public CheckProvider(
            BaseCheckInputValidator<TData> checkInputValidator,
            CheckRunner<TData> checkRunner)
        {
            _checkInputValidator = checkInputValidator ?? throw new ArgumentNullException(nameof(checkInputValidator));
            _checkRunner = checkRunner ?? throw new ArgumentNullException(nameof(checkRunner));
        }

        public override Task<Option<CheckRunningResult, Exception>> RunAsync(CheckInputData inputData)
        {
            if (inputData == null)
                throw new ArgumentNullException(nameof(inputData));

            return ValidateSafelyAsync(inputData)
                   .FlatMapAsync(RunSafelyAsync);

            Task<Option<TData, Exception>> ValidateSafelyAsync(CheckInputData data) =>
                HandleAsync(() => _checkInputValidator.Validate(data));

            Task<Option<CheckRunningResult, Exception>> RunSafelyAsync(TData data) =>
                HandleAsync(() => _checkRunner.RunAsync(data));
        }

        public override Task<Option<CheckProcessingResult, Exception>> GetResultAsync(CheckProcessingContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            return HandleAsync(() => _checkRunner.GetResultAsync(context));
        }

        private static Task<Option<TResult, Exception>> HandleAsync<TResult>(Func<TResult> action) =>
            HandleAsync(action?.ToAsync());

        private static async Task<Option<TResult, Exception>> HandleAsync<TResult>(Func<Task<TResult>> action)
        {
            try
            {
                var result = await action.Invoke();
                return result.Some<TResult, Exception>();
            }
            catch (Exception exc)
            {
                return Option.None<TResult, Exception>(exc);
            }
        }
    }
}