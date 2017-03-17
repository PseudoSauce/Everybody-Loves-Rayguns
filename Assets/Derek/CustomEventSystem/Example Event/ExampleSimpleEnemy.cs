using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyTypes;

public class ExampleSimpleEnemyObserver : Interactable {
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

    }

    protected override void Commit(InteractMessage msg)
    {
    }

    private void MyUpdate(float deltaTime)
    {

    }

    ////////////////////////////////
    //     Custom Event Stuff 
    ///////////////////////////////
    private void ReceiveCustomEvent(CustomEventPacket handlerPacket)
    {

    }

    private void ReceiveManagerEvent(ICustomEventManagerHandler handler)
    {

    }

    ////////////////////////////////
    //     Other Stuff
    ///////////////////////////////
}
