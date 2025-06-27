using UnityEngine;

[CreateAssetMenu(fileName = "New NPC Data", menuName = "NPC/NPC Data")]
public class NPCData : ScriptableObject
{
    public Sprite npcSprite;
    
    [Header("Preferences")]
    [Range(0, 100)]
    public int preferCheapItemsChance = 50;

    [Range(0, 100)]
    public int preferHighNutritionChance = 50;

    [Range(0, 100)]
    public int preferHighSatisfactionChance = 50;
}
