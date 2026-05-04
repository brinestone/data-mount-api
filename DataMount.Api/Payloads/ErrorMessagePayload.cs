namespace DataMount.Api.Payloads;

public record ErrorMessagePayload
{
    public string? Message { get; set; }
    public int Status { get; set; } = 500;
}