using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class TutorialStep
{
    [TextArea] public string instructionText;   // what to tell the player
    public UnityEvent onStepStart;              // actions to do at start (highlight UI, play dialogue, etc.)
    public UnityEvent onStepComplete;           // cleanup or triggers after completion
    public string requiredAction;               // e.g., "OpenStall", "BuyItem"
}
