using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Networking {
	public class Playground : Photon.PunBehaviour {
        #region Variables
        // The PUN loglevel
        public PhotonLogLevel m_LogLevel = PhotonLogLevel.Informational;
        // Maximum number of players per room
        public byte m_MaxPlayers = 4;

		// This client's version number. Users are separated from each other by gameversions
		private string m_Version = "1";
		#endregion

		#region Monobehaviour CallBacks
		void Awake() {
			// There is no need to join a lobby to get the list of rooms
			PhotonNetwork.autoJoinLobby = false;
			// This makes sure we can use PhotonNetwork.LoadLevel()
			// on master client and all clients in the same room sync their level automatically
			PhotonNetwork.automaticallySyncScene = true;
            // Force Photon Log level
            PhotonNetwork.logLevel = m_LogLevel;
		}

		void Start() {
            Connect();
		}
        #endregion

        #region Photon.PunBehaviour Callbacks
        /// <summary>
        /// Called after the connection to the master is established and authenticated but only when PhotonNetwork.autoJoinLobby is false
        /// </summary>
        public override void OnConnectedToMaster() {
            Debug.Log("OnConnectedToMaster was called by PUN");
        }

        /// <summary>
        /// Called after disconnecting from server
        /// </summary>
        public override void OnDisconnectedFromPhoton() {
            Debug.Log("OnDisconnectedFromPhoton was called by PUN");
        }

        /// <summary>
        /// Called when CreateRoom() failed
        /// </summary>
        /// <param name="codeAndMsg">[0] is error code. [1] is error string msg</param>
        public override void OnPhotonCreateRoomFailed(object[] codeAndMsg) {
            Debug.Log("OnPhotonCreateRoomFailed was called by PUN");
            PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = m_MaxPlayers }, null);
        }

        /// <summary>
        /// Called when creating/joining a room. Called on all clients including master client
        /// </summary>
        public override void OnJoinedRoom() {
            Debug.Log("OnJoinedRoom was callled by PUN");
        }
        #endregion

        #region Public Methods
        public void Connect() {
			if (PhotonNetwork.connected) {
				// Attempt to join a random room. If it fails, we'll get notified and create one
				PhotonNetwork.JoinRandomRoom();
			}
			else {
				// Connect 
				PhotonNetwork.ConnectUsingSettings(m_Version);
			}
		}
        #endregion
    }
}
