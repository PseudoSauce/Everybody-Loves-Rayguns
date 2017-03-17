using System.Collections;
using UnityEngine;
using MyTypes;

[RequireComponent(typeof(Rigidbody))]
public class ExampleDoorInteractable : Interactable {
    [Header("My Stuff")]
    [SerializeField] float m_bombForce = 1000.0f;

    private Rigidbody body;

    ////////////////////////////////
    //     Interactable Stuff 
    ///////////////////////////////
	protected override void Init()
    { 
        AssignInteractionType(Interaction.EXAMPLEBOMB);
        AssignStart(MyStart);

        // required right now for both sending and receiving events.
        // use the EventBeacon do send, check received events, etc.
        AssignCustomEventReceiveNotify(ReceiveCustomEvent, ReceiveManagerEvent);
    }

    private void MyStart()
    {
        // registering event for interactable to trigger
        EventBeacon.RegisterEvents((uint)CustomEventExamples.DoorOpenEvent);
        body = GetComponent<Rigidbody>();
    }

    ////////////////////////////////
    //     Event Stuff
    ///////////////////////////////
    private void ReceiveCustomEvent(CustomEventPacket handlerPacket)
    {
        var handler = handlerPacket.Handler;
        var eventID = handler.EventID;

        if ((CustomEventExamples)eventID == CustomEventExamples.DoorOpenEvent)
        {
            EatExplosion();
        }
    }

    // check against "reserved" id
    private void ReceiveManagerEvent(ICustomEventManagerHandler handler)
    {

    }

    ////////////////////////////////
    //     Other Stuff
    ///////////////////////////////
    private void EatExplosion()
    {
        print(name + ": eating explosion!");

        EventBeacon.InvokeEvent(new ExampleEnemyAlertEventHandler());

        Destroy(gameObject);
    }
}
