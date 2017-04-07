using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon;
using UnityStandardAssets.Characters.FirstPerson;

// Note: Make sure client settings is set on Auto-Join Lobby ON
public class NetworkStates : PunBehaviour {

    public enum _NetworkState
    {
        Single = 0,
        Multi = 1,
    }

    private NetworkingUI m_networkUI;

    // TODO: Change this state using UI to select which state to be in
    public _NetworkState currentState = _NetworkState.Single;     // Testing purpose
    private byte maxPlayers = 2;
    private new PhotonView photonView;
    private RoomInfo[] rooms;

    private void Awake() {
        m_networkUI = GetComponent<NetworkingUI>();
    }

    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings("0.1");
    }

    private void OnPhotonRandomJoinFailed()
    {
        Debug.Log("Failed to join a random room");
        if(currentState == _NetworkState.Multi)
        {
            Debug.Log("Creating new room...");
            PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = maxPlayers }, null);
        }
    }

    public void JoinRoom(string name)
    {
        PlayGame();
        PhotonNetwork.JoinRoom(name);
    }

    public void CreateRoom(string name)
    {
        PlayGame();
        PhotonNetwork.CreateRoom(name, new RoomOptions() { MaxPlayers = maxPlayers }, null);
    }

    public override void OnJoinedLobby()
    {
        // Testing purpose just join a random room
        //PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinedRoom()
    {
        //GameObject player = PhotonNetwork.Instantiate("Player", new Vector3(0, 1, 0), Quaternion.identity, 0);
        //player.GetComponent<RigidbodyFirstPersonController>().isControllable = true;
        //player.GetComponent<RigidbodyFirstPersonController>().cam.enabled = true;
        //photonView = player.GetComponent<PhotonView>();
    }

    public void DisableNetworkUI() {
        m_networkUI.enabled = false;
    }

    [PunRPC]
    private void PlayGame() {
        DisableNetworkUI();
        PhotonNetwork.LoadLevel(1);
    }
}
