namespace CloseTabs;

[Command(PackageIds.CloseAllTabsOfFileType)]
internal sealed class CloseAllTabsOfFileTypeCommand : BaseCommand<CloseAllTabsOfFileTypeCommand>
{
    private string? _fileExtension;

    protected override void BeforeQueryStatus(EventArgs e)
    {
        BeforeQueryStatusAsync().FireAndForget();
    }

    protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
    {
        if (_fileExtension is not null)
        {
            await WindowFrameUtilities.CloseAllWindowFramesAsync(frame => frame.GetFileExtension() == _fileExtension);
        }
    }

    private async Task BeforeQueryStatusAsync()
    {
        _fileExtension = await WindowFrameUtilities.GetSelectedFileExtensionAsync();
        Command.Visible = _fileExtension != null;
        Command.Text = $"Close all {_fileExtension} Files";
    }
}