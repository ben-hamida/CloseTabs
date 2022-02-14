﻿namespace CloseTabs;

[Command(PackageIds.CloseAllUnchangedTabs)]
internal sealed class CloseAllUnchangedTabsCommand : BaseCommand<CloseAllUnchangedTabsCommand>
{
    private IEnumerable<IVsWindowFrame> _unchangedDocuments = Enumerable.Empty<IVsWindowFrame>();

    protected override Task ExecuteAsync(OleMenuCmdEventArgs e)
    {
        _unchangedDocuments.ToList().CloseAll();
        return Task.CompletedTask;
    }

    protected override void BeforeQueryStatus(EventArgs e)
    {
        ThreadHelper.ThrowIfNotOnUIThread();
        _unchangedDocuments = Enumerable.Empty<IVsWindowFrame>();
        IEnumerable<IVsWindowFrame> frames = WindowFrameUtilities.GetAllDocumentsInActiveWindow();
        _unchangedDocuments = frames.Where(IsUnchanged);
        Command.Enabled = _unchangedDocuments.Any();
    }

    private static bool IsUnchanged(IVsWindowFrame frame)
    {
        return VsShellUtilities.GetTextView(frame)?.ToDocumentView().Document?.IsDirty == false;
    }
}