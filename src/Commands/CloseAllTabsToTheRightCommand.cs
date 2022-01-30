namespace CloseTabs;

[Command(PackageIds.CloseAllTabsToTheRight)]
internal sealed class CloseAllTabsToTheRightCommand : CloseTabsCommand<CloseAllTabsToTheRightCommand>
{
    protected override IEnumerable<IVsWindowFrame> GetFramesToClose(
        IEnumerable<IVsWindowFrame> orderedFrames,
        IVsWindowFrame selectedFrame)
    {
        return orderedFrames.SkipWhile(frame => frame != selectedFrame).Skip(1);
    }
}