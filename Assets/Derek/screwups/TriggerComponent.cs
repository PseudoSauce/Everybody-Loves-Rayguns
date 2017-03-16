using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyTypes;

[RequireComponent(typeof(MessengerComponent))]
public class TriggerComponent : Interactable {

    protected override void Init()
    {
        AssignInteractionType(Interaction.TRIGGER);
    }

    // place your custom logic here for interaction
    protected override void Commit(InteractMessage msg)
    {
        if (msg.msg == "Trigger")
        {
            gameObject.transform.localScale *= 5;
            Debug.Log(name + ": COMPLETE!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Player")
        {
            InteractMessage message = new InteractMessage(Interaction.MESSENGER, "Start", 3);
            SendMessage("Interact", message);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name == "Player")
        {
            InteractMessage message = new InteractMessage(Interaction.MESSENGER, "Stop", 3);
            SendMessage("Interact", message);
        }
    }
}
