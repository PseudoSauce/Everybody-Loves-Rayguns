using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon;
using UnityStandardAssets.Characters.FirstPerson;

public class NetworkStates : PunBehaviour {

    public enum _NetworkState
    {
        Single = 0,
        Multi = 1,
    }

    // TODO: Change this state using UI to select which state to be in
    private _NetworkState currentState = _NetworkState.Single;     // Testing purpose
    private byte maxPlayers = 2;
    private new PhotonView photonView;
    private RoomInfo[] rooms;

    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings("0.1");
    }

    private void Update()
    {

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
        PhotonNetwork.JoinRoom(name);
    }

    public override void OnJoinedLobby()
    {
        // Testing purpose just join a random room
        //PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinedRoom()
    {
        GameObject player = PhotonNetwork.Instantiate("Player", new Vector3(0, 1, 0), Quaternion.identity, 0);
        player.GetComponent<RigidbodyFirstPersonController>().isControllable = true;
        player.GetComponent<RigidbodyFirstPersonController>().cam.enabled = true;
        photonView = player.GetComponent<PhotonView>();

    }
}
