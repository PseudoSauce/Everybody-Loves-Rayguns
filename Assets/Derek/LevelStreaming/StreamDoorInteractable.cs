using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyTypes;

public class StreamDoorInteractable : Interactable {
    enum DoorState {
        CLOSED, OPENING, CLOSING, OPEN
    }

    [Header("Door Stuff"), SerializeField]
    //private float m_doorOpenDelay;
    //[SerializeField]
    //private float m_doorCloseDelay;
    //[SerializeField]
    Transform m_initTelePoint;
    //private float m_doorOpeningPadding = 0.5f;
    //[SerializeField]
    private float m_doorOpenAngle = 90.0f;
    //[SerializeField]
    private float openCloseDist = 4.0f;
    private DoorState m_doorState = DoorState.CLOSED;

    private bool isTriggered = false;
    private bool hasEntered = false;

    [SerializeField]
    private int m_roomNumber = 0;

    GameObject g_player;
    protected override void Init() {
        AssignInteractionType(MyTypes.Interaction.STREAMING);
        AssignCustomEventReceiveNotify(MyCustomEventReceiveNotify, MyCustomEventReceiveManagerNotify);
        AssignStart(MyCustomStart);
        AssignUpdate(MyCustomUpdate);

    }
    void MyCustomStart() {
        g_player = GameObject.FindGameObjectWithTag("Player");
        EventBeacon.RegisterEvents((uint)RoomResponseLoaded.RoomResponseLoadedEvent);
    }

    //void OnTriggerEnter(Collider other) {
    //    if (other.CompareTag("Player")) {
    //        hasEntered = true;
    //    }
    //}

    //void OnTriggerExit(Collider other) {
    //    if (other.CompareTag("Player")) {
    //        hasEntered = false;
    //    }
    //}

    void CheckerInside() {
        float dist = Vector3.Distance(transform.position, g_player.transform.position);
        if (dist < openCloseDist) {
            hasEntered = true;
        } else {
            hasEntered = false;
        }
    }

    void MyCustomUpdate(float deltaTime) {
        CheckerInside();

        if (!isTriggered && hasEntered) {
            isTriggered = true;
            RoomStreamHandler handler = new RoomStreamHandler();
            handler.connectorObjectName = "connector";
            handler.RoomStreamingID = RoomStreamID.LOAD;
            handler.roomNumber = m_roomNumber;
            handler.loadLocation = m_initTelePoint;
            EventBeacon.InvokeEvent(handler);
        } else if (m_doorState == DoorState.OPEN && !hasEntered) {
            m_doorState = DoorState.CLOSING;
        }

        if (m_doorState == DoorState.OPENING) {
            doorOpenClose(true);
        } else if (m_doorState == DoorState.CLOSING) {
            doorOpenClose(false);
        }
    }

    void doorOpenClose(bool isOpening) {
        Quaternion target = Quaternion.Euler(0, m_doorOpenAngle, 0);
        if (isOpening) {
            transform.localRotation = Quaternion.Slerp(transform.localRotation, target, Time.deltaTime * 2.0f);
            float finalAngOpen = Quaternion.Angle(transform.rotation, target);
            if (finalAngOpen < m_doorOpenAngle / 2 + 1.0f) {
                m_doorState = DoorState.OPEN;
                print("Door now Open");
            }
        } else {
            transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.identity, Time.deltaTime * 2.0f);
            float finalAngClose = Quaternion.Angle(transform.rotation, Quaternion.identity);
            if (finalAngClose > m_doorOpenAngle / 2 - 1.0f) {
                m_doorState = DoorState.CLOSED;
                print("Door now Closed");
            }
        }
    }

    void MyCustomEventReceiveNotify(CustomEventPacket handlerPacket) {
        var handler = handlerPacket.Handler;

        if (handler is RoomResponseLoadedHandler) {
            var handlerCasted = (RoomResponseLoadedHandler)handler;

            if (handlerCasted.roomNumber == m_roomNumber &&
                handlerCasted.loadedResponse == RoomResponseLoaded.LOADED) {
                m_doorState = DoorState.OPENING;
            } else if (handlerCasted.roomNumber == m_roomNumber &&
                  handlerCasted.loadedResponse == RoomResponseLoaded.UNLOADED) {
                m_doorState = DoorState.CLOSING;
            } else if (handlerCasted.roomNumber == m_roomNumber &&
                  handlerCasted.loadedResponse == RoomResponseLoaded.BUSY) {
                print("Busy dial tone... : " + handlerCasted.loadingStreamState);
            } else {
                Debug.Log("Not the appropriate door number. Am looking for: " + m_roomNumber);
            }
        }
    }

    void MyCustomEventReceiveManagerNotify(ICustomEventManagerHandler handler) {

    }
}
