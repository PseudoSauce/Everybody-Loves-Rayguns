using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyTypes;

public class ExampleSimpleEnemy : Interactable {
    [SerializeField] Transform m_pointToMoveTo;
    [SerializeField] string m_messageToSayOnAlert;

    private bool m_isTriggered;

    ////////////////////////////////
    //     Interactable Stuff 
    ///////////////////////////////
	protected override void Init()
    { 
        AssignInteractionType(Interaction.EXAMPLEENEMYOBSERVER);

        // assigns required for this particular interactable
        AssignStart(MyStart);
        AssignUpdate(MyUpdate);
        AssignCustomEventReceiveNotify(ReceiveCustomEvent, ReceiveManagerEvent);
    }

    private void MyStart()
    {
        EventBeacon.RegisterEvents((uint)CustomEventExamples.EnemyAlertEvent);
    }

    private void MyUpdate(float deltaTime)
    {
        if (m_isTriggered)
        {
            transform.position = Vector3.MoveTowards(gameObject.transform.position, m_pointToMoveTo.position, 0.5f);
            print(m_messageToSayOnAlert);
        }
    }

    ////////////////////////////////
    //     Custom Event Stuff 
    ///////////////////////////////
    private void ReceiveCustomEvent(CustomEventPacket handlerPacket)
    {
        var handler = handlerPacket.Handler;
        var eventID = handler.EventID;

        if ((CustomEventExamples)eventID == CustomEventExamples.EnemyAlertEvent);
        {
            m_isTriggered = true;
        }
    }

    private void ReceiveManagerEvent(ICustomEventManagerHandler handler)
    {

    }

    ////////////////////////////////
    //     Other Stuff
    ///////////////////////////////
}
