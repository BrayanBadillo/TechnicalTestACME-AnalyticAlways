using ACME.Domain.Exceptions;

namespace ACME.Domain.Entities;

public class Student
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public int Age { get; private set; }

    private const int MinimumAge = 18;

    public Student()
    { }

    public Student(string name, int age)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty.", nameof(name));
        
        if (age < MinimumAge)
            throw new DomainException($"Only adults (over { MinimumAge } years of age) may register as students.");

        Id = Guid.NewGuid();
        Name = name;
        Age = age;
    }
}