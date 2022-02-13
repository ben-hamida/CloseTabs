namespace CloseTabs;

[Command(PackageIds.CloseAllTabsToTheLeft)]
internal sealed class CloseAllTabsToTheLeftCommand : BaseCommand<CloseAllTabsToTheLeftCommand>
{
    private IVsWindowFrame[] _framesToClose = Array.Empty<IVsWindowFrame>();

    protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
    {
        await _framesToClose.CloseAllAsync();
    }

    protected override void BeforeQueryStatus(EventArgs e) => BeforeQueryStatusAsync().FireAndForget();

    private async Task BeforeQueryStatusAsync()
    {
        _framesToClose = Array.Empty<IVsWindowFrame>();
        IVsWindowFrame? selectedFrame = await WindowFrameUtilities.GetSelectedFrameAsync();
        if (selectedFrame == null)
        {
            return;
        }

        _framesToClose = WindowFrameUtilities.GetOrderedFramesOfActiveWindow()
            .TakeWhile(frame => frame != selectedFrame).ToArray();
        Command.Enabled = _framesToClose.Any();
    }
}