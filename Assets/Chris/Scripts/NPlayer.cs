using UnityEngine;

public class NPlayer : Photon.MonoBehaviour {

    private Vector3 playerPos = Vector3.zero;
    private Quaternion playerRot = Quaternion.identity;

    private void Update()
    {
        if(!photonView.isMine)
        {
            transform.position = Vector3.Lerp(transform.position, playerPos, Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, playerRot, Time.deltaTime);
        }
    }

    private void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.isWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            playerPos = (Vector3)stream.ReceiveNext();
            playerRot = (Quaternion)stream.ReceiveNext();
        }
    }
}
