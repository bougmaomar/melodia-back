namespace melodia_api.Models.Employee;

public class EmployeeViewDto
{
    public long Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public char Sexe { get; set; }
    public DateTime HiringDate { get; set; }
    public DateTime? DepartureDate { get; set; }
    public string PositionName { get; set; }
    public bool Active { get; set; }
}