using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleDoor : MonoBehaviour
{
    [SerializeField] private Animator DoorAnimator;
    [SerializeField] private AudioSource doorOpenEffect;
    [SerializeField] private AudioSource doorCloseEffect;

    public void OpenDoor()
    {
        DoorAnimator.SetBool("isOpen", true);
        if (doorOpenEffect != null) { doorOpenEffect.Play(); }
    }

    public void CloseDoor()
    {
        DoorAnimator.SetBool("isOpen", false);
        if (doorCloseEffect != null) { doorCloseEffect.Play(); }
    }
}
