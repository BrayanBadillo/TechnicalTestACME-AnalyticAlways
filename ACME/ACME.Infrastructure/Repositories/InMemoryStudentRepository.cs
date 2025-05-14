using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACME.Application.Interfaces;
using ACME.Domain.Entities;

namespace ACME.Infrastructure.Repositories;

public class InMemoryStudentRepository : IStudentRepository
{
    private readonly List<Student> _students = new List<Student>();

    public Task<Student> GetByIdAsync(Guid id)
    {
        var student = _students.FirstOrDefault(s => s.Id == id);
        return Task.FromResult(student);
    }

    public Task<IEnumerable<Student>> GetAllAsync()
    {
        return Task.FromResult(_students.AsEnumerable());
    }

    public Task AddAsync(Student student)
    {
        if (student == null)
        {
            throw new ArgumentNullException(nameof(student));
        }

        _students.Add(student);
        return Task.CompletedTask;
    }
}