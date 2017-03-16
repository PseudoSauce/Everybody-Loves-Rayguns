using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObserverScript : MonoBehaviour, ICustomEventObserver {
    [SerializeField] private CustomEventManager m_manager;
    [SerializeField] private uint m_eventID;

    private void Update()
    {
        // register the observer for the specific event
        if (Input.GetKeyDown(KeyCode.R))
        {
            m_manager.RegisterEvent(this, m_eventID);
        }
        // deregister this particular observer from a specific event
        else if (Input.GetKeyDown(KeyCode.D))
        {
            m_manager.DeregisterFromEvent(this, m_eventID);
        }
        // deregister the event completely (includes for other observers)
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            m_manager.DeregisterEvent(m_eventID);
        }
    }
    
    // requirement of an event observer... respond to an event with this
    public void Notify(ICustomEventHandler handler)
    {
        // we should know the type so...
        if (handler is InvokerNamedEventHandler)
        {
            InvokerNamedEventHandler eventHandler = (InvokerNamedEventHandler)handler;

            print(name + ": EventID(" + eventHandler.EventID + "): My invoker's name is " + eventHandler.InvokerName);
        }
        else
        {
            print("fdaposifjpaoijsd");
        }
    }
}
