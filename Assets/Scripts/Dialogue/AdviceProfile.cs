using UnityEngine;

[CreateAssetMenu(fileName = "AdviceProfile", menuName = "Game/AdviceProfile")]
public class AdviceProfile : ScriptableObject
{
    [Header("Advice")]
    [TextArea]
    public string adviceText;

    [Header("Requirements")]
    public bool requiresLowSavings;
    public bool requiresLowNutrition;
    public bool requiresLowSatisfaction;
    public bool requiresSavingsMet;
    public bool requiresPerfectGoals;
    public bool requiresAllFailedGoals;
}
