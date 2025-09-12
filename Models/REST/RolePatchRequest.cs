namespace RBZ.Projekt.Models.REST;

public class RolePatchRequest
{
    public int? RoleId { get; set; }
    public string Name  { get; set; } = string.Empty;
}
