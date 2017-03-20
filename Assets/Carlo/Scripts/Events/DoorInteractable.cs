using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyTypes;

public class DoorInteractable : Interactable
{
    [Header("Door")]
    [SerializeField]
    private uint m_DoorID;
    [SerializeField]
    private GameObject m_doorObject;
    [SerializeField]
    private Transform m_openPosition, m_closePosition;
    [SerializeField]
    private float m_doorSpeed = 2.0f;

    private bool m_moveToClose = false;
    private bool m_moveToOpen = false;

    ////////////////////////////////
    //     Interactable Stuff 
    ///////////////////////////////
    protected override void Init()
    {
        AssignInteractionType(Interaction.DOOR);
        AssignStart(MyStart);
        AssignUpdate(MyUpdate);

        // required right now for both sending and receiving events.
        // use the EventBeacon do send, check received events, etc.
        AssignCustomEventReceiveNotify(ReceiveCustomEvent, ReceiveManagerEvent);
    }

    private void MyStart()
    {
        // registering event for interactable to trigger
        EventBeacon.RegisterEvents((uint)CustomSwitchEvent.OpenDoor);
        EventBeacon.RegisterEvents((uint)CustomSwitchEvent.CloseDoor);
    }

    private void MyUpdate(float deltaTime)
    {
        MoveDoor();
    }

    ////////////////////////////////
    //     Event Stuff
    ///////////////////////////////
    private void ReceiveCustomEvent(CustomEventPacket handlerPacket)
    {
        var eventID = handlerPacket.Handler.EventID;

        if ((CustomSwitchEvent)eventID == CustomSwitchEvent.OpenDoor)
        {
            DoorOpenEventHandler handler = (DoorOpenEventHandler)handlerPacket.Handler;
            if (handler.DoorID == m_DoorID)
            {
                m_moveToOpen = true;
                m_moveToClose = false;
            }
        }

        if ((CustomSwitchEvent)eventID == CustomSwitchEvent.CloseDoor)
        {
            DoorOpenEventHandler handler = (DoorOpenEventHandler)handlerPacket.Handler;
            if (handler.DoorID == m_DoorID)
            {
                m_moveToOpen = false;
                m_moveToClose = true;
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
    private void MoveDoor()
    {
        if (m_moveToClose)
        {
            Vector3 dir = m_closePosition.position - m_doorObject.transform.position;
            m_doorObject.transform.Translate(dir.normalized * m_doorSpeed * Time.deltaTime);
            if(Vector3.Distance(m_doorObject.transform.position, m_closePosition.position) < 0.1f)
            {
                m_moveToClose = false;
            }
        }
        else if (m_moveToOpen)
        {
            Vector3 dir = m_openPosition.position - m_doorObject.transform.position;
            m_doorObject.transform.Translate(dir.normalized * m_doorSpeed * Time.deltaTime);
            if (Vector3.Distance(m_doorObject.transform.position, m_openPosition.position) < 0.1f)
            {
                m_moveToOpen = false;
            }
        }
    }
}
