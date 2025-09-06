using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ArchiveSaveData
{
    public List<ArchiveEntry> unlockedArchives = new();
    public List<string> unlockedItems = new List<string>();
}

[System.Serializable]
public class ArchiveEntry
{
    public string characterName;
    public List<string> cutsceneNames = new();
}

