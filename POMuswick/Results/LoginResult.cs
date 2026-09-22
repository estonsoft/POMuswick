using POMuswick;
using POMuswick.Models;

public class LoginResult
{
    public bool Success { get; set; }

    public string Status { get; set; }
    public string ErrorMessage { get; set; } = "";

    public Customer Customer { get; set; }

    public AppSettings Settings { get; set; }

    public POMuswick.Location Location { get; set; }

    public bool RefreshData { get; set; }

    public bool IsSalesUser { get; set; }
}