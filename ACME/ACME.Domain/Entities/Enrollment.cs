namespace ACME.Domain.Entities;

public class Enrollment
{
    public Guid Id { get; private set; }
    public Course Course { get; private set; }
    public Student Student { get; private set; }
    public DateTime EnrollmentDate { get; private set; }
    public Guid PaymentId { get; private set; }

    private Enrollment()
    { }

    public Enrollment(Course course, Student student, DateTime enrollmentDate, Guid paymentId)
    {
        Id = Guid.NewGuid();
        Course = course ?? throw new ArgumentNullException(nameof(course));
        Student = student ?? throw new ArgumentNullException(nameof(student));
        EnrollmentDate = enrollmentDate;
        PaymentId = paymentId;
    }
}