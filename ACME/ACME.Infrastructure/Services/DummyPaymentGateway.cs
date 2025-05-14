using ACME.Application.Interfaces;
using ACME.Domain.ValueObjects;

namespace ACME.Infrastructure.Services;

public class DummyPaymentGateway : IPaymentGateway
{
    public Task<Guid> ProcessPaymentAsync(Money amount, string description, string payerReference)
    {
        var paymentId = Guid.NewGuid();

        return Task.FromResult(paymentId);
    }
}