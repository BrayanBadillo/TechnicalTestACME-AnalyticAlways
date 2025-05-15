using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACME.Domain.Entities;
using ACME.Domain.Exceptions;
using ACME.Domain.ValueObjects;

namespace ACME.Tests.Domain;

public class CourseTests
{
    [Fact]
    public void Course_Should_Have_Valid_Properties()
    {
        // Arrange
        var course = new Course("Programación en C#", new Money(100, "EUR"), DateTime.Today.AddDays(10), DateTime.Today.AddDays(40));

        // Act & Assert
        Assert.Equal("Programación en C#", course.Name);

    }

    [Fact]
    public void EnrollStudent_WithNullStudent_ShouldThrowException()
    {
        // Arrange
        var course = new Course("Programación en C#", new Money(100, "USD"), DateTime.Today.AddDays(10), DateTime.Today.AddDays(40));
        Student student = null;
        var paymentId = Guid.NewGuid();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => course.EnrollStudent(student, paymentId));
    }

    [Fact]
    public void EnrollStudent_WithoutPaymentWhenFeeRequired_ShouldThrowException()
    {
        // Arrange
        var course = new Course("Programación en C#", new Money(100, "USD"), DateTime.Today.AddDays(10), DateTime.Today.AddDays(40));
        var student = new Student("Juan Pérez", 25);

        // No proporcionamos un ID de pago válido
        var paymentId = Guid.Empty;

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => course.EnrollStudent(student, paymentId));
        Assert.Contains("Payment", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void EnrollStudent_AlreadyEnrolled_ShouldThrowException()
    {
        // Arrange
        var course = new Course("Programación en C#", new Money(100, "USD"), DateTime.Today.AddDays(10), DateTime.Today.AddDays(40));
        var student = new Student("Juan Pérez", 25);
        var paymentId = Guid.NewGuid();

        // Inscribir al estudiante por primera vez
        course.EnrollStudent(student, paymentId);

        // Act & Assert - Intentar inscribir al mismo estudiante de nuevo
        var exception = Assert.Throws<DomainException>(() => course.EnrollStudent(student, paymentId));
        Assert.Contains("already enrolled", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void EnrollStudent_CourseWithoutFee_ShouldEnrollWithoutPaymentId()
    {
        // Arrange
        var course = new Course("Curso Gratuito", new Money(0, "USD"), DateTime.Today.AddDays(10), DateTime.Today.AddDays(40));
        var student = new Student("Juan Pérez", 25);

        // No se requiere ID de pago para cursos gratuitos
        var paymentId = Guid.Empty;

        // Act
        var enrollment = course.EnrollStudent(student, paymentId);

        // Assert
        Assert.NotNull(enrollment);
        Assert.Equal(Guid.Empty, enrollment.PaymentId);
        Assert.Single(course.Enrollments);
    }
}
