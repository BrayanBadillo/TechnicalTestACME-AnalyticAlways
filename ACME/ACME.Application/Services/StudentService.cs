using ACME.Application.Interfaces;
using ACME.Domain.Entities;

namespace ACME.Application.Services;

public class StudentService(IStudentRepository studentRepository, IUnitOfWork unitOfWork) : IStudentService
{
    private readonly IStudentRepository _studentRepository =
        studentRepository ?? throw new ArgumentNullException(nameof(studentRepository));

    private readonly IUnitOfWork _unitOfWork =
        unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

    public async Task<Student> RegisterStudentAsync(string name, int age)
    {
        var student = new Student(name, age);
        await _studentRepository.AddAsync(student);
        await _unitOfWork.CommitAsync();
        return student;
    }

    public async Task<IEnumerable<Student>> GetAllStudentsAsync()
    {
        return await _studentRepository.GetAllAsync();
    }

    public async Task<Student> GetStudentByIdAsync(Guid id)
    {
        return await _studentRepository.GetByIdAsync(id);
    }
}