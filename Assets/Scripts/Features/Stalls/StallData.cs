using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Stall Data", menuName = "Stall/Stall Data")]
public class StallData : ScriptableObject
{
    [Header("Stall Visuals")]
    public Sprite upperStallIcon;
    public Sprite lowerStallIcon;
    public Sprite stallBackground;
    public Sprite stallUIBackground;
    [Header("Stall Vendor")]
    public GameObject vendor;

    [HideInInspector]
    public List<StallItemEntry> items;
}

[System.Serializable]
public class StallItemEntry
{
    public string itemId;
    public int stock;
}
