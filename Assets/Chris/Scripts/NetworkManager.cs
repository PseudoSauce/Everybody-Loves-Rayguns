using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon;
using UnityStandardAssets.Characters.FirstPerson;

/// <summary>
/// Network Manager singleton
/// Note: Make sure client settings is set on Auto-Join Lobby ON
/// </summary>
namespace Network {
    public class NetworkManager : PunBehaviour {

        public enum _NetworkState {
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

        private void Start() {
            PhotonNetwork.ConnectUsingSettings("0.1");
        }

        private void OnPhotonRandomJoinFailed() {
            Debug.Log("Failed to join a random room");
            if (currentState == _NetworkState.Multi) {
                Debug.Log("Creating new room...");
                PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = maxPlayers }, null);
            }
        }

        public void JoinRoom(string name) {
            PlayGame();
            PhotonNetwork.JoinRoom(name);
        }

        public void CreateRoom(string name) {
            PlayGame();
            PhotonNetwork.CreateRoom(name, new RoomOptions() { MaxPlayers = maxPlayers }, null);
        }

        public override void OnJoinedLobby() {
            // Testing purpose just join a random room
            //PhotonNetwork.JoinRandomRoom();
        }

        public override void OnJoinedRoom() {
            GameObject player = PhotonNetwork.Instantiate("Player 1", GameObject.Find("SpawnPoint").transform.position, Quaternion.identity, 0);
            if (PhotonNetwork.player.ID == 1) {
                player.name = "Host";
            } else {
                player.name = "Player";
            }
            RigidbodyFirstPersonController controller = player.GetComponent<RigidbodyFirstPersonController>();
            GameObject[] camera = GameObject.FindGameObjectsWithTag("MainCamera");
            foreach (GameObject cams in camera) {
                if (cams) {
                    cams.tag = "";
                    cams.SetActive(false);
                }
            }
            controller.enabled = true;
            controller.cam.gameObject.SetActive(true);
            controller.cam.tag = "MainCamera";
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
}