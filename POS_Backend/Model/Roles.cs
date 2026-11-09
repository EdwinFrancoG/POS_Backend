namespace POS_Backend.Model;

public class Roles
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }

    public ICollection<UserRoles> UserRoles { get; set; } = new List<UserRoles>();
    public ICollection<RolesPermissions> RolesPermissions { get; set; } = new List<RolesPermissions>();
}