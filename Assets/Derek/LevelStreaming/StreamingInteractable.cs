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

    Scene m_hubScene;

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

        m_hubScene = SceneManager.GetActiveScene();
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

            Debug.Log(handler.roomNumber);
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

            if (loadOrUnload == RoomStreamID.LOAD)
            {
                m_currentRoomIndex = handlerCasted.roomNumber;
                m_currentRoomConnectorName = handlerCasted.connectorObjectName;
                m_currentRoomConnectorPoint = handlerCasted.loadLocation;

                StreamLoad(handlerCasted.roomNumber);
            }
            else if (loadOrUnload == RoomStreamID.UNLOAD)
            {

            }
        }
    }
    void MyCustomEventReceiveManagerNotify(ICustomEventManagerHandler handler)
    {

    }

    void StreamLoad(int sceneIndex)
    {
        m_operation = SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);
        m_loadingScene = true;
    }

    void StreamUnload(int scene)
    {
        SceneManager.UnloadSceneAsync(m_currentRoomIndex);
        m_currentRoomIndex = -1;
        m_currentRoomConnectorPoint = null;
        m_currentRoomConnectorName = "";
        m_loadingScene = false;
        m_operation = null;
    }
}
