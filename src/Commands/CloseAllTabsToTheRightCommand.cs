namespace CloseTabs;

[Command(PackageIds.CloseAllTabsToTheRight)]
internal sealed class CloseAllTabsToTheRightCommand : BaseCommand<CloseAllTabsToTheRightCommand>
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
            .SkipWhile(frame => frame != selectedFrame).Skip(1).ToArray();
        Command.Enabled = _framesToClose.Any();
    }
}