namespace CloseTabs;

internal static class HierarchyUtilities
{
    public static IVsHierarchy? GetProjectHierarchyOfCurrentWindowFrame()
    {
        return Services.VsMonitorSelection.GetSelectedFrame()?.TryGetProjectHierarchy();
    }
}