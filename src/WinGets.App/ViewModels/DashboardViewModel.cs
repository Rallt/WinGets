using System.Collections.ObjectModel;
using System.Windows.Input;
using WinGets.App.Models;
using WinGets.App.Services;

namespace WinGets.App.ViewModels;

public sealed class DashboardViewModel : ObservableObject
{
    private readonly StudyDataService _studyDataService;
    private StudyData _data = new();
    private string _newModuleName = string.Empty;
    private string _newModuleAccent = "#8BB8FF";
    private string _newTaskTitle = string.Empty;
    private StudyModule? _selectedModule;
    private string _newEventTitle = string.Empty;
    private string _newEventType = "Deadline";
    private DateTimeOffset _newEventDate = DateTimeOffset.Now.AddDays(1);
    private string _zenTimerState = "25:00 focus";

    public DashboardViewModel(StudyDataService? studyDataService = null)
    {
        _studyDataService = studyDataService ?? new StudyDataService();

        // Command wiring is kept in the ViewModel so pages can stay mostly declarative XAML.
        AddModuleCommand = new RelayCommand(async () => await AddModuleAsync());
        AddTaskCommand = new RelayCommand(async parameter => await AddTaskAsync(parameter as StudyModule));
        ToggleTaskCommand = new RelayCommand(async _ => await SaveAndRefreshAsync());
        AddEventCommand = new RelayCommand(async () => await AddEventAsync());
        SaveCommand = new RelayCommand(async () => await SaveAndRefreshAsync());
        EnterZenModeCommand = new RelayCommand(() => ZenTimerState = "25:00 focus · breathe in, begin softly");
    }

    public StudyData Data
    {
        get => _data;
        private set
        {
            if (SetProperty(ref _data, value))
            {
                SelectedModule = _data.Modules.FirstOrDefault();
                RefreshComputedProperties();
            }
        }
    }

    public ObservableCollection<StudyModule> Modules => Data.Modules;

    public ObservableCollection<StudyEvent> Events => Data.Events;

    public AppSettings Settings => Data.Settings;

    public IEnumerable<StudyEvent> UpcomingEvents => Events.OrderBy(studyEvent => studyEvent.Date).Take(8);

    public IEnumerable<StudyEvent> UrgentEvents => UpcomingEvents.Where(studyEvent => studyEvent.IsUrgent);

    public StudyModule? TodayFocusModule => Modules.OrderBy(module => module.EffectiveProgress).FirstOrDefault();

    public string NewModuleName
    {
        get => _newModuleName;
        set => SetProperty(ref _newModuleName, value);
    }

    public string NewModuleAccent
    {
        get => _newModuleAccent;
        set => SetProperty(ref _newModuleAccent, value);
    }

    public string NewTaskTitle
    {
        get => _newTaskTitle;
        set => SetProperty(ref _newTaskTitle, value);
    }

    public StudyModule? SelectedModule
    {
        get => _selectedModule;
        set => SetProperty(ref _selectedModule, value);
    }

    public string NewEventTitle
    {
        get => _newEventTitle;
        set => SetProperty(ref _newEventTitle, value);
    }

    public string NewEventType
    {
        get => _newEventType;
        set => SetProperty(ref _newEventType, value);
    }

    public DateTimeOffset NewEventDate
    {
        get => _newEventDate;
        set => SetProperty(ref _newEventDate, value);
    }

    public string ZenTimerState
    {
        get => _zenTimerState;
        set => SetProperty(ref _zenTimerState, value);
    }

    public ICommand AddModuleCommand { get; }

    public ICommand AddTaskCommand { get; }

    public ICommand ToggleTaskCommand { get; }

    public ICommand AddEventCommand { get; }

    public ICommand SaveCommand { get; }

    public ICommand EnterZenModeCommand { get; }

    public async Task InitializeAsync()
    {
        Data = await _studyDataService.LoadAsync();
    }

    private async Task AddModuleAsync()
    {
        string name = string.IsNullOrWhiteSpace(NewModuleName) ? $"New module {Modules.Count + 1}" : NewModuleName.Trim();
        var module = new StudyModule
        {
            Name = name,
            AccentColor = string.IsNullOrWhiteSpace(NewModuleAccent) ? "#8BB8FF" : NewModuleAccent.Trim(),
            Tasks = { new StudyTask { Title = "First calm study step" } }
        };

        Modules.Add(module);
        SelectedModule = module;
        NewModuleName = string.Empty;
        await SaveAndRefreshAsync();
    }

    private async Task AddTaskAsync(StudyModule? module)
    {
        StudyModule? target = module ?? SelectedModule;
        if (target is null)
        {
            return;
        }

        string title = string.IsNullOrWhiteSpace(NewTaskTitle) ? "New study task" : NewTaskTitle.Trim();
        target.Tasks.Add(new StudyTask { Title = title });
        NewTaskTitle = string.Empty;
        await SaveAndRefreshAsync();
    }

    private async Task AddEventAsync()
    {
        StudyModule? module = SelectedModule ?? TodayFocusModule;
        Events.Add(new StudyEvent
        {
            Title = string.IsNullOrWhiteSpace(NewEventTitle) ? "New study event" : NewEventTitle.Trim(),
            ModuleId = module?.Id,
            EventType = string.IsNullOrWhiteSpace(NewEventType) ? "Deadline" : NewEventType.Trim(),
            Date = NewEventDate
        });

        NewEventTitle = string.Empty;
        NewEventDate = DateTimeOffset.Now.AddDays(1);
        await SaveAndRefreshAsync();
    }

    private async Task SaveAndRefreshAsync()
    {
        await _studyDataService.SaveAsync(Data);
        RefreshComputedProperties();
    }

    public string GetModuleName(string? moduleId)
    {
        return Modules.FirstOrDefault(module => module.Id == moduleId)?.Name ?? "General study";
    }

    public void RefreshComputedProperties()
    {
        OnPropertyChanged(nameof(Modules));
        OnPropertyChanged(nameof(Events));
        OnPropertyChanged(nameof(Settings));
        OnPropertyChanged(nameof(UpcomingEvents));
        OnPropertyChanged(nameof(UrgentEvents));
        OnPropertyChanged(nameof(TodayFocusModule));
    }
}
