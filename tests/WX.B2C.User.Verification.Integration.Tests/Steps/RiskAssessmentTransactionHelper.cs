using WX.Backend.EventHub.Events;
using WX.Backend.EventHub.Events.Dtos;
using WX.Backend.EventHub.Events.EventArgs;

namespace WX.B2C.User.Verification.Integration.Tests.Steps;

internal class RiskAssessmentTransactionHelper
{
    public static TopUpTransactionEvent CreateTopUpTransactionEvent(string ownerId, decimal amount)
    {
        var topUpTransactionDto = new TopUpTransactionDto
        {
            UserId = ownerId,
            Amount = amount,
            CreatedAt = DateTime.UtcNow,
            Currency = "GBP",
            Ticker = "GBP/GBP",
            Status = Backend.EventHub.Events.Dtos.Enums.TransactionStatus.Completed,
            IsDebitCardConfirmed = true,
            Id = Guid.NewGuid(),
            AccountBalance = 0,
            AccountId = Guid.NewGuid(),
            ExternalAccountName = "test GBP account",
            DebitAccountId = Guid.NewGuid(),
            DebitCurrency = "GBP",
            DebitAmount = amount,
            ProviderFee = 0,
            WirexFee = 0,
            OperationDevice = Guid.NewGuid(),
            OperationIp = "127.0.0.1",
            OperationSession = Guid.NewGuid(),
            WirexDiscount = 0,
        };

        var eventArgs = new TopUpTransactionEventArgs
        {
            Transaction = topUpTransactionDto
        };

        return new TopUpTransactionEvent(ownerId, eventArgs, Guid.NewGuid(), Guid.NewGuid());
    }
}