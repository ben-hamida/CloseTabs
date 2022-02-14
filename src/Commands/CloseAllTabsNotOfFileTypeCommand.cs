namespace CloseTabs;

[Command(PackageIds.CloseAllTabsNotOfFileType)]
internal sealed class CloseAllTabsNotOfFileTypeCommand : BaseCommand<CloseAllTabsNotOfFileTypeCommand>
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
        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
        string? fileExtension = await WindowFrameUtilities.GetSelectedFileExtensionAsync();
        if (fileExtension is null)
        {
            Command.Visible = false;
            return;
        }

        IEnumerable<IVsWindowFrame> documents = await WindowFrameUtilities.GetAllDocumentsInActiveWindowAsync();
        _framesToClose = documents.Where(frame => frame.GetFileExtension() != fileExtension);
        Command.Visible = true;
        Command.Enabled = _framesToClose.Any();
        Command.Text = $"Close all non-{fileExtension} Files";
    }
}