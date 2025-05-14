using ACME.Domain.Exceptions;
using ACME.Domain.ValueObjects;

namespace ACME.Domain.Entities;

public class Course
{
    private readonly List<Enrollment> _enrollments = new List<Enrollment>();

    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public Money RegistrationFee { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public IReadOnlyCollection<Enrollment> Enrollments => _enrollments.AsReadOnly();

    private Course()
    { }

    public Course(string name, Money registrationFee, DateTime startDate, DateTime endDate)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("The course name cannot be empty", nameof(name));

        if (startDate >= endDate)
            throw new DomainException("Start date must be before the end date");

        Id = Guid.NewGuid();
        Name = name;
        RegistrationFee = registrationFee ?? new Money(0, "EUR");
        StartDate = startDate;
        EndDate = endDate;
    }

    public Enrollment EnrollStudent(Student student, Guid paymentId)
    {
        if (student == null)
            throw new ArgumentNullException(nameof(student));

        if (_enrollments.Any(e => e.Student.Id == student.Id))
            throw new DomainException($"The student '{student.Name}' is already enrolled in this course.");

        if (RegistrationFee.Amount > 0 && paymentId == Guid.Empty)
            throw new DomainException($"Payment is required to enroll in this course (Fee: {RegistrationFee})");

        var enrollment = new Enrollment(this, student, DateTime.UtcNow, paymentId);
        _enrollments.Add(enrollment);

        return enrollment;
    }
}