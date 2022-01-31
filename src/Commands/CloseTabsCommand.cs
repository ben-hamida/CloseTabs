namespace CloseTabs;

internal abstract class CloseTabsCommand<TCommand> : BaseCommand<TCommand>
    where TCommand : class, new()
{
    private IVsWindowFrame[] _framesToClose = Array.Empty<IVsWindowFrame>();
    private IVsMonitorSelection? _monitorSelection;

    protected override async Task InitializeCompletedAsync()
    {
        _monitorSelection = await VS.Services.GetMonitorSelectionAsync();
    }

    protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
    {
        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
        foreach (IVsWindowFrame window in _framesToClose)
        {
            window.CloseFrame((uint)__FRAMECLOSE.FRAMECLOSE_PromptSave);
        }
    }

    protected override void BeforeQueryStatus(EventArgs e)
    {
        ThreadHelper.ThrowIfNotOnUIThread();
        IVsWindowFrame? selectedFrame = GetSelectedFrame();
        if (selectedFrame == null)
        {
            Command.Enabled = false;
            return;
        }

        BeforeQueryStatus(selectedFrame);
        _framesToClose = GetFramesToClose(GetOrderedFramesOfActiveWindow(), selectedFrame).ToArray();
        Command.Enabled = _framesToClose.Any();
    }

    protected virtual void BeforeQueryStatus(IVsWindowFrame selectedFrame)
    {
    }

    protected abstract IEnumerable<IVsWindowFrame> GetFramesToClose(
        IEnumerable<IVsWindowFrame> orderedFrames,
        IVsWindowFrame selectedFrame);

    private static IEnumerable<IVsWindowFrame> GetOrderedFramesOfActiveWindow()
    {
        Window? activeWindow = Application.Current.Windows.OfType<Window>().FirstOrDefault(x => x.IsActive);
        if (activeWindow == null)
        {
            return Enumerable.Empty<IVsWindowFrame>();
        }

        IInputElement focusedElement = FocusManager.GetFocusedElement(activeWindow);
        if (focusedElement == null)
        {
            return Enumerable.Empty<IVsWindowFrame>();
        }

        object control = ((DependencyObject)focusedElement).FindAncestor(
            visual => visual.GetVisualOrLogicalParent(),
            x => x.GetType().Name.Contains("DocumentGroupControl"));

        return control is ItemsControl itemControl
            ? itemControl.Items.Cast<dynamic>().Select(d => d.WindowFrame).Cast<IVsWindowFrame>()
            : Enumerable.Empty<IVsWindowFrame>();
    }

    private IVsWindowFrame? GetSelectedFrame()
    {
        ThreadHelper.ThrowIfNotOnUIThread();
        _monitorSelection!.GetCurrentElementValue((uint)VSConstants.VSSELELEMID.SEID_WindowFrame, out object selection);
        return selection as IVsWindowFrame;
    }
}