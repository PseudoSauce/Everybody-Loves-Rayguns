using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Networking {
	public class Playground : MonoBehaviour {
		#region Variables
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
		}

		void Start() {
			Connect();	
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
