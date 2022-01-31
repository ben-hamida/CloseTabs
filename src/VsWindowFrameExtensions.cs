namespace CloseTabs;

internal static class VsWindowFrameExtensions
{
    /// <summary>
    /// Gets the IVsHierarchy that represents the project for the specified IVsWindowFrame.
    /// </summary>
    public static bool TryGetProjectHierarchy(this IVsWindowFrame frame, out IVsHierarchy? hierarchy)
    {
        ThreadHelper.ThrowIfNotOnUIThread();

        // Apparently getting the VSFPROPID_Hierarchy property gets the hierarchy for the project and not the document itself
        if (frame.GetProperty((int)__VSFPROPID.VSFPROPID_Hierarchy, out object obj) == VSConstants.S_OK &&
            obj is IVsHierarchy vsHierarchy)
        {
            hierarchy = vsHierarchy;
            return true;
        }

        hierarchy = null;
        return false;
    }
}