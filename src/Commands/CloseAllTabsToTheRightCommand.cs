namespace CloseTabs;

[Command(PackageIds.CloseAllTabsToTheRight)]
internal sealed class CloseAllTabsToTheRightCommand : BaseCommand<CloseAllTabsToTheRightCommand>
{
    private IEnumerable<IVsWindowFrame> _framesToClose = Enumerable.Empty<IVsWindowFrame>();

    protected override Task ExecuteAsync(OleMenuCmdEventArgs e)
    {
        _framesToClose.ToList().CloseAll();
        return Task.CompletedTask;
    }

    protected override void BeforeQueryStatus(EventArgs e)
    {
        ThreadHelper.ThrowIfNotOnUIThread();
        Command.Enabled = false;
        _framesToClose = Enumerable.Empty<IVsWindowFrame>();
        IVsWindowFrame? selectedFrame = Services.VsMonitorSelection.GetSelectedFrame();
        if (selectedFrame == null || !selectedFrame.IsDocument())
        {
            return;
        }

        _framesToClose = WindowFrameUtilities.GetOrderedFramesOfActiveWindow()
            .SkipWhile(frame => frame != selectedFrame).Skip(1);
        Command.Enabled = _framesToClose.Any();
    }
}