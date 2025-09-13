namespace RBZ.Projekt.Models.REST;

public class ActorPatchRequest
{
    public int? ActorId { get; set; }
    public string? FirstName { get; set; } = string.Empty;
    public string? LastName { get; set; } = string.Empty;
}
