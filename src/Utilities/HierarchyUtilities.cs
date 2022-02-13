namespace CloseTabs;

internal static class HierarchyUtilities
{
    public static async Task<IVsHierarchy?> GetProjectHierarchyOfCurrentWindowFrameAsync()
    {
        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
        IVsWindowFrame? selectedFrame = await WindowFrameUtilities.GetSelectedFrameAsync();
        return selectedFrame?.TryGetProjectHierarchy();
    }
}