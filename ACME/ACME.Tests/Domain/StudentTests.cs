using ACME.Domain.Entities;
using ACME.Domain.Exceptions;

namespace ACME.Tests.Domain;

public class StudentTests
{
    [Fact]
    public void Constructor_Should_Set_Properties()
    {
        var student = new Student("Ana", 20);
        Assert.Equal("Ana", student.Name);
        Assert.Equal(20, student.Age);
        Assert.NotEqual(Guid.Empty, student.Id);
    }

    [Fact]
    public void Constructor_Should_Throw_If_Age_Is_Less_Than_Minimum()
    {
        Assert.Throws<DomainException>(() => new Student("Ana", 10));
    }
}