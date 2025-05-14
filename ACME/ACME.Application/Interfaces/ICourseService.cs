using ACME.Domain.Entities;

namespace ACME.Application.Interfaces;

public interface ICourseService
{
    Task<Course> RegisterCourseAsync(string name, decimal registrationFee, string currency, DateTime startDate, DateTime endDate);

    Task<IEnumerable<Course>> GetAllCoursesAsync();

    Task<Course> GetCourseByIdAsync(Guid id);

    Task<IEnumerable<Course>> GetCoursesByDateRangeAsync(DateTime startDate, DateTime endDate);

    Task<Enrollment> EnrollStudentInCourseAsync(Guid studentId, Guid courseId, bool processPayment = true);
}