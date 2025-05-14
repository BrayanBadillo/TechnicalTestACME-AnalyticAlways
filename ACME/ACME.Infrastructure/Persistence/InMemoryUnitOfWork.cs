using ACME.Application.Interfaces;

namespace ACME.Infrastructure.Persistence;

public class InMemoryUnitOfWork : IUnitOfWork
{
    public Task CommitAsync()
    {
        return Task.CompletedTask;
    }
}