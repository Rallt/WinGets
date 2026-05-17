using System.Text.Json;
using System.Text.Json.Serialization;
using Windows.Storage;
using WinGets.App.Models;

namespace WinGets.App.Services;

public sealed class StudyDataService
{
    private const string FileName = "study-data.json";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };

    public async Task<StudyData> LoadAsync()
    {
        // Persistence is intentionally local, private, and JSON-based so the first-run
        // experience works without a server or extra dependencies.
        StorageFolder localFolder = ApplicationData.Current.LocalFolder;
        StorageFile? file = await TryGetFileAsync(localFolder, FileName);

        if (file is null)
        {
            var defaults = CreateDefaultData();
            await SaveAsync(defaults);
            return defaults;
        }

        string json = await FileIO.ReadTextAsync(file);
        if (string.IsNullOrWhiteSpace(json))
        {
            return CreateDefaultData();
        }

        StudyData? data = JsonSerializer.Deserialize<StudyData>(json, JsonOptions);
        return EnsureData(data ?? CreateDefaultData());
    }

    public async Task SaveAsync(StudyData data)
    {
        foreach (StudyModule module in data.Modules)
        {
            module.RefreshProgressFromTasks();
        }

        StorageFolder localFolder = ApplicationData.Current.LocalFolder;
        StorageFile file = await localFolder.CreateFileAsync(FileName, CreationCollisionOption.ReplaceExisting);
        string json = JsonSerializer.Serialize(data, JsonOptions);
        await FileIO.WriteTextAsync(file, json);
    }

    private static async Task<StorageFile?> TryGetFileAsync(StorageFolder folder, string fileName)
    {
        try
        {
            return await folder.GetFileAsync(fileName);
        }
        catch (FileNotFoundException)
        {
            return null;
        }
    }

    private static StudyData EnsureData(StudyData data)
    {
        data.Settings ??= new AppSettings();
        data.Modules ??= [];
        data.Events ??= [];
        return data;
    }

    private static StudyData CreateDefaultData()
    {
        var mathModule = new StudyModule
        {
            Name = "Calculus",
            AccentColor = "#8BB8FF",
            Tasks =
            {
                new StudyTask { Title = "Review derivatives", IsDone = true },
                new StudyTask { Title = "Practice integration by parts" },
                new StudyTask { Title = "Summarize chapter 6 notes" }
            }
        };

        var designModule = new StudyModule
        {
            Name = "Design Systems",
            AccentColor = "#C8B6FF",
            Tasks =
            {
                new StudyTask { Title = "Annotate typography examples", IsDone = true },
                new StudyTask { Title = "Build spacing flash cards", IsDone = true },
                new StudyTask { Title = "Prepare critique questions" }
            }
        };

        var historyModule = new StudyModule
        {
            Name = "Modern History",
            AccentColor = "#B7F4D8",
            Tasks =
            {
                new StudyTask { Title = "Outline essay argument" },
                new StudyTask { Title = "Collect primary source quotes" }
            }
        };

        mathModule.RefreshProgressFromTasks();
        designModule.RefreshProgressFromTasks();
        historyModule.RefreshProgressFromTasks();

        return new StudyData
        {
            Modules = { mathModule, designModule, historyModule },
            Events =
            {
                new StudyEvent { Title = "Calculus quiz", ModuleId = mathModule.Id, Date = DateTimeOffset.Now.AddDays(2), EventType = "Exam" },
                new StudyEvent { Title = "Design critique", ModuleId = designModule.Id, Date = DateTimeOffset.Now.AddDays(5), EventType = "Presentation" },
                new StudyEvent { Title = "History source packet", ModuleId = historyModule.Id, Date = DateTimeOffset.Now.AddDays(1), EventType = "Deadline" }
            }
        };
    }
}
