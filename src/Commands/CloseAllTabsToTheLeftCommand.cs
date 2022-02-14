﻿namespace CloseTabs;

[Command(PackageIds.CloseAllTabsToTheLeft)]
internal sealed class CloseAllTabsToTheLeftCommand : BaseCommand<CloseAllTabsToTheLeftCommand>
{
    private IEnumerable<IVsWindowFrame> _framesToClose = Enumerable.Empty<IVsWindowFrame>();

    protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
    {
        await _framesToClose.ToList().CloseAllAsync();
    }

    protected override void BeforeQueryStatus(EventArgs e) => BeforeQueryStatusAsync().FireAndForget();

    private async Task BeforeQueryStatusAsync()
    {
        _framesToClose = Enumerable.Empty<IVsWindowFrame>();
        IVsWindowFrame? selectedFrame = await WindowFrameUtilities.GetSelectedFrameAsync();
        if (selectedFrame == null)
        {
            return;
        }

        _framesToClose = WindowFrameUtilities.GetOrderedFramesOfActiveWindow()
            .TakeWhile(frame => frame != selectedFrame);
        Command.Enabled = _framesToClose.Any();
    }
}