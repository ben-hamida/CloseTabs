namespace CloseTabs;

internal static class VsWindowFrameExtensions
{
    /// <summary>
    /// Gets the IVsHierarchy that represents the project for the specified IVsWindowFrame.
    /// </summary>
    public static IVsHierarchy? TryGetProjectHierarchy(this IVsWindowFrame frame)
    {
        ThreadHelper.ThrowIfNotOnUIThread();

        // Apparently getting the VSFPROPID_Hierarchy property gets the hierarchy for the project and not the document itself
        frame.GetProperty((int)__VSFPROPID.VSFPROPID_Hierarchy, out object obj);
        return obj as IVsHierarchy;
    }
}