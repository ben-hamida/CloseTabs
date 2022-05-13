namespace CloseTabs;

internal static class WindowFrameUtilities
{
    public static IEnumerable<IVsWindowFrame> GetAllDocumentsInActiveWindow()
    {
        ThreadHelper.ThrowIfNotOnUIThread();
        IVsWindowFrame? selectedFrame = Services.VsMonitorSelection.GetSelectedFrame();
        return selectedFrame is null
            ? Enumerable.Empty<IVsWindowFrame>()
            : AllDocumentsInSameTabGroup(selectedFrame);
    }

    public static IVsWindowFrame? GetSelectedFrame(this IVsMonitorSelection monitorSelection)
    {
        ThreadHelper.ThrowIfNotOnUIThread();
        monitorSelection.GetCurrentElementValue((uint)VSConstants.VSSELELEMID.SEID_WindowFrame, out object selection);
        return selection as IVsWindowFrame;
    }

    public static IEnumerable<IVsWindowFrame> GetOrderedFramesOfActiveWindow()
    {
        Window? activeWindow = Application.Current.Windows.OfType<Window>().FirstOrDefault(x => x.IsActive);
        if (activeWindow == null)
        {
            return Enumerable.Empty<IVsWindowFrame>();
        }

        Services.VsTextManager.GetActiveView(1, null, out IVsTextView activeView);
        var wpfTextView = activeView.ToIWpfTextView();
        if (wpfTextView is null)
        {
            return Enumerable.Empty<IVsWindowFrame>();
        }

        object control = ((DependencyObject)wpfTextView.VisualElement).FindAncestor(
            visual => visual.GetVisualOrLogicalParent(),
            x => x.GetType().Name.Contains("DocumentGroupControl"));

        return control is ItemsControl itemControl
            ? itemControl.Items.Cast<dynamic>().Select(d => d.WindowFrame).Cast<IVsWindowFrame>()
            : Enumerable.Empty<IVsWindowFrame>();
    }

    private static IEnumerable<IVsWindowFrame> AllDocumentsInSameTabGroup(IVsWindowFrame selectedFrame)
    {
        ThreadHelper.ThrowIfNotOnUIThread();
        DTEWindow? window = selectedFrame.GetParentWindow();
        return window is null
            ? Enumerable.Empty<IVsWindowFrame>()
            : AllDocumentsInSameTabGroup(selectedFrame, frame => frame.GetParentWindow() == window);
    }

    private static IEnumerable<IVsWindowFrame> AllDocumentsInSameTabGroup(IVsWindowFrame selectedFrame, Func<IVsWindowFrame, bool> includeFrame)
    {
        ThreadHelper.ThrowIfNotOnUIThread();

        ErrorHandler.ThrowOnFailure(Services.VsUIShell.GetDocumentWindowEnum(out IEnumWindowFrames docEnum));
        var temp = new IVsWindowFrame[1];
        while (docEnum.Next(1, temp, out uint fetched) == VSConstants.S_OK && fetched == 1)
        {
            IVsWindowFrame frame = temp[0];
            if (frame != selectedFrame && includeFrame(frame))
            {
                yield return frame;
            }
        }

        // Always return the selected frame last. When closing the window frames it is very costly for performance
        // to have to switch selection on a close. Therefor close all non-selected frames first.
        // ReSharper disable once SuspiciousTypeConversion.Global
        yield return selectedFrame;
    }
}