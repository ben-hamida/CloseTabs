namespace CloseTabs;

internal static class WindowFrameExtensions
{
    public static void CloseAll(this IEnumerable<IVsWindowFrame> frames)
    {
        ThreadHelper.ThrowIfNotOnUIThread();
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

    public static bool IsDocument(this IVsWindowFrame frame)
    {
        ThreadHelper.ThrowIfNotOnUIThread();
        frame.GetProperty((int)__VSFPROPID.VSFPROPID_Type, out object value);
        int? frameType = value as int?;
        return frameType ==  1;
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