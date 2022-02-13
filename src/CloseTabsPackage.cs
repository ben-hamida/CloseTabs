namespace CloseTabs;

[PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
[InstalledProductRegistration(Vsix.Name, Vsix.Description, Vsix.Version)]
[ProvideMenuResource("Menus.ctmenu", 1)]
[Guid(PackageGuids.CloseTabsString)]
[ProvideAutoLoad(VSConstants.UICONTEXT.DocumentWindowActive_string, PackageAutoLoadFlags.BackgroundLoad)]
public sealed class CloseTabsPackage : ToolkitPackage
{
    protected override async Task InitializeAsync(
        CancellationToken cancellationToken,
        IProgress<ServiceProgressData> progress)
    {
        await this.RegisterCommandsAsync();
    }
}