using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeaconTestObject : MonoBehaviour {

    void OnTriggerStay()
    {
        Destroy(this.gameObject);
    }

    public void SetColliderSize(Vector3 size)
    {
        transform.localScale = size;
    }


}
