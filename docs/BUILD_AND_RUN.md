# Build, run, and validation guide

## Prerequisites

Use a Windows 11 development machine with:

- Visual Studio 2022 or newer with **.NET desktop development** enabled.
- .NET 8 SDK.
- Windows App SDK support.
- Network access to NuGet for `Microsoft.WindowsAppSDK` and `Microsoft.Windows.SDK.BuildTools` restore.

The app targets `net8.0-windows10.0.19041.0` and is configured as an unpackaged WinUI 3 desktop app.

## Restore and compile

From the repository root:

```powershell
dotnet restore WinGets.sln
dotnet build WinGets.sln
```

For a release build:

```powershell
dotnet build WinGets.sln -c Release
```

## Run locally

From Visual Studio:

1. Open `WinGets.sln`.
2. Set `WinGets.App` as the startup project.
3. Start debugging or run without debugging.

From the command line on Windows, after restore:

```powershell
dotnet run --project src/WinGets.App/WinGets.App.csproj
```

## Manual verification checklist

After launching the app on Windows, verify:

- Dashboard opens with the greeting card, daily focus card, module progress cards, upcoming timeline, and urgent card area.
- Switching Windows between light and dark mode updates the pastel theme resources automatically.
- Adding a module on the Modules page creates a card and persists after restart.
- Adding/checking tasks updates module checklist counts and computed progress.
- Adding an event on the Events page displays it in the sorted upcoming timeline.
- Events dated today through three days ahead show the urgent styling and label.
- Settings toggles and preset/layout selections save to the local JSON file.
- Zen Mode shows the current focus module/task and updates the timer state when starting focus.

## Cross-platform smoke test

When a Windows build environment is not available, run the repository smoke test:

```bash
python3 tests/validate_project.py
```

This does not replace a Windows compile. It verifies project structure, XAML XML well-formedness, package/project settings, theme dictionaries, persistence members, and ViewModel/navigation surface area.

## Persistence file

Runtime data is saved as `study-data.json` in `ApplicationData.Current.LocalFolder`. The exact folder is assigned by Windows for the app identity/runtime context. Delete that file to force first-run default data creation again.

## Troubleshooting

- If restore fails, confirm NuGet access and that the Windows App SDK package can be downloaded.
- If build fails with Windows targeting errors, confirm the .NET 8 SDK and Windows SDK targeting packs are installed.
- If the app cannot launch, confirm it is being run on Windows; WinUI 3 desktop apps cannot launch in Linux containers.
