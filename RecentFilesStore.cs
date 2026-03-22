using System.IO;
using System.Text.Json;

namespace ByeVS_Memo;

internal static class RecentFilesStore
{
    private const string FileName = "recent_files.json";
    private const int MaxCount = 10;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
    };

    public static List<string> Load()
    {
        if (!File.Exists(FileName))
            return new List<string>();

        try
        {
            string json = File.ReadAllText(FileName);
            return JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
        }
        catch
        {
            return new List<string>();
        }
    }

    public static void Add(string path)
    {
        string full = Path.GetFullPath(path);
        List<string> list = Load();
        list.RemoveAll(p => string.Equals(p, full, StringComparison.OrdinalIgnoreCase));
        list.Insert(0, full);
        while (list.Count > MaxCount)
            list.RemoveAt(list.Count - 1);
        Save(list);
    }

    public static void Remove(string path)
    {
        string full = Path.GetFullPath(path);
        List<string> list = Load();
        list.RemoveAll(p => string.Equals(p, full, StringComparison.OrdinalIgnoreCase));
        Save(list);
    }

    private static void Save(List<string> paths)
    {
        string json = JsonSerializer.Serialize(paths, JsonOptions);
        File.WriteAllText(FileName, json);
    }
}
