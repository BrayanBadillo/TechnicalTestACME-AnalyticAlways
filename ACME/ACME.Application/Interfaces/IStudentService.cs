using ACME.Domain.Entities;

namespace ACME.Application.Interfaces;

public interface IStudentService
{
    Task<Student> RegisterStudentAsync(string name, int age);

    Task<IEnumerable<Student>> GetAllStudentsAsync();

    Task<Student> GetStudentByIdAsync(Guid id);
}