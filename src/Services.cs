namespace CloseTabs;

internal static class Services
{
    private static IVsUIShell? _vsUIShell;
    private static IVsMonitorSelection? _vsMonitorSelection;

    public static async Task InitializeAsync()
    {
        _vsUIShell = await VS.Services.GetUIShellAsync();
        _vsMonitorSelection = await VS.Services.GetMonitorSelectionAsync();
    }

    public static IVsUIShell VsUIShell =>
        _vsUIShell ?? throw new InvalidOperationException("Services not initialized");

    public static IVsMonitorSelection VsMonitorSelection =>
        _vsMonitorSelection ?? throw new InvalidOperationException("Services not initialized");
}