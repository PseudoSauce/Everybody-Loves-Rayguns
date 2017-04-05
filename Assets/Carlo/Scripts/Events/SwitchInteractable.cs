using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyTypes;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class SwitchInteractable : Interactable
{
    [Header("Door")]
    public bool m_isDoorSwitch = true;
    [SerializeField, Tooltip("ID's of the doors connected to this switch.")]
    private uint[] m_DoorID;
    // Has the switch been triggered
    bool wasTriggered = false;

    [Header("Treasure Chest")]
    public bool m_isChestSwitch = false;
    [SerializeField, Tooltip("ID's of the doors connected to this switch.")]
    private uint[] m_chestID;
    private bool m_canPickup = true;
    private bool m_isChestOpen = false;

    ////////////////////////////////
    //     Interactable Stuff 
    ///////////////////////////////
    protected override void Init()
    {
        if (m_isDoorSwitch)
        {
            AssignInteractionType(Interaction.DOOR);
        }
        else if(m_isChestSwitch)
        {
            AssignInteractionType(Interaction.TREASURECHEST);
        }

        AssignUpdate(MyUpdate);

        // assigns required for this particular interactable
        AssignCustomEventReceiveNotify(ReceiveCustomEvent, ReceiveManagerEvent);
    }

    private void MyUpdate(float deltaTime)
    {
        if (m_canPickup && m_isChestOpen)
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hitInfo;
                if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitInfo))
                {
                    if(hitInfo.collider.GetComponent<TreasurePickup>())
                    {
                        foreach (uint i in m_chestID)
                        {
                            if (hitInfo.collider.GetComponent<TreasurePickup>().chestID == i)
                            {
                                foreach (uint c in m_chestID)
                                {
                                    var chest = new ChestEventHandler();
                                    chest.ChestID = c;
                                    chest.chestEvent = CustomChestEvent.PickupTreasure;
                                    // invoking the event
                                    EventBeacon.InvokeEvent(chest);
                                    m_canPickup = false;
                                }
                            }
                        }
                    }
                }
            }
        }
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
        if (m_isDoorSwitch)
        {
            ObjectTag otherObject = other.GetComponent<ObjectTag>();
            if (otherObject != null)
            {
                if (!wasTriggered && otherObject.objectTag == ObjectTags.Cube)
                {
                    TriggerSwitch();
                }
            }
        }
        else if (m_isChestSwitch)
        {
            foreach (uint i in m_chestID)
            {
                var chest = new ChestEventHandler();
                chest.ChestID = i;
                chest.chestEvent = CustomChestEvent.OpenChest;
                // invoking the event
                EventBeacon.InvokeEvent(chest);
                m_isChestOpen = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (m_isDoorSwitch)
        {
            ObjectTag otherObject = other.GetComponent<ObjectTag>();
            if (otherObject != null)
            {
                if (!wasTriggered && otherObject.objectTag == ObjectTags.Cube)
                {
                    UnTriggerSwitch();
                }
            }
        }
        else if (m_isChestSwitch)
        {
            foreach (uint i in m_chestID)
            {
                var chest = new ChestEventHandler();
                chest.ChestID = i;
                chest.chestEvent = CustomChestEvent.CloseChest;
                // invoking the event
                EventBeacon.InvokeEvent(chest);
                m_isChestOpen = false;
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
