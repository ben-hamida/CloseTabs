namespace CloseTabs;

[Command(PackageIds.CloseAllTabsToTheLeft)]
internal sealed class CloseAllTabsToTheLeftCommand : BaseCommand<CloseAllTabsToTheLeftCommand>
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
        Command.Enabled = false;
        _framesToClose = Enumerable.Empty<IVsWindowFrame>();
        IVsWindowFrame? selectedFrame = Services.VsMonitorSelection.GetSelectedFrame();
        if (selectedFrame == null || !selectedFrame.IsDocument())
        {
            return;
        }

        _framesToClose = WindowFrameUtilities.GetOrderedFramesOfActiveWindow()
            .TakeWhile(frame => frame != selectedFrame);
        Command.Enabled = _framesToClose.Any();
    }
}