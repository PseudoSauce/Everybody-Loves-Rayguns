using System;
using System.Collections.Generic;
using UnityEngine;

// each event is associated with a unique id that is manually set
// through the editor
using EventID = System.UInt32;
using InvokerID = System.UInt32;

// weak references to prevent memory leaks
using ObserverList = System.Collections.Generic.LinkedList<System.WeakReference>;
using InvokerRef = System.WeakReference;
using ObserverRef = System.WeakReference;

///////////////////////////////////////////////////////////
// these are reserved event IDs custom handlers
//////////////////////////////////////////////////////////
enum ReservedEventID : EventID
{
    // I'm lazy so using powers of 2 : P
    ObserverRegistered = 1<<0x1B,
    DeregisteredAll = 1<<0x1C,
    DeregisteredObserver = 1<<0x1D,
    DeregisteredEvent = 1<<0x1E    
}

public interface ICustomEventManagerHandler : ICustomEventHandler
{
}

// sent out to all observers of the specific event
// when an event is deregistered
struct ReservedEventDeregistered : ICustomEventManagerHandler
{
    public ReservedEventDeregistered(EventID associatedEvent)
    {
        this.associatedEvent = associatedEvent;
    }

    private EventID associatedEvent;
    public EventID EventIDDeregistered
    {
        get { return associatedEvent; }
    }

    public EventID EventID
    {
        get { return (EventID)ReservedEventID.DeregisteredEvent; }
    }
}

// sent out to all observers of the specific event
// when an event is deregistered
struct ReservedEventObserverDeregistered : ICustomEventManagerHandler
{
    public ReservedEventObserverDeregistered(EventID associatedEvent)
    {
        this.associatedEvent = associatedEvent;
    }

    private EventID associatedEvent;
    public EventID EventIDDeregistered
    {
        get { return associatedEvent; }
    }

    public EventID EventID
    {
        get { return (EventID)ReservedEventID.DeregisteredObserver; }
    }
}

// sent out to all observers of the specific event
// when an observer registers
struct ReservedEventObserverRegistered : ICustomEventManagerHandler
{
    public ReservedEventObserverRegistered(EventID associatedEvent)
    {
        this.associatedEvent = associatedEvent;
    }

    private EventID associatedEvent;
    public EventID EventIDDeregistered
    {
        get { return associatedEvent; }
    }

    public EventID EventID
    {
        get { return (EventID)ReservedEventID.ObserverRegistered; }
    }
}

// sent out to all observers of the specific event
// when an event is deregistered
struct ReservedEventAllEventsDeregistered : ICustomEventManagerHandler
{
    public EventID EventID
    {
        get { return (EventID)ReservedEventID.DeregisteredAll; }
    }
}
///////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////

// used to be able to associate an id with the handler, without putting
// the responsibility on the invoker.
public struct CustomEventPacket
{
    public CustomEventPacket(ICustomEventHandler handler, InvokerID invokerID)
    {
        this.handler = handler;
        this.invokerID = invokerID;
    }

    private ICustomEventHandler handler;
    private InvokerID invokerID;

    public InvokerID Invoker
    {
        get
        {
            return invokerID;
        }
    }

    public ICustomEventHandler Handler
    {
        get
        {
            return handler;
        }
    }
}

/*
 * Manages all the events observers register to. When an invoker calls "notify observers",
 * every observer listening to that event is invoked.
 * The event manager can also invoke events (will use reserved event IDs)
 */
public sealed class CustomEventManager : MonoBehaviour, ICustomEventInvoker {
    private Dictionary<EventID, ObserverList> m_customEvents;
    private Dictionary<InvokerRef, InvokerID> m_invokerIDList;
    
    private void Awake()
    {
        m_customEvents = new Dictionary<EventID, ObserverList>();
        m_invokerIDList = new Dictionary<InvokerRef, EventID>();
    }

