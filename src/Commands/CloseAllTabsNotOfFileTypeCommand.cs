namespace CloseTabs;

[Command(PackageIds.CloseAllTabsNotOfFileType)]
internal sealed class CloseAllTabsNotOfFileTypeCommand : BaseCommand<CloseAllTabsNotOfFileTypeCommand>
{
    private IEnumerable<IVsWindowFrame> _framesToClose = Enumerable.Empty<IVsWindowFrame>();

    protected override void Execute(object sender, EventArgs e)
    {
        ThreadHelper.ThrowIfNotOnUIThread();
        _framesToClose.ToList().CloseAll();
    }

    protected override void BeforeQueryStatus(EventArgs e)
    {
        ThreadHelper.ThrowIfNotOnUIThread();
        Command.Visible = false;
        Command.Enabled = false;
        _framesToClose = Enumerable.Empty<IVsWindowFrame>();
        string? fileExtension = Services.VsMonitorSelection.GetSelectedFrame()?.GetFileExtension();
        if (fileExtension is null)
        {
            return;
        }

        IEnumerable<IVsWindowFrame> documents = WindowFrameUtilities.GetAllDocumentsInActiveWindow();
        _framesToClose = documents.Where(frame => frame.GetFileExtension() != fileExtension);
        Command.Visible = true;
        Command.Enabled = _framesToClose.Any();
        Command.Text = $"Close all non-{fileExtension} Files";
    }
}