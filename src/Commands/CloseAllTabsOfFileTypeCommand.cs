namespace CloseTabs;

[Command(PackageIds.CloseAllTabsOfFileType)]
internal sealed class CloseAllTabsOfFileTypeCommand : BaseCommand<CloseAllTabsOfFileTypeCommand>
{
    private string? _fileExtension;

    protected override void Execute(object sender, EventArgs e)
    {
        ThreadHelper.ThrowIfNotOnUIThread();

        if (_fileExtension is not null)
        {
            IEnumerable<IVsWindowFrame> documents = WindowFrameUtilities.GetAllDocumentsInActiveWindow();
            documents.Where(frame => frame.GetFileExtension() == _fileExtension).CloseAll();
        }
    }

    protected override void BeforeQueryStatus(EventArgs e)
    {
        ThreadHelper.ThrowIfNotOnUIThread();
        _fileExtension = Services.VsMonitorSelection.GetSelectedFrame()?.GetFileExtension();
        Command.Enabled = _fileExtension != null;
        Command.Visible = _fileExtension != null;
        Command.Text = $"Close all {_fileExtension} Files";
    }
}