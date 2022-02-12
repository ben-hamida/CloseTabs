﻿namespace CloseTabs;

[Command(PackageIds.CloseAllTabsExceptOfFileType)]
internal sealed class CloseAllTabsExceptOfFileTypeCommand : BaseCommand<CloseAllTabsExceptOfFileTypeCommand>
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
            await WindowFrameUtilities.CloseALlTabsAsync(frame => frame.GetFileExtension() != _fileExtension);
        }
    }

    private async Task BeforeQueryStatusAsync()
    {
        _fileExtension = await WindowFrameUtilities.GetSelectedFileExtensionAsync();
        Command.Visible = _fileExtension != null;
        Command.Text = $"Close all Non-{_fileExtension} Files";
    }
}