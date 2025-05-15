using ACME.Application.Interfaces;
using ACME.Application.Services;
using ACME.Domain.Entities;
using ACME.Domain.Exceptions;
using ACME.Domain.ValueObjects;
using Moq;

namespace ACME.Tests.Application;

public class CourseServiceTests
{
    private readonly Mock<ICourseRepository> _mockCourseRepository;
    private readonly Mock<IStudentRepository> _mockStudentRepository;
    private readonly Mock<IPaymentGateway> _mockPaymentGateway;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly ICourseService _courseService;

    public CourseServiceTests()
    {
        _mockCourseRepository = new Mock<ICourseRepository>();
        _mockStudentRepository = new Mock<IStudentRepository>();
        _mockPaymentGateway = new Mock<IPaymentGateway>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();

        _courseService = new CourseService(
            _mockCourseRepository.Object,
            _mockStudentRepository.Object,
            _mockPaymentGateway.Object,
            _mockUnitOfWork.Object);
    }

    [Fact]
    public async Task RegisterCourse_WithValidData_ShouldReturnCourse()
    {
        // Arrange
        string name = "Programación en C#";
        decimal fee = 100;
        string currency = "USD";
        var startDate = DateTime.Today.AddDays(10);
        var endDate = DateTime.Today.AddDays(40);

        Course capturedCourse = null;
        _mockCourseRepository
            .Setup(repo => repo.AddAsync(It.IsAny<Course>()))
            .Callback<Course>(course => capturedCourse = course)
            .Returns(Task.CompletedTask);

        _mockUnitOfWork.Setup(uow => uow.CommitAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _courseService.RegisterCourseAsync(name, fee, currency, startDate, endDate);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(name, result.Name);
        Assert.Equal(fee, result.RegistrationFee.Amount);
        Assert.Equal(currency, result.RegistrationFee.Currency);
        Assert.Equal(startDate, result.StartDate);
        Assert.Equal(endDate, result.EndDate);

        _mockCourseRepository.Verify(repo => repo.AddAsync(It.IsAny<Course>()), Times.Once);
        _mockUnitOfWork.Verify(uow => uow.CommitAsync(), Times.Once);

        Assert.NotNull(capturedCourse);
        Assert.Equal(name, capturedCourse.Name);
        Assert.Equal(fee, capturedCourse.RegistrationFee.Amount);
    }

    [Fact]
    public async Task EnrollStudentInCourse_WithValidIds_ShouldReturnEnrollment()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var studentId = Guid.NewGuid();
        var paymentId = Guid.NewGuid();

        var student = new Student("Juan Pérez", 25);
        var course = new Course("Programación en C#", new Money(100, "USD"), DateTime.Today.AddDays(10), DateTime.Today.AddDays(40));

        // Usamos reflexión para establecer Ids (ya que son propiedades privadas de escritura)
        typeof(Student).GetProperty("Id").SetValue(student, studentId);
        typeof(Course).GetProperty("Id").SetValue(course, courseId);

        _mockStudentRepository.Setup(repo => repo.GetByIdAsync(studentId))
            .ReturnsAsync(student);

        _mockCourseRepository.Setup(repo => repo.GetByIdAsync(courseId))
            .ReturnsAsync(course);

        _mockPaymentGateway.Setup(gateway => gateway.ProcessPaymentAsync(
                It.IsAny<Money>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
            .ReturnsAsync(paymentId);

        _mockUnitOfWork.Setup(uow => uow.CommitAsync())
            .Returns(Task.CompletedTask);

        // Act
        var result = await _courseService.EnrollStudentInCourseAsync(studentId, courseId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(student, result.Student);
        Assert.Equal(course, result.Course);
        Assert.Equal(paymentId, result.PaymentId);

        _mockStudentRepository.Verify(repo => repo.GetByIdAsync(studentId), Times.Once);
        _mockCourseRepository.Verify(repo => repo.GetByIdAsync(courseId), Times.Once);
        _mockPaymentGateway.Verify(
            gateway => gateway.ProcessPaymentAsync(It.IsAny<Money>(), It.IsAny<string>(), It.IsAny<string>()),
            Times.Once);
        _mockUnitOfWork.Verify(uow => uow.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task EnrollStudentInCourse_WithNonExistingCourse_ShouldThrowException()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var studentId = Guid.NewGuid();

        _mockCourseRepository.Setup(repo => repo.GetByIdAsync(courseId))
            .ReturnsAsync((Course)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<DomainException>(
            () => _courseService.EnrollStudentInCourseAsync(studentId, courseId));

        Assert.Contains(courseId.ToString(), exception.Message);

        _mockCourseRepository.Verify(repo => repo.GetByIdAsync(courseId), Times.Once);
        _mockStudentRepository.Verify(repo => repo.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
        _mockPaymentGateway.Verify(
            gateway => gateway.ProcessPaymentAsync(It.IsAny<Money>(), It.IsAny<string>(), It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task EnrollStudentInCourse_WithNonExistingStudent_ShouldThrowException()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var studentId = Guid.NewGuid();

        var course = new Course("Programación en C#", new Money(100, "USD"), DateTime.Today.AddDays(10), DateTime.Today.AddDays(40));

        // Usamos reflexión para establecer Id (ya que es una propiedad privada de escritura)
        typeof(Course).GetProperty("Id").SetValue(course, courseId);

        _mockCourseRepository.Setup(repo => repo.GetByIdAsync(courseId))
            .ReturnsAsync(course);

        _mockStudentRepository.Setup(repo => repo.GetByIdAsync(studentId))
            .ReturnsAsync((Student)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<DomainException>(
            () => _courseService.EnrollStudentInCourseAsync(studentId, courseId));

        Assert.Contains(studentId.ToString(), exception.Message);

        _mockCourseRepository.Verify(repo => repo.GetByIdAsync(courseId), Times.Once);
        _mockStudentRepository.Verify(repo => repo.GetByIdAsync(studentId), Times.Once);
        _mockPaymentGateway.Verify(
            gateway => gateway.ProcessPaymentAsync(It.IsAny<Money>(), It.IsAny<string>(), It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task GetCoursesByDateRange_ShouldReturnCoursesInRange()
    {
        // Arrange
        var startDate = DateTime.Today;
        var endDate = DateTime.Today.AddDays(30);

        var courses = new List<Course>
            {
                new Course("Curso 1", new Money(100, "USD"), startDate.AddDays(5), endDate.AddDays(5)),
                new Course("Curso 2", new Money(150, "USD"), startDate.AddDays(10), endDate.AddDays(10))
            };

        _mockCourseRepository.Setup(repo => repo.GetCoursesByDateRangeAsync(startDate, endDate))
            .ReturnsAsync(courses);

        // Act
        var result = await _courseService.GetCoursesByDateRangeAsync(startDate, endDate);

        // Assert
        var resultList = result.ToList();
        Assert.Equal(2, resultList.Count);
        Assert.Equal("Curso 1", resultList[0].Name);
        Assert.Equal("Curso 2", resultList[1].Name);

        _mockCourseRepository.Verify(repo => repo.GetCoursesByDateRangeAsync(startDate, endDate), Times.Once);
    }

    [Fact]
    public async Task RegisterCourse_WithEmptyName_ShouldThrowArgumentException()
    {
        // Arrange
        string name = "";
        decimal fee = 100;
        string currency = "USD";
        var startDate = DateTime.Today.AddDays(1);
        var endDate = DateTime.Today.AddDays(10);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _courseService.RegisterCourseAsync(name, fee, currency, startDate, endDate));
    }

    [Fact]
    public async Task RegisterCourse_WithNegativeFee_ShouldThrowArgumentException()
    {
        // Arrange
        string name = "Curso";
        decimal fee = -50;
        string currency = "USD";
        var startDate = DateTime.Today.AddDays(1);
        var endDate = DateTime.Today.AddDays(10);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _courseService.RegisterCourseAsync(name, fee, currency, startDate, endDate));
    }

    [Fact]
    public async Task RegisterCourse_WithEndDateBeforeStartDate_ShouldThrowArgumentException()
    {
        // Arrange
        string name = "Curso";
        decimal fee = 100;
        string currency = "USD";
        var startDate = DateTime.Today.AddDays(10);
        var endDate = DateTime.Today.AddDays(1);

        // Act & Assert
        await Assert.ThrowsAsync<DomainException>(() =>
            _courseService.RegisterCourseAsync(name, fee, currency, startDate, endDate));
    }

    [Fact]
    public async Task GetCourseById_WithExistingId_ShouldReturnCourse()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var course = new Course("Curso", new Money(100, "USD"), DateTime.Today, DateTime.Today.AddDays(1));
        typeof(Course).GetProperty("Id").SetValue(course, courseId);

        _mockCourseRepository.Setup(repo => repo.GetByIdAsync(courseId))
            .ReturnsAsync(course);

        // Act
        var result = await _courseService.GetCourseByIdAsync(courseId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(courseId, result.Id);
        _mockCourseRepository.Verify(repo => repo.GetByIdAsync(courseId), Times.Once);
    }

    [Fact]
    public async Task GetCourseById_WithNonExistingId_ShouldReturnNull()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        _mockCourseRepository.Setup(repo => repo.GetByIdAsync(courseId))
            .ReturnsAsync((Course)null);

        // Act
        var result = await _courseService.GetCourseByIdAsync(courseId);

        // Assert
        Assert.Null(result);
        _mockCourseRepository.Verify(repo => repo.GetByIdAsync(courseId), Times.Once);
    }

    [Fact]
    public async Task GetAllCourses_ShouldReturnAllCourses()
    {
        // Arrange
        var courses = new List<Course>
    {
        new Course("Curso 1", new Money(100, "USD"), DateTime.Today, DateTime.Today.AddDays(1)),
        new Course("Curso 2", new Money(200, "USD"), DateTime.Today, DateTime.Today.AddDays(2))
    };

        _mockCourseRepository.Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(courses);

        // Act
        var result = await _courseService.GetAllCoursesAsync();

        // Assert
        var resultList = result.ToList();
        Assert.Equal(2, resultList.Count);
        Assert.Equal("Curso 1", resultList[0].Name);
        Assert.Equal("Curso 2", resultList[1].Name);
        _mockCourseRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
    }

}