    #region properties
    public int Count { get { return m_customEvents.Count; } }
    public int TotalRegisteredObservers
    {
        get
        {
            int count = 0;

            foreach (var customEvent in m_customEvents)
            {
                count += customEvent.Value.Count;
            }

            return count;
        }
    } 
    #endregion

    #region sanitychecks
    public bool IsEventRegistered(EventID id)
    {
        return m_customEvents.ContainsKey(id) 
            && m_customEvents[id].Count > 0;
    }

    // returns the amount of registered observers for an event
    public int RegisteredObserversForEvent(EventID id)
    {
        return m_customEvents.ContainsKey(id) ? m_customEvents[id].Count : 0;
    }
    
    // returns whether an observer is apart of an event... obviously ; )
    public bool IsApartOfEvent(ICustomEventObserver observer, EventID id)
    {
        ObserverList customEvent = null;
        bool containsEvent = m_customEvents.TryGetValue(id, out customEvent);

        bool eventContainsObserver = false;

        // check if the located event contains the observer
        if (containsEvent)
        {
            foreach (var value in m_customEvents[id])
            {
                if (value.IsAlive && value.Target == observer)
                {
                    eventContainsObserver = true;
                    break;
                }
            }
        }        

        return eventContainsObserver;
    }
    #endregion

    #region registration
    // register an observer to an event specified by the event id.
    public bool RegisterEvent(ICustomEventObserver observer, EventID id, bool clearEventIfPreviouslyOccupied = false)
    { 
        // the event is reserved by the manager, so the registration fails...
        if (IsEventIDReserved(id))
        {
            print("EventID(" + id + ") is reserved for the event manager. Cannot register event.");
            return false;
        }
        
        // create the event list if it does not already exist for this eventID.
        // this event ID may still be left over from before.
        if (!m_customEvents.ContainsKey(id))
        {
            m_customEvents.Add(id, new ObserverList());
        }

        // clear the event before registering if specified. defaults to true.
        if (clearEventIfPreviouslyOccupied && m_customEvents[id].Count>0)
        {
            print(name + ": Clearing the EventID, " + id + " before registering.");
            m_customEvents[id].Clear();
        }

        // add the observer to the event if it's not currently included
        bool wontBeDuplicate = true;
        foreach (var o in m_customEvents[id])
        {
            if (o.Target == observer)
            {
                wontBeDuplicate = false;
                break;
            }
        }

        // add the observer to the event if it is not currently included
        if (wontBeDuplicate)
        {
            NotifyObservers(this, new ReservedEventObserverRegistered(id));
            m_customEvents[id].AddLast(new ObserverRef(observer));
        }

        return true;
    }

    // deregister a specific observer from an event specified by id.
    // this is superior to "Deregister Event" if there is potential the event id may be reused,
    // as the linked list will be spared (if you are sure that there are no more associations with
    // this id... aka. IsEventRegistered. or you can check the count of an event using RegisteredObservers
    public void DeregisterFromEvent(ICustomEventObserver observer, EventID id)
    {
        ObserverList eventObservers = null;
        bool containsEvent = m_customEvents.TryGetValue(id, out eventObservers);

        // if the event exists, iterate through each associated observer
        // and remove the observer if it is apart of the event
        if (containsEvent)
        {
            foreach (var o in eventObservers)
            {
                if (o.IsAlive && o.Target == observer)
                {
                    bool removed = m_customEvents[id].Remove(o);

                    // notify all observers of an event that an observer has
                    // deregistered
                    if (removed)
                    {
                        NotifyObservers(this, new ReservedEventObserverDeregistered(id));
                    }

                    break;
                }
            }
        }
    }

    // obliterates an event.
    // only do if necessary, as the associated linked list is thrown to garbage collection.
    // an alternative is to make sure each observer is deregistered for an event.
    // you can check the count for a specific event.
    public bool DeregisterEvent(EventID id)
    {   
        if (m_customEvents.ContainsKey(id))
        {
            NotifyObservers(this, new ReservedEventDeregistered(id));
            m_customEvents.Remove(id);
            
            return true;            
        }

        return false;
    }

