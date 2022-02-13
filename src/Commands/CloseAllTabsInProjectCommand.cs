﻿namespace CloseTabs;

[Command(PackageIds.CloseAllTabsInProject)]
internal sealed class CloseAllTabsInProjectCommand : BaseCommand<CloseAllTabsInProjectCommand>
{
    private IVsWindowFrame[] _framesToClose = Array.Empty<IVsWindowFrame>();

    protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
    {
        await _framesToClose.CloseAllAsync();
    }

    protected override void BeforeQueryStatus(EventArgs e) => BeforeQueryStatusAsync().FireAndForget();

    private async Task BeforeQueryStatusAsync()
    {
        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

        Command.Visible = false;
        _framesToClose = Array.Empty<IVsWindowFrame>();
        IVsHierarchy? selectedProjectHierarchy = await HierarchyUtilities.GetProjectHierarchyOfCurrentWindowFrameAsync();
        if (selectedProjectHierarchy == null)
        {
            return;
        }

        Command.Visible = true;
        Command.Text = $"Close All in {selectedProjectHierarchy.GetCanonicalName()}";
        _framesToClose = (await WindowFrameUtilities.GetAllDocumentsInActiveWindowAsync())
            .Where(frame => frame.TryGetProjectHierarchy() == selectedProjectHierarchy)
            .ToArray();
        Command.Enabled = _framesToClose.Any();
    }
}