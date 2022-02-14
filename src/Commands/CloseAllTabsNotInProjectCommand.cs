namespace CloseTabs;

[Command(PackageIds.CloseAllTabsNotInProject)]
internal sealed class CloseAllTabsNotInProjectCommand : BaseCommand<CloseAllTabsNotInProjectCommand>
{
    private IEnumerable<IVsWindowFrame> _framesToClose = Enumerable.Empty<IVsWindowFrame>();

    protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
    {
        await _framesToClose.ToList().CloseAllAsync();
    }

    protected override void BeforeQueryStatus(EventArgs e) => BeforeQueryStatusAsync().FireAndForget();

    private async Task BeforeQueryStatusAsync()
    {
        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

        Command.Visible = false;
        _framesToClose = Enumerable.Empty<IVsWindowFrame>();
        IVsHierarchy? selectedProjectHierarchy = await HierarchyUtilities.GetProjectHierarchyOfCurrentWindowFrameAsync();
        if (selectedProjectHierarchy == null)
        {
            return;
        }

        Command.Visible = true;
        Command.Text = $"Close All not in {selectedProjectHierarchy.GetCanonicalName()}";
        _framesToClose = (await WindowFrameUtilities.GetAllDocumentsInActiveWindowAsync())
            .Where(frame => frame.TryGetProjectHierarchy() != selectedProjectHierarchy);
        Command.Enabled = _framesToClose.Any();
    }
}