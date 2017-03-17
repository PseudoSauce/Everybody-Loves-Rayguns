using System.Collections;
using UnityEngine;
using MyTypes;

[RequireComponent(typeof(Rigidbody))]
public class ExampleBombInteractable : Interactable {
    [Header("My Stuff")]
    [SerializeField] GameObject explosionParticles;
    [SerializeField] float m_delay = 5.0f;

    ////////////////////////////////
    //     Interactable Stuff 
    ///////////////////////////////
	protected override void Init()
    { 
        AssignInteractionType(Interaction.EXAMPLEBOMB);

        // assigns required for this particular interactable
        AssignStart(MyStart);

        // required right now for both sending and receiving events.
        // use the EventBeacon do send, check received events, etc.
        AssignCustomEventReceiveNotify(ReceiveCustomEvent, ReceiveManagerEvent);
    }

    private void MyStart()
    {
        StartCoroutine(BombTick());
    }

    ////////////////////////////////
    //     Event Stuff
    ///////////////////////////////
    private void ReceiveCustomEvent(CustomEventPacket handlerPacket)
    {

    }

    // check against "reserved" id
    private void ReceiveManagerEvent(ICustomEventManagerHandler handler)
    {

    }

    ////////////////////////////////
    //     Other Stuff
    ///////////////////////////////
    private IEnumerator BombTick()
    {
        yield return new WaitForSeconds(m_delay);

        Explode();
    }

    private void Explode()
    {
        print("Kaboom!!11");
        Instantiate(explosionParticles, transform);

        // invoking an event with the beacon
        EventBeacon.InvokeEvent(new DoorOpenEvent());

        Destroy(gameObject);
    }
}