    // deregister every registered event and notify "everyone" of this occurance
    public void DeregisterAllEvents()
    {
        NotifyAllObservers(new ReservedEventAllEventsDeregistered());
    }
    #endregion registration

    // called by "invoker" to trigger an event.
    // returns true if at least one observer was notified.
    // the invoker is stored with a unique ID that is attached to sent with handlers.
    public bool NotifyObservers(ICustomEventInvoker invoker, ICustomEventHandler handle)
    {
        bool observersWereNotified = false;

        ObserverList observers = null;
        bool eventExists = m_customEvents.TryGetValue(handle.EventID, out observers);

        // for each observer associated with the event,
        // forward the handle event
        if (eventExists && observers.Count > 0)
        {
            foreach (var o in observers)
            {
                if (o.IsAlive)
                {
                    ICustomEventObserver observer = o.Target as ICustomEventObserver;                    

                    if (observer != null)
                    {
                        InvokerID id = AssignInvokerID(invoker);

                        observer.ReceiveNotify(new CustomEventPacket(handle, id));
                    }
                }
            }

            observersWereNotified = true;
        }

        return observersWereNotified;
    }

    #region internal
    // notify all observers of a particular system event
    private void NotifyAllObservers(ICustomEventManagerHandler handler)
    {
        var packet = new CustomEventPacket(handler, AssignInvokerID(this));

        foreach (var customEvent in m_customEvents)
        {
            foreach (var o in m_customEvents[customEvent.Key])
            {
                if (o.IsAlive)
                {
                    var observer = (ICustomEventObserver)o.Target;
                    observer.ReceiveNotify(packet);
                }
            }
        }
    }

    // once an invoker is added, it will continue in the list, even
    // if the original event was discarded. ultimately this allows
    // faster event invocation, if an event is triggered later
    // by the same invoker
    private InvokerID AssignInvokerID(ICustomEventInvoker invoker)
    {
        bool containsInvoker = false;
        InvokerID invokerID = 0;

        // make sure the invoker id does not currently exist already
        foreach (var o in m_invokerIDList)
        {
            var key = o.Key;

            if (key.Target != null && key.Target == invoker)
            {
                invokerID = m_invokerIDList[key];
                containsInvoker = true;
            }
        }   
        // if the invoker is not currently registered, add them to the
        // "list"
        if (!containsInvoker)
        {
            int invokerCount = m_invokerIDList.Count;      
            invokerID = (InvokerID)invokerCount;
            m_invokerIDList.Add(new InvokerRef(invoker), invokerID);
        }

        return invokerID;
    }

    // very slow...
    private void ClearEmptyReferences()
    {
        var toBeRemoved = new Dictionary<EventID, ObserverList>();

        // locate the empty references
        foreach (var id in m_customEvents.Keys)
        {
            toBeRemoved.Add(id, new ObserverList());

            foreach (var observer in m_customEvents[id])
            {
                if (!observer.IsAlive)
                {
                    toBeRemoved[id].AddLast(observer);
                }
            }
        }
        // clear the dictionary of empty references
        foreach (var id in toBeRemoved.Keys)
        {
            foreach (var observer in toBeRemoved[id])
            {
                m_customEvents[id].Remove(observer);
            }
        }
    }
    #endregion

    #region static
    // whether this received handler is a notification from this manager
    public static bool IsManagerHandler(ICustomEventHandler handler)
    {
        return handler.GetType() == typeof(ICustomEventManagerHandler);
    }

    // whether a specific event id is reserved by this system
    public static bool IsEventIDReserved(EventID id)
    {
        return Enum.IsDefined(typeof(ReservedEventID), id);
    }
    #endregion
}
