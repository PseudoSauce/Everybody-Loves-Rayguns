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

public class InvokerScript : MonoBehaviour {
    [SerializeField] private CustomEventManager m_manager;
    [SerializeField] private uint m_eventID, m_eventID2;
	
	// Update is called once per frame
	void Update () {
	    if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            // you can check if an event is registered... 
            // notifyobservers also returns whether it was successful, for convenience
            if (m_manager.IsEventRegistered(m_eventID))       // --> if statement not really necessary...
            {
                // notify all observers of an event with this particular handle
                m_manager.NotifyObservers(new InvokerNamedEventHandler(m_eventID, name));
            }
            else
            {
                print(this + ": EventID(" + m_eventID + ") is not currently registered.");
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            // you can check if an event is registered... 
            // notifyobservers also returns whether it was successful, for convenience
            if (m_manager.IsEventRegistered(m_eventID2))       // --> if statement not really necessary...
            {
                // notify all observers of an event with this particular handle
                m_manager.NotifyObservers(new InvokerNamedEventHandler(m_eventID2, name));
            }
            else
            {
                print(this + ": EventID(" + m_eventID + ") is not currently registered.");
            }
        }
	}
}
