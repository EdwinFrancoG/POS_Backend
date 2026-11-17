namespace POS_Backend.Datos.Models;

public class Permissions
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Module { get; set; } = string.Empty;

    public ICollection<RolesPermissions> RolesPermissions { get; set; } = new List<RolesPermissions>();
}