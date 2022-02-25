namespace CloseTabs;

[Command(PackageIds.CloseAllTabsNotInProject)]
internal sealed class CloseAllTabsNotInProjectCommand : BaseCommand<CloseAllTabsNotInProjectCommand>
{
    private IEnumerable<IVsWindowFrame> _framesToClose = Enumerable.Empty<IVsWindowFrame>();

    protected override void Execute(object sender, EventArgs e)
    {
        ThreadHelper.ThrowIfNotOnUIThread();
        _framesToClose.ToList().CloseAll();
    }

    protected override void BeforeQueryStatus(EventArgs e)
    {
        ThreadHelper.ThrowIfNotOnUIThread();
        Command.Visible = false;
        Command.Enabled = false;
        _framesToClose = Enumerable.Empty<IVsWindowFrame>();
        IVsHierarchy? selectedProjectHierarchy = HierarchyUtilities.GetProjectHierarchyOfCurrentWindowFrame();
        if (selectedProjectHierarchy == null)
        {
            return;
        }

        Command.Visible = true;
        Command.Text = $"Close All not in {selectedProjectHierarchy.GetCanonicalName()}";
        _framesToClose = WindowFrameUtilities.GetAllDocumentsInActiveWindow()
            .Where(frame => frame.TryGetProjectHierarchy() != selectedProjectHierarchy);
        Command.Enabled = _framesToClose.Any();
    }
}