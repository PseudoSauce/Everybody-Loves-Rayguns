using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyTypes;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class SwitchInteractable : Interactable
{
    [Header("Door")]
    [SerializeField, Tooltip("ID's of the doors connected to this switch.")]
    private uint[] m_DoorID;
    // Has the switch been triggered
    private bool wasTriggered = false;
    private Animator m_animator;

    ////////////////////////////////
    //     Interactable Stuff 
    ///////////////////////////////
    protected override void Init()
    {
        AssignInteractionType(Interaction.DOOR);
        AssignStart(MyStart);
        // assigns required for this particular interactable
        AssignCustomEventReceiveNotify(ReceiveCustomEvent, ReceiveManagerEvent);
    }

    private void MyStart()
    {
        m_animator = GetComponent<Animator>();
    }

    ////////////////////////////////
    //     Custom Event Stuff 
    ///////////////////////////////
    private void ReceiveCustomEvent(CustomEventPacket handlerPacket)
    {
    }

    private void ReceiveManagerEvent(ICustomEventManagerHandler handler)
    {

    }

    ////////////////////////////////
    //     Other Stuff
    ///////////////////////////////
    private void OnTriggerEnter(Collider other)
    {
        ObjectTag otherObject = other.GetComponent<ObjectTag>();
        if (otherObject != null)
        {
            if (!wasTriggered && otherObject.objectTag == ObjectTags.Cube)
            {
                TriggerSwitch();
                m_animator.SetBool("Pressed", true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        ObjectTag otherObject = other.GetComponent<ObjectTag>();
        if (otherObject != null)
        {
            if (!wasTriggered && otherObject.objectTag == ObjectTags.Cube)
            {
                UnTriggerSwitch();
                m_animator.SetBool("Pressed", false);
            }
        }
    }

    private void TriggerSwitch()
    {
        wasTriggered = true;
        foreach(uint i in m_DoorID)
        {
            var doorOpen = new DoorOpenEventHandler();
            doorOpen.DoorID = i;
            // invoking the event
            EventBeacon.InvokeEvent(doorOpen);
        }
    }

    private void UnTriggerSwitch()
    {
        wasTriggered = true;
        foreach (uint i in m_DoorID)
        {
            var doorClose = new DoorCloseEventHandler();
            doorClose.DoorID = i;
            // invoking the event
            EventBeacon.InvokeEvent(doorClose);
        }
    }


}
