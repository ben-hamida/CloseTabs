namespace CloseTabs;

[Command(PackageIds.CloseAllUnchangedTabs)]
internal sealed class CloseAllUnchangedTabsCommand : BaseCommand<CloseAllUnchangedTabsCommand>
{
    private IEnumerable<IVsWindowFrame> _unchangedDocuments = Enumerable.Empty<IVsWindowFrame>();

    protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
    {
        await _unchangedDocuments.ToList().CloseAllAsync();
    }

    protected override void BeforeQueryStatus(EventArgs e) => BeforeQueryStatusAsync().FireAndForget();

    private async Task BeforeQueryStatusAsync()
    {
        _unchangedDocuments = Enumerable.Empty<IVsWindowFrame>();
        IEnumerable<IVsWindowFrame> frames = await WindowFrameUtilities.GetAllDocumentsInActiveWindowAsync();
        _unchangedDocuments = frames.Where(IsUnchanged);
        Command.Enabled = _unchangedDocuments.Any();
    }

    private static bool IsUnchanged(IVsWindowFrame frame)
    {
        return VsShellUtilities.GetTextView(frame)?.ToDocumentView().Document?.IsDirty == false;
    }
}