﻿namespace ACME.Application.Interfaces;

public interface IUnitOfWork
{
    Task CommitAsync();
}