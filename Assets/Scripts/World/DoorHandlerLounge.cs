using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorHandlerLounge : MonoBehaviour
{
    [SerializeField] private ToggleDoor toggleDoor;
    [SerializeField] private string canTriggerByTag;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag(canTriggerByTag))
        {
            toggleDoor.OpenDoor();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag(canTriggerByTag))
        {
            toggleDoor.CloseDoor();
        }
    }
}
