using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine;
using MyTypes;


public enum RoomResponseLoaded
{
    UNLOADED, LOADED = 1, RoomResponseLoadedEvent = 234842
}

public enum RoomStreamID
{
    UNLOAD = 0, LOAD = 1, RoomStreamEvent = 9942
}

public struct RoomResponseLoadedHandler : ICustomEventHandler
{
    public RoomResponseLoaded loadedResponse;
    public int roomNumber;      

    public uint EventID
    {
        get { return (uint)RoomResponseLoaded.RoomResponseLoadedEvent; }
    }
}

public struct RoomStreamHandler : ICustomEventHandler
{
    public RoomStreamID RoomStreamingID;
    public int roomNumber;

    public Transform loadLocation;
    public string connectorObjectName;

    public uint EventID
    {
        get { return (uint)RoomStreamID.RoomStreamEvent;  }
    }
}

public class StreamingInteractable : Interactable {
    [SerializeField]
    GameObject m_player;

    bool m_loadingScene = false;

    AsyncOperation m_operation;
    int m_currentRoomIndex;
    string m_currentRoomConnectorName;
    Transform m_currentRoomConnectorPoint;

    bool m_sceneCurrentlyLoaded = false;

    protected override void Init()
    {
        AssignInteractionType(MyTypes.Interaction.STREAMING);
        AssignCustomEventReceiveNotify(MyCustomEventReceiveNotify, MyCustomEventReceiveManagerNotify);
        
        AssignStart(MyCustomStart);
        AssignUpdate(MyCustomUpdate);

    }
    void MyCustomStart()
    {
        EventBeacon.RegisterEvents((uint)RoomStreamID.RoomStreamEvent);
    }

    void MyCustomUpdate(float deltaTime)
    {
        if (m_loadingScene && m_operation.progress > 0.9f)
        {
            m_loadingScene = false;

            // send event back to the invoker through event system
            GameObject connector = null;
            Scene scene = SceneManager.GetSceneByBuildIndex(m_currentRoomIndex);
            var objects = scene.GetRootGameObjects();
            foreach (GameObject obj in objects)
            {
                if (obj.tag == m_currentRoomConnectorName)
                {
                    connector = obj;
                    break;
                }
            }
            connector.transform.position = m_currentRoomConnectorPoint.position;
            connector.transform.rotation = m_currentRoomConnectorPoint.rotation;
            connector.transform.localScale = m_currentRoomConnectorPoint.localScale;

            RoomResponseLoadedHandler handler = new RoomResponseLoadedHandler();
            handler.loadedResponse = RoomResponseLoaded.LOADED;
            handler.roomNumber = m_currentRoomIndex;

            EventBeacon.InvokeEvent(handler);            
        }
    }

    void MyCustomEventReceiveNotify(CustomEventPacket handlerPacket)
    {
        var handler = handlerPacket.Handler;

        if (handler is RoomStreamHandler)
        {
            var handlerCasted = (RoomStreamHandler)handlerPacket.Handler;
            var loadOrUnload = (handlerCasted).RoomStreamingID;

            if (loadOrUnload == RoomStreamID.LOAD && !m_sceneCurrentlyLoaded)
            {
                StreamLoad(handlerCasted);
            }
            else if (loadOrUnload == RoomStreamID.UNLOAD && m_sceneCurrentlyLoaded)
            {
                StreamUnload(handlerCasted.roomNumber);
            }
        }
    }
    void MyCustomEventReceiveManagerNotify(ICustomEventManagerHandler handler)
    {

    }

    void StreamLoad(RoomStreamHandler handler)
    {
        m_operation = SceneManager.LoadSceneAsync(handler.roomNumber, LoadSceneMode.Additive);

        if (SceneManager.sceneCount > 1)
        {
            m_sceneCurrentlyLoaded = true;
            m_currentRoomIndex = handler.roomNumber;
            m_currentRoomConnectorName = handler.connectorObjectName;
            m_currentRoomConnectorPoint = handler.loadLocation;
        }
        else
        {
            Debug.Log("Failure to load scene: Build Setting: " + handler.roomNumber);
        }

        m_loadingScene = true;
    }

    void StreamUnload(int scene)
    {
        SceneManager.UnloadSceneAsync(scene);
        m_currentRoomIndex = -1;
        m_currentRoomConnectorPoint = null;
        m_currentRoomConnectorName = "";
        m_loadingScene = false;
        m_operation = null;

        m_sceneCurrentlyLoaded = false;
    }
}
