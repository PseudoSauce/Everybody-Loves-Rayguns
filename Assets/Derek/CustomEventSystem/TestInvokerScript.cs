using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// example handler for an event... is really just a message.
// you can technically add any functionality you want. even delegates for chaining events
// even further. or perhaps a coroutine? a purple dragon spawner?
// the Observer will take this message forwarded by the event manager and do whatever...
// in this case, just say this invoker's gameobject name : P
// it's up to the implementer of an observer to decide the result (and the implementer of the specific "handler").
// this invoker is essentially directly decoupled. you must still know what message to send through a particular eventid,
// otherwise potentially bad things can happen... most likely nothing!
// -------------------------------------------------------------------------------------------------------
//         -----all event handlers must implement ICustomEventHandler!------
//             ** EventID(uint) is the only requirement of a handler **
// ------------------------------------------------------------------------------------------------------
struct InvokerNamedEventHandler : ICustomEventHandler
{
    public InvokerNamedEventHandler(uint eventID, string name)
    {
        this.name = name;
        this.eventID = eventID;
    }

    private uint eventID;           // ... exactly what it means
    private string name;

    public string InvokerName { get { return name; } }
    public uint EventID { get { return eventID; } }    
}

// implements the invoker interface to be able to send messages to
// event listeners
public class TestInvokerScript : MonoBehaviour, ICustomEventInvoker {
    [SerializeField] private CustomEventManager m_manager;
    [SerializeField] private uint m_eventID;
	
	// Update is called once per frame
	void Update () {
	}
}
