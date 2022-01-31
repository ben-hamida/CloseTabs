namespace CloseTabs;

[Command(PackageIds.CloseAllTabsInProject)]
internal sealed class ClassAllTabsInProjectCommand : CloseTabsCommand<ClassAllTabsInProjectCommand>
{
    protected override void BeforeQueryStatus(IVsWindowFrame selectedFrame)
    {
        if (!selectedFrame.TryGetProjectHierarchy(out IVsHierarchy? hierarchy))
        {
            Command.Visible = false;
            return;
        }

        Command.Visible = true;
        string projectName = hierarchy!.ToHierarchyItem((uint)VSConstants.VSITEMID.Root).CanonicalName;
        Command.Text = $"Close All in Project {projectName}";
    }

    protected override IEnumerable<IVsWindowFrame> GetFramesToClose(
        IEnumerable<IVsWindowFrame> orderedFrames,
        IVsWindowFrame selectedFrame)
    {
        if (!selectedFrame.TryGetProjectHierarchy(out IVsHierarchy? selectedHierarchy))
        {
            yield break;
        }

        foreach (IVsWindowFrame frame in orderedFrames)
        {
            if (frame.TryGetProjectHierarchy(out IVsHierarchy? hierarchy) && hierarchy == selectedHierarchy)
            {
                yield return frame;
            }
        }
    }
}