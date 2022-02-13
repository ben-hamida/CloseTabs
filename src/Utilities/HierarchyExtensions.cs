namespace CloseTabs;

internal static class HierarchyExtensions
{
    public static string? GetCanonicalName(this IVsHierarchy hierarchy)
    {
        ThreadHelper.ThrowIfNotOnUIThread();
        return hierarchy.ToHierarchyItem((uint)VSConstants.VSITEMID.Root).CanonicalName;
    }
}