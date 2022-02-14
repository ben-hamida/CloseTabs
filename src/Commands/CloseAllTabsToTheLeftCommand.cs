namespace CloseTabs;

[Command(PackageIds.CloseAllTabsToTheLeft)]
internal sealed class CloseAllTabsToTheLeftCommand : BaseCommand<CloseAllTabsToTheLeftCommand>
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
        _framesToClose = Enumerable.Empty<IVsWindowFrame>();
        IVsWindowFrame? selectedFrame = Services.VsMonitorSelection.GetSelectedFrame();
        if (selectedFrame == null)
        {
            return;
        }

        _framesToClose = WindowFrameUtilities.GetOrderedFramesOfActiveWindow()
            .TakeWhile(frame => frame != selectedFrame);
        Command.Enabled = _framesToClose.Any();
    }
}