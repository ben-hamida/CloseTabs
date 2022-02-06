namespace CloseTabs;

[Command(PackageIds.CloseAllTabsInProject)]
internal sealed class CloseAllTabsInProjectCommand : CloseTabsCommand<CloseAllTabsInProjectCommand>
{
    protected override void BeforeQueryStatus(IVsWindowFrame selectedFrame)
    {
        Command.SetVisibleIfInProject(
            selectedFrame,
            projectName => $"Close All in {projectName}");
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
            if (hierarchy == selectedHierarchy)
            {
                yield return frame;
            }
        }
    }
}