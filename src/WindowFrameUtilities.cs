namespace CloseTabs;

internal static class WindowFrameUtilities
{
    public static async Task<IReadOnlyList<IVsWindowFrame>> GetAllDocumentsInActiveWindowAsync()
    {
        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

        IVsWindowFrame? selection = await GetSelectedFrameAsync();
        // ReSharper disable once SuspiciousTypeConversion.Global
        if (selection is not IVsWindowFrame6 selectedFrame)
        {
            return new List<IVsWindowFrame>();
        }

        IVsUIShell uiShell = await VS.Services.GetUIShellAsync();
        ErrorHandler.ThrowOnFailure(uiShell.GetDocumentWindowEnum(out IEnumWindowFrames docEnum));
        var temp = new IVsWindowFrame[1];
        var frames = new List<IVsWindowFrame>();
        while (docEnum.Next(1, temp, out uint fetched) == VSConstants.S_OK && fetched == 1)
        {
            IVsWindowFrame frame = temp[0];
            if (selectedFrame.IsInSameTabGroup(frame))
            {
                frames.Add(frame);
            }
        }

        return frames;
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
        IVsWindowFrame? selectedFrame = await GetSelectedFrameAsync();
        return selectedFrame?.GetFileExtension();
    }

    public static async Task CloseALlTabsAsync(Func<IVsWindowFrame, bool> selector)
    {
        IReadOnlyList<IVsWindowFrame> documents = await GetAllDocumentsInActiveWindowAsync();
        documents.Where(selector).CloseAll();
    }
}