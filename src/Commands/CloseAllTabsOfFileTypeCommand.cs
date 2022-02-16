namespace CloseTabs;

[Command(PackageIds.CloseAllTabsOfFileType)]
internal sealed class CloseAllTabsOfFileTypeCommand : BaseCommand<CloseAllTabsOfFileTypeCommand>
{
    private string? _fileExtension;

    protected override Task ExecuteAsync(OleMenuCmdEventArgs e)
    {
        if (_fileExtension is not null)
        {
            IEnumerable<IVsWindowFrame> documents = WindowFrameUtilities.GetAllDocumentsInActiveWindow();
            documents.Where(frame => frame.GetFileExtension() == _fileExtension).CloseAll();
        }

        return Task.CompletedTask;
    }

    protected override void BeforeQueryStatus(EventArgs e)
    {
        _fileExtension = Services.VsMonitorSelection.GetSelectedFrame()?.GetFileExtension();
        Command.Enabled = _fileExtension != null;
        Command.Visible = _fileExtension != null;
        Command.Text = $"Close all {_fileExtension} Files";
    }
}