using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine;
using MyTypes;


public enum RoomResponseLoaded
{
    UNLOADED, LOADED = 1, BUSY, FAILED, INFO, RoomResponseLoadedEvent = 234842
}

public enum RoomStreamID
{
    UNLOAD = 0, LOAD = 1, QueryInfo, RoomStreamEvent = 9942
}

// a response event handler to loading/unloading a scene
public struct RoomResponseLoadedHandler : ICustomEventHandler
{
    public RoomResponseLoaded loadedResponse;
    public StreamingInteractable.StreamState loadingStreamState;
    public int roomNumber;      

    public uint EventID
    {
        get { return (uint)RoomResponseLoaded.RoomResponseLoadedEvent; }
    }
}

public struct RoomResponseInfoHandler : ICustomEventHandler
{
    public RoomResponseLoaded loadedResponse;
    public StreamingInteractable.StreamState loadingStreamState;
    public RoomStreamID purposeOfQuery;
    public int currentLoadedRoom;

    public uint EventID
    {
        get { return (uint)RoomResponseLoaded.RoomResponseLoadedEvent; }
    }
}

// send this through the EventManager as a request for loading/unloading a scene.
// will response with a RoomResponseHandler
public struct RoomStreamHandler : ICustomEventHandler
{
    public RoomStreamID RoomStreamingID;
    public int roomNumber;

    public bool unloadCurrentRoom; // override room number

    public Transform loadLocation;
    public string connectorObjectName;

    public uint EventID
    {
        get { return (uint)RoomStreamID.RoomStreamEvent;  }
    }
}

// loads the requested scene and "snaps it" to the location provided by the invoker,
// using the RoomStreamHandler for information.
// will respond with an appropriate message using the RoomResponseLoadedHandler.
//
// **
// explicitly can only handle one scene loaded at time. if you try to load another,
// it simply will not work, unless you unload the current scene.
// **
//
// this requires an active EventManager in the scene!
//
public class StreamingInteractable : Interactable {
    public enum StreamState
    {
        FREE, UNLOADING, LOADING, SCENELOADED
    }

    [SerializeField]
    GameObject m_player;

    StreamState m_streamState = StreamState.FREE;

    AsyncOperation m_operation;
    int m_currentRoomIndex = -1;
    string m_currentRoomConnectorName;
    Transform m_currentRoomConnectorPoint;

    bool isWaitingToLoad = false;


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
        if (m_streamState == StreamState.LOADING && m_operation.progress > 0.9f)
        {
            m_streamState = StreamState.SCENELOADED; 

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

            RoomResponseLoadedHandler loadResponse = new RoomResponseLoadedHandler();
            loadResponse.loadedResponse = RoomResponseLoaded.LOADED;
            loadResponse.loadingStreamState = StreamState.SCENELOADED;
            loadResponse.roomNumber = m_currentRoomIndex;

            EventBeacon.InvokeEvent(loadResponse);            
        }
        else if (m_streamState == StreamState.UNLOADING && m_operation.progress > 0.9f)
        {
            m_streamState = StreamState.FREE;

            RoomResponseLoadedHandler unloadResponse;
            unloadResponse.loadedResponse = RoomResponseLoaded.UNLOADED;
            unloadResponse.roomNumber = m_currentRoomIndex;
            unloadResponse.loadingStreamState = StreamState.FREE;

            EventBeacon.InvokeEvent(unloadResponse);

            m_currentRoomIndex = -1;
            m_currentRoomConnectorPoint = null;
            m_currentRoomConnectorName = "";
            m_operation = null;
        }
    }

    void MyCustomEventReceiveNotify(CustomEventPacket handlerPacket)
    {
        var handler = handlerPacket.Handler;

        if (handler is RoomStreamHandler)
        {
            var handlerCasted = (RoomStreamHandler)handlerPacket.Handler;
            var loadOrUnload = (handlerCasted).RoomStreamingID;

            if (loadOrUnload == RoomStreamID.LOAD && (m_streamState == StreamState.FREE || handlerCasted.unloadCurrentRoom))
            {
                if (handlerCasted.unloadCurrentRoom && m_currentRoomIndex >= 0 && m_currentRoomIndex != handlerCasted.roomNumber)
                {
                    StreamUnload(m_currentRoomIndex);

                    isWaitingToLoad = true;
                    StartCoroutine(loadIfFree(handlerCasted));
                    return;
                }
                if (m_currentRoomIndex != handlerCasted.roomNumber)
                    StreamLoad(handlerCasted);
            }
            else if (loadOrUnload == RoomStreamID.LOAD && !isWaitingToLoad && m_streamState == StreamState.UNLOADING)
            {
                isWaitingToLoad = true;
                StartCoroutine(loadIfFree(handlerCasted));
            }
            else if (loadOrUnload == RoomStreamID.UNLOAD && m_streamState == StreamState.SCENELOADED && m_currentRoomIndex != handlerCasted.roomNumber)
            {
                StreamUnload(handlerCasted.roomNumber);
            }
            else if (loadOrUnload == RoomStreamID.QueryInfo)
            {
                RoomResponseInfoHandler info;
                info.currentLoadedRoom = m_currentRoomIndex;
                info.loadedResponse = RoomResponseLoaded.INFO;
                info.loadingStreamState = m_streamState;                
            }
            else
            {
                // generally signifies that this stream is either in a loading/unloading state,
                // or a scene is currently already loaded or unloaded.
                // check the loadingStreamState sent by this response handler, to know for sure.
                RoomResponseLoadedHandler busyResponse;
                busyResponse.loadedResponse = RoomResponseLoaded.BUSY;
                busyResponse.loadingStreamState = m_streamState;
                busyResponse.roomNumber = handlerCasted.roomNumber;

                EventBeacon.InvokeEvent(busyResponse);
            }
        }
    }

    void MyCustomEventReceiveManagerNotify(ICustomEventManagerHandler handler)
    {

    }

    IEnumerator loadIfFree(RoomStreamHandler handler)
    {
        yield return new WaitForEndOfFrame();

        if (m_streamState == StreamState.FREE)
        {
            isWaitingToLoad = false;
            StreamLoad(handler);
        }
        else
        {
            StartCoroutine(loadIfFree(handler));
        }
    }
    
    //[PunRPC]
    void StreamLoad(RoomStreamHandler handler)
    {
        m_operation = SceneManager.LoadSceneAsync(handler.roomNumber, LoadSceneMode.Additive);
        //PhotonNetwork.LoadLevel(handler.roomNumber);
        
        if (SceneManager.sceneCount > 1)
        {
            m_streamState = StreamState.LOADING;
            m_currentRoomIndex = handler.roomNumber;
            m_currentRoomConnectorName = handler.connectorObjectName;
            m_currentRoomConnectorPoint = handler.loadLocation;
        }
        else
        {
            Debug.Log("Failure to load scene: Build Setting: " + handler.roomNumber);
        }
    }

    void StreamUnload(int scene)
    {
        m_streamState = StreamState.UNLOADING;
        m_operation = SceneManager.UnloadSceneAsync(scene);
    }
}
