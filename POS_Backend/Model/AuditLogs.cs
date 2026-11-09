namespace POS_Backend.Model;

public class AuditLogs
{
    public long Id { get; set; }
    public DateTime Date { get; set; }
    public int? UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public AuditAction Action { get; set; }
    public string Module { get; set; } = string.Empty;
    public string Entity { get; set; } = string.Empty;
    public int? EntityId { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? Changes { get; set; }
}