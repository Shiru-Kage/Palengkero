using UnityEngine;
using System.IO;

public static class SaveSystem
{
    private static string GetPathForSlot(int slotIndex)
    {
        return Path.Combine(Application.persistentDataPath, $"save_slot_{slotIndex}.json");
    }

    private static string GetArchivePath()
    {
        return Path.Combine(Application.persistentDataPath, "archives.json");
    }

    public static void SaveToSlot(int slotIndex, SaveData data)
    {
        string path = GetPathForSlot(slotIndex);
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);
        Debug.Log($"Saved to slot {slotIndex} at {path}");
    }

    public static SaveData LoadFromSlot(int slotIndex)
    {
        string path = GetPathForSlot(slotIndex);
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<SaveData>(json);
        }

        Debug.LogWarning($"No save file found for slot {slotIndex}");
        return null;
    }

    public static bool SaveExists(int slotIndex)
    {
        return File.Exists(GetPathForSlot(slotIndex));
    }

    public static void DeleteSlot(int slotIndex)
    {
        string path = GetPathForSlot(slotIndex);
        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log($"Deleted save slot {slotIndex}");
        }
    }

    public static void SaveArchives(ArchiveSaveData data)
    {
        string path = GetArchivePath();
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);
        Debug.Log($"[Archive] Saved archives to {path}");
    }

    public static ArchiveSaveData LoadArchives()
    {
        string path = GetArchivePath();
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<ArchiveSaveData>(json);
        }

        Debug.Log("[Archive] No archives file found, returning empty");
        return new ArchiveSaveData();
    }
}
