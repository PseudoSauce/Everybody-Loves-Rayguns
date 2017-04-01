using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

public class NRandomMatch : PunBehaviour
{
    public Transform[] m_spawnPoints;
    public byte m_MaxPlayers = 2;

    private Vector3 playerPos = Vector3.zero;
    private Quaternion playerRot = Quaternion.identity;

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings("0.1");
    }

    void Update()
    {
        if(!photonView.isMine)
        {
            transform.position = Vector3.Lerp(transform.position, playerPos, Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, playerRot, Time.deltaTime);
        }
    }

    void OnGUI()
    {
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // Sending packets
        if (stream.isWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }

        // Receiving packets
        else
        {
            playerPos = (Vector3)stream.ReceiveNext();
            playerRot = (Quaternion)stream.ReceiveNext();
        }
    }

    void OnPhotonRandomJoinFailed()
    {
        Debug.Log("Failed to join a random room");
        Debug.Log("Creating new room...");
        PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = m_MaxPlayers }, null);
    }

    public override void OnJoinedLobby()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinedRoom()
    {
        // Any prefabs that needs to be instantiated using photon network needs to be inside the resources folder
        GameObject player = PhotonNetwork.Instantiate("Player", new Vector3(0, 1, 0), Quaternion.identity, 0);
    }
}
