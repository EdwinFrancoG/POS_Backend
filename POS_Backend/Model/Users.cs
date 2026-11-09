namespace POS_Backend.Model;

public class Users
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public int FailedLoginCount { get; set; }
    public DateTime? LockoutEndAt { get; set; }

    public ICollection<UserRoles> UserRoles { get; set; } = new List<UserRoles>();
    public ICollection<CashClosings> CashClosings { get; set; } = new List<CashClosings>();
    public ICollection<Invoices> Invoices { get; set; } = new List<Invoices>();
    public ICollection<Payments> Payments { get; set; } = new List<Payments>();
}