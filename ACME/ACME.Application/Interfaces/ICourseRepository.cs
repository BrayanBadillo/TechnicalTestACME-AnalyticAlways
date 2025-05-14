using ACME.Domain.Entities;

namespace ACME.Application.Interfaces;

public interface ICourseRepository
{
    Task<Course> GetByIdAsync(Guid id);

    Task<IEnumerable<Course>> GetAllAsync();

    Task<IEnumerable<Course>> GetCoursesByDateRangeAsync(DateTime startDate, DateTime endDate);

    Task AddAsync(Course course);
}