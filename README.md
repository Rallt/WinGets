# WinGets

WinGets is a Windows 11 study dashboard prototype built with Windows App SDK and WinUI 3.

## Functionality

- Widget-style dashboard with a daily focus module, module progress cards, upcoming events, and urgent deadline highlighting.
- Modules page for adding modules and checklist tasks.
- Events page for adding exams, presentations, and assignment deadlines.
- Settings page for pastel presets, widget density, and dashboard visibility toggles.
- Zen page with a minimal focus layout and Pomodoro-style timer state text.
- Local JSON persistence through `ApplicationData.Current.LocalFolder` so study data is recreated on first launch and saved after edits.
- Automation card that can generate a daily study plan, create daily focus tasks, and add urgent event preparation tasks.
- Expanded theme presets including Ocean Mist, Sunset Rose, and Forest Glow resources for future customization.

## Documentation

- [Architecture overview](docs/ARCHITECTURE.md)
- [Build, run, and validation guide](docs/BUILD_AND_RUN.md)

## Automated checks

The repository includes a cross-platform smoke test that validates the required WinUI project structure, parses XAML, checks the Windows App SDK project settings, verifies light/dark theme resources, and confirms the persistence/ViewModel members requested for the app.

```bash
python3 tests/validate_project.py
```

## Windows build checks

Run these on a Windows machine with the .NET 8 SDK, Windows App SDK workload support, and access to NuGet:

```powershell
dotnet restore WinGets.sln
dotnet build WinGets.sln
```

Manual Windows verification should include launching the app, switching Windows light/dark mode, adding/editing modules and events, checking urgent dates within three days, and opening Zen Mode.
