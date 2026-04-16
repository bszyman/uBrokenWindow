namespace uBrokenWindow.Models;

public class BrokenWindowConfiguration
{
    public bool EnableBrokenWindow { get; set; } = false;
    public string? NewAdminEmail { get; set; }
    public string? NewAdminPassword { get; set; }
}