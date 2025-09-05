using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Stall Data", menuName = "Stall/Stall Data")]
public class StallData : ScriptableObject
{
    [Header("Stall Visuals")]
    public Sprite upperStallIcon;
    public Sprite lowerStallIcon;
    public Sprite stallBackground;
    public Sprite stallUIRoofBackground;
    [Tooltip("Background for different times of the day.")]
    public Sprite[] morningBackgrounds;
    public Sprite[] afternoonBackgrounds;
    public Sprite[] nightBackgrounds;

    [Header("Stall Vendor")]
    public CharacterData vendor;

    [HideInInspector]
    public List<StallItemEntry> items;
}

[System.Serializable]
public class StallItemEntry
{
    public string itemId;
    public int stock;
}
