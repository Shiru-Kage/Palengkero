using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CutsceneBackground : MonoBehaviour
{
    public bool isSwitched = false;
    public Image image;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void SwitchImage(Sprite sprite)
    {
        if (!isSwitched)
        {
            SetImage(sprite);
            animator.SetTrigger("SwitchFirst");
        }
        else
        {
            SetImage(sprite);
            animator.SetTrigger("SwitchSecond");
        }
        isSwitched = !isSwitched;
    }

    public void SetImage(Sprite sprite)
    {
        image.sprite = sprite;
    }

    public Sprite GetImage()
    {
        return image.sprite;
    }
}
