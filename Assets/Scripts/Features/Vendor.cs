using UnityEngine;

public class Vendor : MonoBehaviour
{
    [Header("Character Data")]
    [SerializeField] private CharacterData vendorData;
    public CharacterData Data => vendorData;
}
