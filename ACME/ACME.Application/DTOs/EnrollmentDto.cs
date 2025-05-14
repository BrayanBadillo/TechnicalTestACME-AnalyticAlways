namespace ACME.Application.DTOs;

public class EnrollmentDto
{
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
    public string CourseName { get; set; } = null!;
    public Guid StudentId { get; set; }
    public string StudentName { get; set; } = null!;
    public DateTime EnrollmentDate { get; set; }
    public Guid? PaymentId { get; set; }
}