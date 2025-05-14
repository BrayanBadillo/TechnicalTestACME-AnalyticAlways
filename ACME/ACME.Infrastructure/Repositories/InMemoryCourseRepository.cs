using ACME.Application.Interfaces;
using ACME.Domain.Entities;

namespace ACME.Infrastructure.Repositories;

public class InMemoryCourseRepository : ICourseRepository
{
    private readonly List<Course> _courses = new List<Course>();

    public Task<Course> GetByIdAsync(Guid id)
    {
        var course = _courses.FirstOrDefault(c => c.Id == id);
        return Task.FromResult(course);
    }

    public Task<IEnumerable<Course>> GetAllAsync()
    {
        return Task.FromResult(_courses.AsEnumerable());
    }

    public Task<IEnumerable<Course>> GetCoursesByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        var courses = _courses.Where(c =>
            (c.StartDate >= startDate && c.StartDate <= endDate) ||
            (c.EndDate >= startDate && c.EndDate <= endDate) ||
            (c.StartDate <= startDate && c.EndDate >= endDate));

        return Task.FromResult(courses);
    }

    public Task AddAsync(Course course)
    {
        if (course == null)
        {
            throw new ArgumentNullException(nameof(course));
        }

        _courses.Add(course);
        return Task.CompletedTask;
    }
}