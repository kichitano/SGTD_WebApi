namespace SGTD_WebApi.Models.User;

public class UserDto
{
    public int Id { get; set; }
    public int PersonId { get; set; }
    public string Email { get; set; }
    public bool Status { get; set; }
}