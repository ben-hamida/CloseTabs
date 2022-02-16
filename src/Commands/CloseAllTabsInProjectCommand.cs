namespace CloseTabs;

[Command(PackageIds.CloseAllTabsInProject)]
internal sealed class CloseAllTabsInProjectCommand : BaseCommand<CloseAllTabsInProjectCommand>
{
    private IVsHierarchy? _selectedProjectHierarchy;

    protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
    {
        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
        IEnumerable<IVsWindowFrame> documents = WindowFrameUtilities.GetAllDocumentsInActiveWindow();
        documents
            .Where(frame => frame.TryGetProjectHierarchy() == _selectedProjectHierarchy)
            .ToList()
            .CloseAll();
    }

    protected override void BeforeQueryStatus(EventArgs e)
    {
        ThreadHelper.ThrowIfNotOnUIThread();
        Command.Visible = false;
        Command.Enabled = false;
        _selectedProjectHierarchy = HierarchyUtilities.GetProjectHierarchyOfCurrentWindowFrame();
        if (_selectedProjectHierarchy == null)
        {
            return;
        }

        Command.Enabled = true;
        Command.Visible = true;
        Command.Text = $"Close All in {_selectedProjectHierarchy.GetCanonicalName()}";
    }
}