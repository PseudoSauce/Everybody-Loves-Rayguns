using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Collections;

public class NetworkingUI : MonoBehaviour
{
    [System.Serializable]
    public class roomList
    {
        public Text name;
        public Text number;
        public Text availability;
        public GameObject join;
    }

    public GameObject m_RoomCanvas;
    public GameObject m_Selection;
    public GameObject m_textPrefab;
    public GameObject m_buttonPrefab;
    public GameObject m_inputField;
    public GameObject m_cancelPrefab;

    private NetworkStates m_states;
    private RoomInfo[] rooms;
    private List<roomList> roomRows;

    private bool updateList = false;

    private List<GameObject> m_createInput;
    private InputField m_input;
    private UnityAction m_action;

    private void Start()
    {
        m_states = FindObjectOfType<NetworkStates>();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.F5) && !updateList)
        {
            UpdateRoomList();
        }
        if (Input.GetKey(KeyCode.Escape))
        {
            SelectionScreen();
        }
        if (m_createInput != null)
        {

        }
    }

    public void UpdateRoomList()
    {
        updateList = true;
        rooms = PhotonNetwork.GetRoomList();
        if (roomRows != null)
        {
            foreach(roomList room in roomRows)
            {
                Destroy(room.name.gameObject);
                Destroy(room.number.gameObject);
                Destroy(room.availability.gameObject);
                Destroy(room.join);
            }
        }
        roomRows = new List<roomList>();
        foreach (RoomInfo info in rooms)
        {
            roomList newRoom = new roomList();
            GameObject newName = Instantiate(m_textPrefab, m_RoomCanvas.transform);
            newRoom.name = newName.GetComponent<Text>();
            newRoom.name.text = info.Name;
            GameObject newNumber = Instantiate(m_textPrefab, m_RoomCanvas.transform);
            newRoom.number = newNumber.GetComponent<Text>();
            newRoom.number.text = info.PlayerCount.ToString();
            GameObject newAvailability = Instantiate(m_textPrefab, m_RoomCanvas.transform);
            newRoom.availability = newAvailability.GetComponent<Text>();
            newRoom.availability.text = info.IsOpen.ToString();
            newRoom.join = Instantiate(m_buttonPrefab, m_RoomCanvas.transform);
            newRoom.join.GetComponentInChildren<Text>().text = "Join";
            newRoom.join.name = info.Name;
            roomRows.Add(newRoom);
        }
        updateList = false;
    }

    public void SinglePlayer()
    {
        m_states.currentState = NetworkStates._NetworkState.Single;
    }

    public void MultiPlayer()
    {
        m_Selection.SetActive(false);
        m_RoomCanvas.SetActive(true);
        m_states.currentState = NetworkStates._NetworkState.Multi;
        InvokeRepeating("UpdateRoomList", 0, 10);
    }

    public void SelectionScreen()
    {
        CancelInvoke("UpdateRoomList");
        m_Selection.SetActive(true);
        m_RoomCanvas.SetActive(false);
    }

    public void CreateRoom()
    {
        m_createInput = new List<GameObject>();
        GameObject canvas = new GameObject("CreateGameCanvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        canvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.AddComponent<VerticalLayoutGroup>();
        canvas.GetComponent<VerticalLayoutGroup>().childAlignment = TextAnchor.MiddleCenter;
        GameObject topCanvas = new GameObject("TopGameCanvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        topCanvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        topCanvas.transform.parent = canvas.transform;
        GameObject bottomCanvas = new GameObject("BotGameCanvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        bottomCanvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        bottomCanvas.transform.parent = canvas.transform;
        bottomCanvas.AddComponent<HorizontalLayoutGroup>();
        bottomCanvas.GetComponent<HorizontalLayoutGroup>().childAlignment = TextAnchor.MiddleCenter;
        bottomCanvas.GetComponent<HorizontalLayoutGroup>().spacing = 100;
        bottomCanvas.GetComponent<HorizontalLayoutGroup>().childControlHeight = false;
        bottomCanvas.GetComponent<HorizontalLayoutGroup>().childControlWidth = false;
        bottomCanvas.GetComponent<HorizontalLayoutGroup>().childForceExpandHeight = false;
        bottomCanvas.GetComponent<HorizontalLayoutGroup>().childForceExpandWidth = false;
        GameObject input = Instantiate(m_inputField);
        input.transform.parent = topCanvas.transform;
        input.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
        input.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
        input.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        m_input = input.GetComponent<InputField>();
        GameObject enter = Instantiate(m_buttonPrefab);
        enter.transform.parent = bottomCanvas.transform;
        enter.GetComponentInChildren<Text>().text = "Create";
        GameObject cancel = Instantiate(m_cancelPrefab);
        cancel.transform.parent = bottomCanvas.transform;
        cancel.GetComponentInChildren<Text>().text = "Cancel";
        m_createInput.Add(canvas);
        m_createInput.Add(topCanvas);
        m_createInput.Add(bottomCanvas);
        m_createInput.Add(input);
        m_createInput.Add(enter);
        m_createInput.Add(cancel);

        
    }

    public void ConfirmRoom(string name)
    {
        m_states.CreateRoom(name);
    }

    public void cancel()
    {
        foreach(GameObject obj in m_createInput)
        {
            if (obj != null)
            {
                Destroy(obj);
            }
            m_createInput.Remove(obj);
        }
    }

    public void JoinGame(GameObject button)
    {
        m_states.JoinRoom(button.name);
    }
    
}
