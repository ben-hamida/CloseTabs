namespace CloseTabs;

[Command(PackageIds.CloseAllTabsExceptInProject)]
internal sealed class CloseAllTabsExceptInProjectCommand : CloseTabsCommand<CloseAllTabsExceptInProjectCommand>
{
    protected override void BeforeQueryStatus(IVsWindowFrame selectedFrame)
    {
        ThreadHelper.ThrowIfNotOnUIThread();
        IVsHierarchy? hierarchy = selectedFrame.TryGetProjectHierarchy();
        if (hierarchy == null)
        {
            Command.Visible = false;
            return;
        }

        Command.Visible = true;
        string projectName = hierarchy.ToHierarchyItem((uint)VSConstants.VSITEMID.Root).CanonicalName;
        Command.Text = $"Close All Except in Project {projectName}";
    }

    protected override IEnumerable<IVsWindowFrame> GetFramesToClose(
        IEnumerable<IVsWindowFrame> orderedFrames,
        IVsWindowFrame selectedFrame)
    {
        ThreadHelper.ThrowIfNotOnUIThread();
        IVsHierarchy? selectedHierarchy = selectedFrame.TryGetProjectHierarchy();
        if (selectedHierarchy == null)
        {
            yield break;
        }

        foreach (IVsWindowFrame frame in orderedFrames)
        {
            IVsHierarchy? hierarchy = frame.TryGetProjectHierarchy();
            if (hierarchy != selectedHierarchy)
            {
                yield return frame;
            }
        }
    }
}