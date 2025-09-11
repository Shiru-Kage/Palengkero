using UnityEngine;

[System.Serializable]
public class FamilyMemberData
{
    public string callerName;
    public string callerDescription;
    public int familyDeduction;
}

[CreateAssetMenu(fileName = "New Family Aid Data", menuName = "Family Aid/Family Aid Data")]
public class FamilyAidData : ScriptableObject
{
    [Header("Family Aid Details")]
    public FamilyMemberData[] familyMembers; 

    public FamilyMemberData GetFamilyMember(int index)
    {
        if (index >= 0 && index < familyMembers.Length)
        {
            return familyMembers[index];
        }
        else
        {
            Debug.LogError("Family member index out of range.");
            return null;
        }
    }
}
