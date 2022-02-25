namespace CloseTabs;

internal static class HierarchyUtilities
{
    public static IVsHierarchy? GetProjectHierarchyOfCurrentWindowFrame()
    {
        ThreadHelper.ThrowIfNotOnUIThread();
        return Services.VsMonitorSelection.GetSelectedFrame()?.TryGetProjectHierarchy();
    }
}