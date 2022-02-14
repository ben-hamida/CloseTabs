namespace CloseTabs;

internal static class WindowFrameUtilities
{
    public static async Task<IEnumerable<IVsWindowFrame>> GetAllDocumentsInActiveWindowAsync()
    {
        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

        IVsWindowFrame? selection = await GetSelectedFrameAsync();
        // ReSharper disable once SuspiciousTypeConversion.Global
        if (selection is not IVsWindowFrame6 selectedFrame)
        {
            return new List<IVsWindowFrame>();
        }

        IVsUIShell uiShell = await VS.Services.GetUIShellAsync();
        return AllDocumentsInActiveWindow();

        IEnumerable<IVsWindowFrame> AllDocumentsInActiveWindow()
        {
            ErrorHandler.ThrowOnFailure(uiShell.GetDocumentWindowEnum(out IEnumWindowFrames docEnum));
            var temp = new IVsWindowFrame[1];
            while (docEnum.Next(1, temp, out uint fetched) == VSConstants.S_OK && fetched == 1)
            {
                IVsWindowFrame frame = temp[0];
                if (frame != selection && selectedFrame.IsInSameTabGroup(frame))
                {
                    yield return frame;
                }
            }

            // Always return the selected frame last. When closing the window frames it is very costly for performance
            // to have to switch selection on a close. Therefor close all non-selected frames first.
            // ReSharper disable once SuspiciousTypeConversion.Global
            yield return (IVsWindowFrame)selectedFrame;
        }
    }

    public static async Task<IVsWindowFrame?> GetSelectedFrameAsync()
    {
        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
        IVsMonitorSelection monitorSelection = await VS.Services.GetMonitorSelectionAsync();
        monitorSelection.GetCurrentElementValue((uint)VSConstants.VSSELELEMID.SEID_WindowFrame, out object selection);
        return selection as IVsWindowFrame;
    }

    public static async Task<string?> GetSelectedFileExtensionAsync()
    {
        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
        IVsWindowFrame? selectedFrame = await GetSelectedFrameAsync();
        return selectedFrame?.GetFileExtension();
    }

    public static IEnumerable<IVsWindowFrame> GetOrderedFramesOfActiveWindow()
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
}