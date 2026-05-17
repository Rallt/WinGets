#!/usr/bin/env python3
"""Repository smoke tests for the WinGets WinUI project.

These checks intentionally avoid compiling the Windows-only WinUI target so they can
run in Linux CI/agent containers while still catching broken structure, malformed
XAML, and missing implementation requirements before a Windows build is run.
"""

from __future__ import annotations

import re
import sys
import xml.etree.ElementTree as ET
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
APP = ROOT / "src" / "WinGets.App"

REQUIRED_FILES = [
    "WinGets.sln",
    "docs/ARCHITECTURE.md",
    "docs/BUILD_AND_RUN.md",
    "src/WinGets.App/WinGets.App.csproj",
    "src/WinGets.App/app.manifest",
    "src/WinGets.App/App.xaml",
    "src/WinGets.App/App.xaml.cs",
    "src/WinGets.App/MainWindow.xaml",
    "src/WinGets.App/MainWindow.xaml.cs",
    "src/WinGets.App/Models/StudyModule.cs",
    "src/WinGets.App/Models/StudyTask.cs",
    "src/WinGets.App/Models/StudyEvent.cs",
    "src/WinGets.App/Models/AppSettings.cs",
    "src/WinGets.App/Models/StudyData.cs",
    "src/WinGets.App/Services/StudyDataService.cs",
    "src/WinGets.App/Services/ThemeService.cs",
    "src/WinGets.App/ViewModels/ObservableObject.cs",
    "src/WinGets.App/ViewModels/RelayCommand.cs",
    "src/WinGets.App/ViewModels/DashboardViewModel.cs",
    "src/WinGets.App/Views/DashboardPage.xaml",
    "src/WinGets.App/Views/DashboardPage.xaml.cs",
    "src/WinGets.App/Views/ModulesPage.xaml",
    "src/WinGets.App/Views/ModulesPage.xaml.cs",
    "src/WinGets.App/Views/EventsPage.xaml",
    "src/WinGets.App/Views/EventsPage.xaml.cs",
    "src/WinGets.App/Views/SettingsPage.xaml",
    "src/WinGets.App/Views/SettingsPage.xaml.cs",
    "src/WinGets.App/Views/ZenPage.xaml",
    "src/WinGets.App/Views/ZenPage.xaml.cs",
    "src/WinGets.App/Controls/ModuleCard.xaml",
    "src/WinGets.App/Controls/ModuleCard.xaml.cs",
    "src/WinGets.App/Controls/EventCard.xaml",
    "src/WinGets.App/Controls/EventCard.xaml.cs",
    "src/WinGets.App/Styles/Colors.xaml",
    "src/WinGets.App/Styles/Typography.xaml",
    "src/WinGets.App/Styles/Controls.xaml",
]

REQUIRED_BRUSHES = [
    "PastelBlueBrush",
    "PastelLavenderBrush",
    "PastelMintBrush",
    "PastelPeachBrush",
    "CardBackgroundBrush",
    "CardStrokeBrush",
    "UrgentBrush",
]


def read(relative_path: str) -> str:
    return (ROOT / relative_path).read_text(encoding="utf-8")


def assert_contains(text: str, needle: str, label: str) -> None:
    if needle not in text:
        raise AssertionError(f"Missing {label}: {needle}")


def test_required_file_inventory() -> None:
    missing = [path for path in REQUIRED_FILES if not (ROOT / path).is_file()]
    if missing:
        raise AssertionError("Missing required files: " + ", ".join(missing))


def test_xaml_is_well_formed() -> None:
    for path in APP.rglob("*.xaml"):
        ET.parse(path)


def test_project_targets_winui_windows_app_sdk() -> None:
    project = read("src/WinGets.App/WinGets.App.csproj")
    for needle in [
        "<TargetFramework>net8.0-windows10.0.19041.0</TargetFramework>",
        "<UseWinUI>true</UseWinUI>",
        "Microsoft.WindowsAppSDK",
        "Microsoft.Windows.SDK.BuildTools",
        "<EnableWindowsTargeting>true</EnableWindowsTargeting>",
    ]:
        assert_contains(project, needle, "project setting")


def test_theme_resources_and_app_theme() -> None:
    colors = read("src/WinGets.App/Styles/Colors.xaml")
    assert_contains(colors, 'x:Key="Light"', "light theme dictionary")
    assert_contains(colors, 'x:Key="Dark"', "dark theme dictionary")
    for brush in REQUIRED_BRUSHES:
        if len(re.findall(fr'x:Key="{brush}"', colors)) != 2:
            raise AssertionError(f"Expected light and dark definitions for {brush}")

    app = read("src/WinGets.App/App.xaml")
    assert_contains(app, 'RequestedTheme="Default"', "system theme request")
    for dictionary in ["Styles/Colors.xaml", "Styles/Typography.xaml", "Styles/Controls.xaml"]:
        assert_contains(app, dictionary, "merged style dictionary")


def test_persistence_and_default_data_requirements() -> None:
    service = read("src/WinGets.App/Services/StudyDataService.cs")
    for needle in [
        "ApplicationData.Current.LocalFolder",
        "JsonSerializerOptions",
        "WriteIndented = true",
        "LoadAsync()",
        "SaveAsync(StudyData data)",
        "CreateDefaultData()",
        "FileIO.ReadTextAsync",
        "FileIO.WriteTextAsync",
    ]:
        assert_contains(service, needle, "persistence implementation")


def test_view_model_exposes_required_commands_and_views() -> None:
    view_model = read("src/WinGets.App/ViewModels/DashboardViewModel.cs")
    for needle in [
        "UpcomingEvents",
        "UrgentEvents",
        "TodayFocusModule",
        "AddModuleCommand",
        "AddEventCommand",
        "SaveCommand",
        "EnterZenModeCommand",
    ]:
        assert_contains(view_model, needle, "dashboard view model member")

    shell = read("src/WinGets.App/MainWindow.xaml")
    for page in ["Dashboard", "Modules", "Events", "Zen", "Settings"]:
        assert_contains(shell, f'Tag="{page}"', "navigation item")


def main() -> int:
    tests = [value for name, value in globals().items() if name.startswith("test_")]
    for test in tests:
        test()
        print(f"PASS {test.__name__}")
    print(f"PASS {len(tests)} smoke tests")
    return 0


if __name__ == "__main__":
    try:
        raise SystemExit(main())
    except Exception as exc:  # noqa: BLE001 - make command-line failures concise.
        print(f"FAIL {exc}", file=sys.stderr)
        raise SystemExit(1)
