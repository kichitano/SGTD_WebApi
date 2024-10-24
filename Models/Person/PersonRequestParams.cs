namespace SGTD_WebApi.Models.Person;

public class PersonRequestParams
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Phone { get; set; }
    public string NationalityCode { get; set; }
    public string DocumentNumber { get; set; }
    public bool Gender { get; set; }
}