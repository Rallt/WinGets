using WinGets.App.Models;

namespace WinGets.App.Services;

public sealed class StudyAutomationService
{
    private const string AutomationTag = "#auto-plan";

    public bool ApplyDailyAutomation(StudyData data)
    {
        if (!data.Settings.AutoCreateDailyPlan || data.Modules.Count == 0)
        {
            return false;
        }

        bool changed = false;
        changed |= EnsureDailyFocusTask(data);
        changed |= EnsureUrgentEventPrepTasks(data);
        return changed;
    }

    public IReadOnlyList<string> BuildDailyPlan(StudyData data)
    {
        List<string> plan = [];
        StudyModule? focusModule = GetRecommendedFocusModule(data);
        StudyTask? focusTask = GetRecommendedFocusTask(data);

        if (focusModule is not null)
        {
            plan.Add($"Start with {focusModule.Name} for {data.Settings.PomodoroMinutes} minutes.");
        }

        if (focusTask is not null)
        {
            plan.Add($"Next task: {focusTask.Title}.");
        }

        foreach (StudyEvent urgentEvent in data.Events.Where(studyEvent => studyEvent.IsUrgent).OrderBy(studyEvent => studyEvent.Date).Take(2))
        {
            plan.Add($"Prepare for {urgentEvent.Title} by {urgentEvent.DateLabel}.");
        }

        if (plan.Count == 0)
        {
            plan.Add("No urgent items. Use Zen Mode for a light review session.");
        }

        return plan;
    }

    public StudyModule? GetRecommendedFocusModule(StudyData data)
    {
        return data.Modules
            .OrderBy(module => module.EffectiveProgress)
            .ThenByDescending(module => module.Tasks.Count(task => !task.IsDone))
            .FirstOrDefault();
    }

    public StudyTask? GetRecommendedFocusTask(StudyData data)
    {
        return GetRecommendedFocusModule(data)?.Tasks
            .Where(task => !task.IsDone)
            .OrderByDescending(task => task.Priority)
            .ThenBy(task => task.DueDate ?? DateTimeOffset.MaxValue)
            .FirstOrDefault();
    }

    private static bool EnsureDailyFocusTask(StudyData data)
    {
        StudyModule? module = data.Modules
            .OrderBy(studyModule => studyModule.EffectiveProgress)
            .FirstOrDefault(studyModule => studyModule.Tasks.Any(task => !task.IsDone));

        if (module is null)
        {
            return false;
        }

        bool alreadyCreated = module.Tasks.Any(task =>
            task.Notes?.Contains(AutomationTag, StringComparison.OrdinalIgnoreCase) == true &&
            task.DueDate?.Date == DateTimeOffset.Now.Date);

        if (alreadyCreated)
        {
            return false;
        }

        module.Tasks.Add(new StudyTask
        {
            Title = $"Daily focus: {module.Name}",
            DueDate = DateTimeOffset.Now.Date,
            EstimatedMinutes = data.Settings.PomodoroMinutes,
            Priority = 2,
            Notes = $"Created automatically by WinGets daily planning. {AutomationTag}"
        });

        return true;
    }

    private static bool EnsureUrgentEventPrepTasks(StudyData data)
    {
        bool changed = false;
        IEnumerable<StudyEvent> urgentEvents = data.Events
            .Where(studyEvent => studyEvent.IsUrgent && !string.IsNullOrWhiteSpace(studyEvent.ModuleId));

        foreach (StudyEvent urgentEvent in urgentEvents)
        {
            StudyModule? module = data.Modules.FirstOrDefault(studyModule => studyModule.Id == urgentEvent.ModuleId);
            if (module is null)
            {
                continue;
            }

            string marker = $"{AutomationTag}:{urgentEvent.Id}";
            bool alreadyCreated = module.Tasks.Any(task => task.Notes?.Contains(marker, StringComparison.OrdinalIgnoreCase) == true);
            if (alreadyCreated)
            {
                continue;
            }

            module.Tasks.Add(new StudyTask
            {
                Title = $"Prep for {urgentEvent.Title}",
                DueDate = urgentEvent.Date,
                EstimatedMinutes = data.Settings.PomodoroMinutes,
                Priority = 3,
                Notes = $"Created automatically because this event is urgent. {marker}"
            });
            changed = true;
        }

        return changed;
    }
}
