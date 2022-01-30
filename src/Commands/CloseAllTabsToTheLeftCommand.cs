namespace CloseTabs;

[Command(PackageIds.CloseAllTabsToTheLeft)]
internal sealed class CloseAllTabsToTheLeftCommand : CloseTabsCommand<CloseAllTabsToTheLeftCommand>
{
    protected override IEnumerable<IVsWindowFrame> GetFramesToClose(
        IEnumerable<IVsWindowFrame> orderedFrames,
        IVsWindowFrame selectedFrame)
    {
        return orderedFrames.TakeWhile(frame => frame != selectedFrame);
    }
}