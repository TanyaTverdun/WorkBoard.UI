namespace WorkBoard.Domain.Options;

public class AzureAdOptions
{
    public required string Authority { get; set; }
    public required string ClientId { get; set; }
    public required string BackendClientId { get; set; }
    public bool ValidateAuthority { get; set; }
}
