using EnvDTE;
using EnvDTE80;

namespace CloseTabs;

[Command(PackageIds.CloseAllTabsInProject)]
internal sealed class CloseAllTabsInProjectCommand : BaseCommand<CloseAllTabsInProjectCommand>
{
    private IVsHierarchy? _selectedProjectHierarchy;

    protected override void Execute(object sender, EventArgs e)
    {
        ThreadHelper.ThrowIfNotOnUIThread();
        IEnumerable<IVsWindowFrame> documents = WindowFrameUtilities.GetAllDocumentsInActiveWindow();
        documents
            .Where(frame => frame.TryGetProjectHierarchy() == _selectedProjectHierarchy)
            .ToList()
            .CloseAll();
    }

    protected override void BeforeQueryStatus(EventArgs e)
    {
        ThreadHelper.ThrowIfNotOnUIThread();

        var dte = VS.GetRequiredService<DTE, DTE2>();
        var windowConfiguration = dte.WindowConfigurations;
        foreach (object configuration in windowConfiguration)
        {
            
        }

        Command.Visible = false;
        Command.Enabled = false;
        _selectedProjectHierarchy = HierarchyUtilities.GetProjectHierarchyOfCurrentWindowFrame();
        if (_selectedProjectHierarchy == null)
        {
            return;
        }

        Command.Enabled = true;
        Command.Visible = true;
        Command.Text = $"Close All in {_selectedProjectHierarchy.GetCanonicalName()}";
    }
}