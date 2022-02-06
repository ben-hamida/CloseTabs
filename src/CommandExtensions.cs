namespace CloseTabs;

internal static class CommandExtensions
{
    /// <summary>
    /// Sets the command visibility to visible of the specified frame is associated with a
    /// hierarchy that is child to a project or solution folder.
    /// </summary>
    public static void SetVisibleIfInProject(
        this OleMenuCommand command,
        IVsWindowFrame frame,
        Func<string, string> getText)
    {
        ThreadHelper.ThrowIfNotOnUIThread();
        IVsHierarchy? hierarchy = frame.TryGetProjectHierarchy();
        if (hierarchy == null)
        {
            command.Visible = false;
            return;
        }

        command.Visible = true;
        string projectName = hierarchy.ToHierarchyItem((uint)VSConstants.VSITEMID.Root).CanonicalName;
        command.Text = getText(projectName);
    }
}
