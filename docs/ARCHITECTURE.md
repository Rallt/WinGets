# WinGets architecture

WinGets is a lightweight Windows 11 study dashboard prototype implemented as an unpackaged Windows App SDK / WinUI 3 desktop application.

## Project layout

```text
WinGets/
├── WinGets.sln
├── src/
│   └── WinGets.App/
│       ├── Models/       # Serializable study data contracts
│       ├── Services/     # Persistence and theme helpers
│       ├── ViewModels/   # MVVM state and command wiring
│       ├── Views/        # Navigation pages
│       ├── Controls/     # Reusable cards
│       └── Styles/       # Theme, typography, and control resources
└── tests/                # Cross-platform repository smoke tests
```

## Application flow

1. `App.OnLaunched` creates the shared `DashboardViewModel`, loads persisted study data through `StudyDataService`, and opens `MainWindow`.
2. `MainWindow` hosts a `NavigationView` with Dashboard, Modules, Events, Zen, and Settings pages.
3. Each page binds to `App.Current.ViewModel`, keeping the prototype intentionally small and easy to follow.
4. Commands in `DashboardViewModel` mutate the in-memory `StudyData`, call `StudyDataService.SaveAsync`, and raise property change notifications for computed dashboard sections.

## Data model

- `StudyData` is the JSON root object and contains modules, events, and settings.
- `StudyModule` stores module metadata and checklist tasks. `EffectiveProgress` computes progress from completed tasks when tasks exist.
- `StudyEvent` stores exam/deadline metadata. `IsUrgent` is true when the event date is today through three days from today.
- `AppSettings` stores dashboard toggles, the pastel preset name, widget layout size, and Zen animation preference.

The models are deliberately simple POCO-style classes so `System.Text.Json` can serialize them without extra dependencies.

## Persistence

`StudyDataService` reads and writes `study-data.json` under `ApplicationData.Current.LocalFolder` using async WinRT file APIs. On first launch, the service creates default modules, tasks, and upcoming events, then saves them immediately so subsequent launches use the persisted file.

## UI and theming

`App.xaml` merges three style dictionaries:

- `Styles/Colors.xaml` contains light and dark theme dictionaries with soft pastel resources.
- `Styles/Typography.xaml` centralizes Segoe UI Variable title, subtitle, body, and caption text styles.
- `Styles/Controls.xaml` defines rounded, touch-friendly defaults for buttons, inputs, progress bars, and cards.

`RequestedTheme="Default"` lets WinUI follow the Windows system light/dark mode. `ThemeService` is a small extension point for future per-window or per-preset theme behavior.

## Validation strategy

The Windows App SDK project should be restored and built on Windows. Because Linux containers cannot launch WinUI desktop apps, `tests/validate_project.py` provides a cross-platform smoke test that verifies repository structure, XAML well-formedness, core project settings, theme resources, persistence hooks, and ViewModel/navigation members.
