using ACME.Application.Interfaces;
using ACME.Application.Services;
using ACME.Domain.Entities;
using ACME.Domain.Exceptions;
using Moq;

namespace ACME.Tests.Application;

public class StudentServiceTests
{
    private readonly Mock<IStudentRepository> _mockStudentRepository;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly IStudentService _studentService;

    public StudentServiceTests()
    {
        _mockStudentRepository = new Mock<IStudentRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _studentService = new StudentService(_mockStudentRepository.Object, _mockUnitOfWork.Object);
    }

    [Fact]
    public async Task RegisterStudent_WithValidData_ShouldReturnStudent()
    {
        // Arrange
        string name = "Juan Pérez";
        int age = 25;

        Student capturedStudent = null;
        _mockStudentRepository
            .Setup(repo => repo.AddAsync(It.IsAny<Student>()))
            .Callback<Student>(student => capturedStudent = student)
            .Returns(Task.CompletedTask);

        _mockUnitOfWork.Setup(uow => uow.CommitAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _studentService.RegisterStudentAsync(name, age);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(name, result.Name);
        Assert.Equal(age, result.Age);

        _mockStudentRepository.Verify(repo => repo.AddAsync(It.IsAny<Student>()), Times.Once);
        _mockUnitOfWork.Verify(uow => uow.CommitAsync(), Times.Once);

        Assert.NotNull(capturedStudent);
        Assert.Equal(name, capturedStudent.Name);
        Assert.Equal(age, capturedStudent.Age);
    }

    [Fact]
    public async Task GetAllStudents_ShouldReturnAllStudents()
    {
        // Arrange
        var students = new List<Student>
            {
                new Student("Juan Pérez", 25),
                new Student("María López", 30)
            };

        _mockStudentRepository.Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(students);

        // Act
        var result = await _studentService.GetAllStudentsAsync();

        // Assert
        var resultList = result.ToList();
        Assert.Equal(2, resultList.Count);
        Assert.Equal("Juan Pérez", resultList[0].Name);
        Assert.Equal("María López", resultList[1].Name);

        _mockStudentRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetStudentById_WithExistingId_ShouldReturnStudent()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var student = new Student("Juan Pérez", 25);

        // Usamos reflexión para establecer el Id (ya que es una propiedad privada de escritura)
        typeof(Student).GetProperty("Id").SetValue(student, studentId);

        _mockStudentRepository.Setup(repo => repo.GetByIdAsync(studentId))
            .ReturnsAsync(student);

        // Act
        var result = await _studentService.GetStudentByIdAsync(studentId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(studentId, result.Id);
        Assert.Equal("Juan Pérez", result.Name);

        _mockStudentRepository.Verify(repo => repo.GetByIdAsync(studentId), Times.Once);
    }

    [Fact]
    public async Task GetStudentById_WithNonExistingId_ShouldReturnNull()
    {
        // Arrange
        var studentId = Guid.NewGuid();

        _mockStudentRepository.Setup(repo => repo.GetByIdAsync(studentId))
            .ReturnsAsync((Student)null);

        // Act
        var result = await _studentService.GetStudentByIdAsync(studentId);

        // Assert
        Assert.Null(result);

        _mockStudentRepository.Verify(repo => repo.GetByIdAsync(studentId), Times.Once);
    }

    [Fact]
    public async Task RegisterStudent_WithEmptyName_ShouldThrowArgumentException()
    {
        // Arrange
        string name = "";
        int age = 25;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _studentService.RegisterStudentAsync(name, age));
    }

    [Fact]
    public async Task RegisterStudent_WithAgeBelowMinimum_ShouldThrowArgumentException()
    {
        // Arrange
        string name = "Juan";
        int age = 10; // Menor que la edad mínima

        // Act & Assert
        await Assert.ThrowsAsync<DomainException>(() => _studentService.RegisterStudentAsync(name, age));
    }

    [Fact]
    public async Task GetStudentById_WithEmptyGuid_ShouldReturnNull()
    {
        // Arrange
        var emptyId = Guid.Empty;
        _mockStudentRepository.Setup(repo => repo.GetByIdAsync(emptyId))
            .ReturnsAsync((Student)null);

        // Act
        var result = await _studentService.GetStudentByIdAsync(emptyId);

        // Assert
        Assert.Null(result);
        _mockStudentRepository.Verify(repo => repo.GetByIdAsync(emptyId), Times.Once);
    }

}
