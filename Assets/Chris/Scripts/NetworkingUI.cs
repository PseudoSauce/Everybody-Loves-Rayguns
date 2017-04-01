using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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

    private NetworkStates m_states;
    private RoomInfo[] rooms;
    private List<roomList> roomRows;

    private bool updateList = false;

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

    public void MultiPlayer()
    {
        m_Selection.SetActive(false);
        m_RoomCanvas.SetActive(true);
        InvokeRepeating("UpdateRoomList", 0, 10);
    }

    public void SelectionScreen()
    {
        CancelInvoke("UpdateRoomList");
        m_Selection.SetActive(true);
        m_RoomCanvas.SetActive(false);
    }

    public void JoinGame(GameObject button)
    {
        m_states.JoinRoom(button.name);
    }
}
