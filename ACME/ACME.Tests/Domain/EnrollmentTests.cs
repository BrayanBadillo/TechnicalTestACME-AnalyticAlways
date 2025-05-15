using ACME.Domain.Entities;
using ACME.Domain.ValueObjects;

namespace ACME.Tests.Domain;

public class EnrollmentTests
{
    [Fact]
    public void Constructor_Should_Set_Properties()
    {
        var course = new Course("Test", new Money(0, "USD"), DateTime.Today, DateTime.Today.AddDays(1));
        var student = new Student("Ana", 20);
        var now = DateTime.Now;
        var paymentId = Guid.NewGuid();

        var enrollment = new Enrollment(course, student, now, paymentId);

        Assert.Equal(course, enrollment.Course);
        Assert.Equal(student, enrollment.Student);
        Assert.Equal(now, enrollment.EnrollmentDate);
        Assert.Equal(paymentId, enrollment.PaymentId);
        Assert.NotEqual(Guid.Empty, enrollment.Id);
    }
}