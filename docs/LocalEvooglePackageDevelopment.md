# Local Evoogle Package Development

ApiFramework consumes locally built `Evoogle.Core` and `Evoogle.XUnit` packages by default.
Both packages are restored from the user-level `EvoogleLocal` NuGet source at
`%LOCALAPPDATA%\Evoogle\NuGetFeed`; no package is uploaded to a cloud registry.

## Package Mode

Run `Pack-EvoogleCore.ps1` from the sibling `evoogle-core` repository. The script tests Core,
creates matching timestamped Core and XUnit packages, and writes their exact versions to the
ignored `Directory.Build.evoogle-core.local.props` file in this repository.

Use package mode for normal ApiFramework development. Each new Core package run updates the local
pin, so the next restore uses the matching Core and XUnit package versions. The local NuGet source
must be registered before restoring packages:

```powershell
$feedPath = Join-Path $env:LOCALAPPDATA 'Evoogle\NuGetFeed'
dotnet nuget add source $feedPath --name EvoogleLocal
```

## Source Mode

When changing ApiFramework together with Core or XUnit source, copy
`Directory.Build.local.props.example` to the ignored `Directory.Build.local.props`. Its combined
switch replaces the Core production package reference and both XUnit test package references with
their sibling project references:

```powershell
Copy-Item Directory.Build.local.props.example Directory.Build.local.props
```

Remove the ignored local file or set its property to `false` to return to package mode.

## Debugging

Both local packages include portable PDBs. Visual Studio can open the matching Core or XUnit
source checkout while stepping through packaged code. Rebuild the packages after changing source;
if Visual Studio skips a library frame, confirm symbols loaded in the Modules window and disable
Just My Code for that debugging session.
