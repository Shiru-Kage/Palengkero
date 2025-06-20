using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Stall : Interactable
{
    [SerializeField] private GameObject stallUI;
    [SerializeField] private StallManager stallManager;

    public override void Interact()
    {
        base.Interact();
        stallUI.SetActive(true);
    }

    public void SetBlinking(bool shouldBlink)
    {
        if (stallManager == null) return;

        if (shouldBlink)
            stallManager.StartBlinkingOutline();
        else
            stallManager.StopBlinkingOutline();
    }
}
