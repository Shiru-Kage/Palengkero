using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ArchiveSaveData
{
    public List<ArchiveEntry> unlockedArchives = new();
}

[System.Serializable]
public class ArchiveEntry
{
    public string characterName;
    public List<string> cutsceneNames = new();
}

