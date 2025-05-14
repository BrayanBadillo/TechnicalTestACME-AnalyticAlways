using ACME.Domain.ValueObjects;

namespace ACME.Application.Interfaces;

public interface IPaymentGateway
{
    Task<Guid> ProcessPaymentAsync(Money amount, string description, string payerReference);
}