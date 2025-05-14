using ACME.Domain.Entities;

namespace ACME.Application.Interfaces;

public interface IStudentRepository
{
    Task<Student> GetByIdAsync(Guid id);

    Task<IEnumerable<Student>> GetAllAsync();

    Task AddAsync(Student student);
}