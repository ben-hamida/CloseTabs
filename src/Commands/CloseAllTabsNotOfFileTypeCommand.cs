namespace CloseTabs;

[Command(PackageIds.CloseAllTabsNotOfFileType)]
internal sealed class CloseAllTabsNotOfFileTypeCommand : BaseCommand<CloseAllTabsNotOfFileTypeCommand>
{
    private IVsWindowFrame[] _framesToClose = Array.Empty<IVsWindowFrame>();

    protected override void BeforeQueryStatus(EventArgs e)
    {
        BeforeQueryStatusAsync().FireAndForget();
    }

    protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
    {
        await _framesToClose.CloseAllAsync();
    }

    private async Task BeforeQueryStatusAsync()
    {
        _framesToClose = Array.Empty<IVsWindowFrame>();
        IReadOnlyList<IVsWindowFrame> documents = await WindowFrameUtilities.GetAllDocumentsInActiveWindowAsync();
        string? fileExtension = await WindowFrameUtilities.GetSelectedFileExtensionAsync();
        _framesToClose = documents.Where(frame => frame.GetFileExtension() != fileExtension).ToArray();
        Command.Visible = fileExtension != null;
        Command.Enabled = _framesToClose.Any();
        Command.Text = $"Close all non-{fileExtension} Files";
    }
}