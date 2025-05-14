using ACME.Application.Interfaces;
using ACME.Domain.Entities;
using ACME.Domain.Exceptions;
using ACME.Domain.ValueObjects;

namespace ACME.Application.Services;

public class CourseService(
    ICourseRepository courseRepository,
    IStudentRepository studentRepository,
    IPaymentGateway paymentGateway,
    IUnitOfWork unitOfWork) : ICourseService
{
    private readonly ICourseRepository _courseRepository =
        courseRepository ?? throw new ArgumentNullException(nameof(courseRepository));

    private readonly IStudentRepository _studentRepository =
        studentRepository ?? throw new ArgumentNullException(nameof(studentRepository));

    private readonly IPaymentGateway _paymentGateway =
        paymentGateway ?? throw new ArgumentNullException(nameof(paymentGateway));

    private readonly IUnitOfWork _unitOfWork =
        unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

    public async Task<Course> RegisterCourseAsync(string name, decimal registrationFee, string currency, DateTime startDate, DateTime endDate)
    {
        var course = new Course(name, new Money(registrationFee, currency), startDate, endDate);
        await _courseRepository.AddAsync(course);
        await _unitOfWork.CommitAsync();
        return course;
    }

    public async Task<IEnumerable<Course>> GetAllCoursesAsync()
    {
        return await _courseRepository.GetAllAsync();
    }

    public async Task<Course> GetCourseByIdAsync(Guid id)
    {
        return await _courseRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Course>> GetCoursesByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _courseRepository.GetCoursesByDateRangeAsync(startDate, endDate);
    }

    public async Task<Enrollment> EnrollStudentInCourseAsync(Guid studentId, Guid courseId, bool processPayment = true)
    {
        var course = await _courseRepository.GetByIdAsync(courseId);
        if (course == null)
        {
            throw new DomainException($"El curso con ID {courseId} no existe");
        }

        var student = await _studentRepository.GetByIdAsync(studentId);
        if (student == null)
        {
            throw new DomainException($"El estudiante con ID {studentId} no existe");
        }

        Guid paymentId = Guid.Empty;
        if (processPayment && course.RegistrationFee.Amount > 0)
        {
            // Procesar el pago a través de la pasarela de pago
            paymentId = await _paymentGateway.ProcessPaymentAsync(
                course.RegistrationFee,
                $"Inscripción al curso: {course.Name}",
                student.Name);
        }

        var enrollment = course.EnrollStudent(student, paymentId);
        await _unitOfWork.CommitAsync();

        return enrollment;
    }
}