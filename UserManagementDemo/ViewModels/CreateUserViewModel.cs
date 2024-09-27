namespace UserManagementDemo.ViewModels;

public class CreateUserViewModel
{
  public string Id { get; set; } = String.Empty;
  public string  UserName { get; set; } = String.Empty;
  public string  Email { get; set; }  = String.Empty;
  public string PhoneNumber { get; set; }  = String.Empty;
  public string  Password { get; set; } = String.Empty;
}