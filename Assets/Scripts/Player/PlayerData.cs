using UnityEngine;

public class PlayerData : MonoBehaviour
{
    [Header("Character Data")]
    [SerializeField] private CharacterData playerData;
    public CharacterData Data => playerData;
}
