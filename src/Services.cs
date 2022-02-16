namespace CloseTabs;

internal static class Services
{
    private static IVsUIShell? _vsUIShell;
    private static IVsMonitorSelection? _vsMonitorSelection;
    private static IVsTextManager? _vsTextManager;

    public static async Task InitializeAsync()
    {
        _vsUIShell = await VS.Services.GetUIShellAsync();
        _vsMonitorSelection = await VS.Services.GetMonitorSelectionAsync();
        _vsTextManager = await VS.GetRequiredServiceAsync<SVsTextManager, IVsTextManager>();
    }

    public static IVsUIShell VsUIShell => _vsUIShell ?? throw Exception;

    public static IVsMonitorSelection VsMonitorSelection => _vsMonitorSelection ?? throw Exception;

    public static IVsTextManager VsTextManager => _vsTextManager ?? throw Exception;

    private static Exception Exception => throw new InvalidOperationException("Services not initialized");
}