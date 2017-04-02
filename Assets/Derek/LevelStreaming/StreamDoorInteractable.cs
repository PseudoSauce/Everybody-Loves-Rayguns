using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyTypes;

public class StreamDoorInteractable : Interactable {
    enum DoorState
    {
        CLOSED, OPENING, CLOSING, OPEN
    }

    [Header("Door Stuff"), SerializeField]
    private float m_doorOpenDelay;
    [SerializeField]
    private float m_doorCloseDelay;
    [SerializeField]
    Transform m_initTelePoint;
    [SerializeField]
    private Transform m_closedPos;
    [SerializeField]
    private Transform m_openPos;
    [SerializeField]
    private float m_doorOpeningPadding = 0.5f;

    private DoorState m_doorState = DoorState.CLOSED;

    private bool isTriggered = false;

    [SerializeField]
    private int m_roomNumber = 0;

    protected override void Init()
    {
        AssignInteractionType(MyTypes.Interaction.STREAMING);
        AssignCustomEventReceiveNotify(MyCustomEventReceiveNotify, MyCustomEventReceiveManagerNotify);
        AssignStart(MyCustomStart);
        AssignUpdate(MyCustomUpdate);
        
    }
    void MyCustomStart()
    {
        EventBeacon.RegisterEvents((uint)RoomResponseLoaded.RoomResponseLoadedEvent);
    }

    void MyCustomUpdate(float deltaTime)
    {
        if (!isTriggered && Input.GetKeyDown(KeyCode.F))
        {
            isTriggered = true;

            RoomStreamHandler handler = new RoomStreamHandler();
            handler.connectorObjectName = "connector";
            handler.RoomStreamingID = RoomStreamID.LOAD;
            handler.roomNumber = m_roomNumber;
            handler.loadLocation = m_initTelePoint;

            EventBeacon.InvokeEvent(handler);
        }

        if (m_doorState == DoorState.OPENING)
        {
            transform.Translate(new Vector3(0, m_doorOpenDelay * deltaTime, 0));

            float magnitude = (m_openPos.position - transform.position).magnitude;

            if (magnitude <= m_doorOpeningPadding)
            {
                m_doorState = DoorState.OPEN;
                transform.position = m_openPos.position;
            }            
        }
        else if (m_doorState == DoorState.CLOSING)
        {
            transform.Translate(new Vector3(0, -m_doorOpenDelay * deltaTime, 0));

            float magnitude = (m_closedPos.position - transform.position).magnitude;

            if (magnitude <= m_doorOpeningPadding)
            {
                m_doorState = DoorState.CLOSED;
                transform.position = m_closedPos.position;
            }
        }
    }

    void MyCustomEventReceiveNotify(CustomEventPacket handlerPacket)
    {
        var handler = handlerPacket.Handler;

        if (handler is RoomResponseLoadedHandler)
        {
            var handlerCasted = (RoomResponseLoadedHandler)handler;

            if (handlerCasted.roomNumber == m_roomNumber && 
                handlerCasted.loadedResponse == RoomResponseLoaded.LOADED)
            {
                m_doorState = DoorState.OPENING;
            }
            else if (handlerCasted.roomNumber == m_roomNumber && 
                handlerCasted.loadedResponse == RoomResponseLoaded.UNLOADED)
            {
                m_doorState = DoorState.CLOSING;
            }
            else
            {
                Debug.Log("Not the appropriate door number. Am looking for: " + m_roomNumber);
            }
        }
    }

    void MyCustomEventReceiveManagerNotify(ICustomEventManagerHandler handler)
    {

    }
}
