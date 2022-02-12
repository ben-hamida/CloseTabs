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
}