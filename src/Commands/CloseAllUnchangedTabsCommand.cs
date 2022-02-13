namespace CloseTabs;

[Command(PackageIds.CloseAllUnchangedTabs)]
internal sealed class CloseAllUnchangedTabsCommand : BaseCommand<CloseAllUnchangedTabsCommand>
{
    private IEnumerable<IVsWindowFrame> _unchangedDocuments = Enumerable.Empty<IVsWindowFrame>();

    protected override void BeforeQueryStatus(EventArgs e)
    {
        BeforeQueryStatusAsync().FireAndForget();
    }

    protected override Task ExecuteAsync(OleMenuCmdEventArgs e)
    {
        _unchangedDocuments.CloseAll();
        return Task.CompletedTask;
    }

    private async Task BeforeQueryStatusAsync()
    {
        IReadOnlyList<IVsWindowFrame> frames = await WindowFrameUtilities.GetAllDocumentsInActiveWindowAsync();
        _unchangedDocuments = frames.Where(IsUnchanged);
        Command.Enabled = _unchangedDocuments.Any();
    }

    private static bool IsUnchanged(IVsWindowFrame frame)
    {
        return VsShellUtilities.GetTextView(frame)?.ToDocumentView().Document?.IsDirty == false;
    }
}