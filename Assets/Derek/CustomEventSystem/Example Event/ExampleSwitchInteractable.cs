using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyTypes;

public class ExampleSwitchInteractable : Interactable {
    static int switchNumber = 1;
    bool wasTriggered = false;
    
    ////////////////////////////////
    //     Interactable Stuff 
    ///////////////////////////////
	protected override void Init()
    {
        AssignInteractionType(Interaction.EXAMPLEBOMB);

        // assigns required for this particular interactable
        AssignCustomEventReceiveNotify(ReceiveCustomEvent, ReceiveManagerEvent);
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
    private void OnTriggerEnter(Collider other)
    {
        if (!wasTriggered && other.name == "Player")
        {
            TriggerChainOfEvents();
        }
    }

    private void TriggerChainOfEvents()
    {
        wasTriggered = true;
        var bombTrigger = new BombTriggerEvent();
        bombTrigger.switchNumber = switchNumber;
        switchNumber++;

        // invoking the event
        EventBeacon.InvokeEvent(new BombTriggerEvent());
    }
}
