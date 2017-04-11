using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// GameManager associated with Networking
/// </summary>
namespace Network {
    public class NGameManager : SingletonClass<NGameManager> {
        public GameObject m_Player;

        private NetworkManager m_NetworkManager;

        private void Awake() {
            m_NetworkManager = FindObjectOfType<NetworkManager>();
        }

        private void Start() {
            if (m_NetworkManager.currentState == NetworkManager._NetworkState.Single) {
                m_Player.SetActive(true);
            }
        }
    }
}