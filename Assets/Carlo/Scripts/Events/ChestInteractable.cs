using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyTypes;

public class ChestInteractable : Interactable
{
    [SerializeField]
    private int m_chestID;
    [SerializeField]
    private TreasurePickup m_treasure;
    private Animator m_animator;

    ////////////////////////////////
    //     Interactable Stuff 
    ///////////////////////////////

    protected override void Init()
    {
        AssignInteractionType(Interaction.TREASURECHEST);
        AssignStart(MyStart);
        AssignUpdate(MyUpdate);

        // required right now for both sending and receiving events.
        // use the EventBeacon do send, check received events, etc.
        AssignCustomEventReceiveNotify(ReceiveCustomEvent, ReceiveManagerEvent);
    }

    private void MyStart()
    {
        // registering event for interactable to trigger
        EventBeacon.RegisterEvents((uint)CustomChestEvent.OpenChest);
        EventBeacon.RegisterEvents((uint)CustomChestEvent.CloseChest);
        EventBeacon.RegisterEvents((uint)CustomChestEvent.PickupTreasure);

        m_treasure = GetComponentInChildren<TreasurePickup>();
    }

    private void MyUpdate(float deltaTime)
    {

    }

    ////////////////////////////////
    //     Event Stuff
    ///////////////////////////////
    private void ReceiveCustomEvent(CustomEventPacket handlerPacket)
    {
        var eventID = handlerPacket.Handler.EventID;

        // Signal to open the door
        if ((CustomSwitchEvent)eventID == CustomSwitchEvent.OpenDoor)
        {
            ChestEventHandler handler = (ChestEventHandler)handlerPacket.Handler;
            if (handler.ChestID == m_chestID)
            {
                switch(handler.EventID)
                {
                    case (uint)CustomChestEvent.OpenChest:
                        OpenChest();
                        break;
                    case (uint)CustomChestEvent.CloseChest:
                        CloseChest();
                        break;
                    case (uint)CustomChestEvent.PickupTreasure:
                        PickupTreasure();
                        break;
                }
            }
        }
    }

    // check against "reserved" id
    private void ReceiveManagerEvent(ICustomEventManagerHandler handler)
    {

    }

    ////////////////////////////////
    //     Other Stuff
    ///////////////////////////////
    private void OpenChest()
    {
        m_animator.SetBool("Open", true);
    }

    private void CloseChest()
    {
        m_animator.SetBool("Open", false);
    }

    private void PickupTreasure()
    {
        m_treasure.transform.parent = null;
        m_treasure.transform.position = Camera.main.transform.position + (Vector3.forward * 3);
        m_treasure.transform.parent = Camera.main.transform;
    }
}
