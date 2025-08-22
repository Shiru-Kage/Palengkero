#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;

[CustomEditor(typeof(Stall))]
public class StallEditor : Editor
{
    private List<string> jsonItemIDs = new();
    private Dictionary<string, bool> toggleStates = new();

    private const int maxSelectable = 5;

    private void OnEnable()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "item_data.json");
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            var database = JsonUtility.FromJson<ItemDatabase>(json);
            jsonItemIDs.Clear();
            toggleStates.Clear();

            foreach (var item in database.items)
            {
                jsonItemIDs.Add(item.id);
                toggleStates[item.id] = false; // default false
            }
        }
        else
        {
            Debug.LogWarning("Cannot find item_data.json in StreamingAssets for StallEditor.");
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        Stall stall = (Stall)target;

        EditorGUILayout.PropertyField(serializedObject.FindProperty("stallCooldown"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("useManualItems"));

        SerializedProperty listProp = serializedObject.FindProperty("manuallyAssignedItemIDs");

        // Sync toggle state with existing list
        for (int i = 0; i < listProp.arraySize; i++)
        {
            string existingId = listProp.GetArrayElementAtIndex(i).stringValue;
            if (toggleStates.ContainsKey(existingId))
                toggleStates[existingId] = true;
        }

        if (stall.useManualItems && jsonItemIDs.Count > 0)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField($"Select up to {maxSelectable} items:", EditorStyles.boldLabel);

            int selectedCount = GetSelectedCount();

            foreach (string itemId in jsonItemIDs)
            {
                bool currentState = toggleStates[itemId];
                bool newState = EditorGUILayout.ToggleLeft(itemId, currentState);

                if (newState != currentState)
                {
                    if (newState && selectedCount >= maxSelectable)
                    {
                        EditorGUILayout.HelpBox("Maximum of 5 items allowed.", MessageType.Warning);
                        continue;
                    }
                    toggleStates[itemId] = newState;
                    selectedCount += newState ? 1 : -1;
                }
            }

            // Write back to SerializedProperty
            listProp.ClearArray();
            int index = 0;
            foreach (var kvp in toggleStates)
            {
                if (kvp.Value)
                {
                    listProp.InsertArrayElementAtIndex(index);
                    listProp.GetArrayElementAtIndex(index).stringValue = kvp.Key;
                    index++;
                }
            }
        }

        serializedObject.ApplyModifiedProperties();
    }

    private int GetSelectedCount()
    {
        int count = 0;
        foreach (var entry in toggleStates)
        {
            if (entry.Value) count++;
        }
        return count;
    }
}
