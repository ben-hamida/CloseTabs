namespace CloseTabs;

internal static class WindowFrameExtensions
{
    public static async Task CloseAllAsync(this IEnumerable<IVsWindowFrame> frames)
    {
        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

        foreach (IVsWindowFrame frame in frames)
        {
            frame.CloseFrame((uint)__FRAMECLOSE.FRAMECLOSE_PromptSave);
        }
    }

    public static string? GetFileExtension(this IVsWindowFrame frame)
    {
        ThreadHelper.ThrowIfNotOnUIThread();
        frame.GetProperty((int)__VSFPROPID.VSFPROPID_pszMkDocument, out object value);
        return Path.GetExtension(value as string);
    }

    /// <summary>
    /// Gets the IVsHierarchy that represents the project for the specified IVsWindowFrame.
    /// </summary>
    public static IVsHierarchy? TryGetProjectHierarchy(this IVsWindowFrame frame)
    {
        ThreadHelper.ThrowIfNotOnUIThread();

        // Apparently getting the VSFPROPID_Hierarchy property gets the hierarchy for the project and not the document itself
        frame.GetProperty((int)__VSFPROPID.VSFPROPID_Hierarchy, out object obj);
        return obj as IVsHierarchy;
    }
}