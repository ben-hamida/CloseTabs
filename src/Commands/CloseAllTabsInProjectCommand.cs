namespace CloseTabs;

[Command(PackageIds.CloseAllTabsInProject)]
internal sealed class CloseAllTabsInProjectCommand : BaseCommand<CloseAllTabsInProjectCommand>
{
    private IVsHierarchy? _selectedProjectHierarchy;

    protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
    {
        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
        IEnumerable<IVsWindowFrame> documents = await WindowFrameUtilities.GetAllDocumentsInActiveWindowAsync();
        await documents
            .Where(frame => frame.TryGetProjectHierarchy() == _selectedProjectHierarchy)
            .ToList()
            .CloseAllAsync();
    }

    protected override void BeforeQueryStatus(EventArgs e) => BeforeQueryStatusAsync().FireAndForget();

    private async Task BeforeQueryStatusAsync()
    {
        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

        Command.Visible = false;
        _selectedProjectHierarchy = await HierarchyUtilities.GetProjectHierarchyOfCurrentWindowFrameAsync();
        if (_selectedProjectHierarchy == null)
        {
            return;
        }

        Command.Visible = true;
        Command.Text = $"Close All in {_selectedProjectHierarchy.GetCanonicalName()}";
    }
}