namespace ACME.Application.DTOs;

public class CourseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public decimal RegistrationFee { get; set; }
    public string Currency { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<EnrollmentDto> Enrollments { get; set; } = new List<EnrollmentDto>();
}