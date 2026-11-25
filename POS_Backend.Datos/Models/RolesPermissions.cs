namespace POS_Backend.Datos.Models;

public class RolesPermissions
{
    public int RoleId { get; set; }
    public int PermissionId { get; set; }

    public Roles Role { get; set; } = null!;
    public Permissions Permission { get; set; } = null!;
}