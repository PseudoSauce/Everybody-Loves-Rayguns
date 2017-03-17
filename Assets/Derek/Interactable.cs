using System;
using System.Collections.Generic;
using UnityEngine;
using MyTypes;

using EventID = System.UInt32;

// inherit this for your interaction.
// all unity API based functions happen through this class.
// please override the Init function, and initialize your own custom
// Update in it. (Call AssignUpdate to be recognized for unity protocol updates)

// ***************************************************************************************
// every single interactable can be considered an invoker and/or an observer for the
// custom event system. you can choose to ignore that functionality for your interactable.
// you are also still welcome to branch away from using interactables for events though,
// as it does not matter ultimately what an invoker or observer is.
// ***************************************************************************************

public class Interactable : MonoBehaviour {
    // this class acts as the beacon for all events associated with the custom event manager.
    protected sealed class CustomEventBeacon : ICustomEventInvoker, ICustomEventObserver
    {    
        private WeakReference m_customEventManager;
        private WeakReference m_interactable;

        private LinkedList<EventID> m_eventsSubscribedTo;

        private bool m_isInitialized;

        internal CustomEventBeacon(Interactable eventReceiver, CustomEventManager eventManager)
        {
            m_customEventManager = new WeakReference(eventManager);
            m_interactable = new WeakReference(eventReceiver);
            m_eventsSubscribedTo = new LinkedList<EventID>();

            if (eventReceiver && eventManager)
            {
                m_isInitialized = true;
            }
        }

        #region Event Stuff
        // request an event to occur using a handler defined with
        // the appropriate event id.
        // returns whether the request was sent successfully to at least one
        // of the observers.
        // this does not necessarely suggest that the event was successful,
        // since the event requirements are defined individually.
        public bool NotifyEvent(ICustomEventHandler customHandler)
        {
            if (IsActive)
            {
                var manager = (CustomEventManager)m_customEventManager.Target;
                return manager.NotifyObservers(this, customHandler);
            }

            return false;
        }

        // register to the events specified on the 
        public bool RegisterEvents(params EventID[] requestedEvents)
        {
            // subscribe to events requested by the interactable
            if (IsActive)
            {
                var manager = (CustomEventManager)m_customEventManager.Target;

                foreach (var customEvent in requestedEvents)
                {
                    // register to (and this) event if not currently
                    // apart of it
                    if (!manager.IsApartOfEvent(this, customEvent))
                    {
                        m_eventsSubscribedTo.AddLast(customEvent);
                        return manager.RegisterEvent(this, customEvent);
                    }
                }

                return true;
            }
            
            return false;
        }

        // obliterates the specified events
        public void DeregisterEvents(params EventID[] requestedEvents)
        {
            if (IsActive)
            {
                var manager = (CustomEventManager)m_customEventManager.Target;

                // deregister these events if they exist
                foreach (var customEvent in requestedEvents)
                {
                    if (manager.IsApartOfEvent(this, customEvent))
                    {
                        m_eventsSubscribedTo.Remove(customEvent);
                    }

                    // perhaps this object wants to deregister an event
                    // that it technically is not associated with
                    manager.DeregisterEvent(customEvent);                    
                }
            }
        }

        // deregister the interactable from an event
        public void DeregisterFromEvents(params EventID[] requestedEvents)
        {
            // subscribe to events requested by the interactable
            if (IsActive)
            {
                var manager = (CustomEventManager)m_customEventManager.Target;

                foreach (var customEvent in requestedEvents)
                {
                    manager.DeregisterFromEvent(this, customEvent);
                }
            }
        }
        #endregion

        #region internal
        // notifies the interactable of received events...
        // internal use only.
        public void ReceiveNotify(CustomEventPacket customEventPacket)
        {
            if (IsActive)
            {
                Interactable interactable = (Interactable)m_interactable.Target;
                interactable.ReceiveNotify(customEventPacket);
            }
        }
        #endregion

        #region properties
        public int SubscribedCount
        {
            get
            {
                return m_eventsSubscribedTo.Count;
            }
        }

        public ICollection<EventID> SubscribedEvents
        {
            get
            {
                var subscribed = new EventID[m_eventsSubscribedTo.Count];
                m_eventsSubscribedTo.CopyTo(subscribed, 0);

                return subscribed;
            }
        }

        // if this returns false, this "beacon" is as good as useless : P
        public bool IsActive
        {
            get
            {
                return m_isInitialized && m_customEventManager.IsAlive
                    && m_interactable.IsAlive;
            }
        }

        // perhaps you need to do some "custom stuff"...
        public CustomEventManager Manager
        {
            get { return m_customEventManager.IsAlive ? (CustomEventManager)m_customEventManager.Target : null; }
        }
        #endregion
    }      
      
    protected delegate void CustomUpdate(float deltaTime);
    protected delegate void CustomStart();
    protected delegate void CustomEventReceiveNotify(CustomEventPacket handler);

    //---------------------------------------------------------  
    #region Internal Fields
    protected Interaction m_Interaction; 

    // child class creates these custom delegates for additional functionality
    // using the assign functions
    private CustomUpdate m_CustomUpdate;
    private CustomStart m_CustomStart;
    // required to receive custom events from an event manager!
    private CustomEventReceiveNotify m_CustomEventNotify;

    private bool m_Update;
    private bool m_Start;
    private bool m_Initialized;
    private bool m_ListensToEvents;

    // call any custom event needs through through this object.
    // you can check if it's available through IsActive.
    // reasons it won't be active:
    // 1) GameObject with GameManager tag, with a CustomEventManager does not
    //    exist in the scene
    // 2) this object is somehow null, which would make no sense
    private CustomEventBeacon m_EventBeacon;
    #endregion

    #region Public Fields
    [SerializeField, Header("Custom Event Stuff")]
    private EventID[] m_associatedEvents;
    #endregion

    #region UnityAPI
    private void Awake()
    {
        Initialize();
    }

    private void Start()
    {
        if (m_Start)
        {
            m_CustomStart();
        }
    }
    
    private void Update()
    {
        if (m_Update)
        {
            m_CustomUpdate(Time.deltaTime);
        }
    }
    #endregion

    #region Internal
    // called on awake.
    // calls the child's class init.
    private void Initialize()
    {
        GameObject gameManager = GameObject.FindGameObjectWithTag("GameManager");
        if (gameManager)
        {
            // safe to pass in null values (but why would you do that?)
            m_EventBeacon = new CustomEventBeacon(this, gameManager.GetComponent<CustomEventManager>());
        }
        else
        {
            print(name + ": Interactable: Failure to locate GameManager.");
        }

        Init();
        m_Initialized = true;        
    }
    #endregion

    #region Public Interface
    // call this function to interact with the object. acts as the base interface.
    // this function calls the user defined Commit function
    public void Interact(InteractMessage message)
    {
        if (message.interaction == InteractionType)
            Commit(message);
    }

    // set whether this interactable will be updated each tick.    
    public void SetUpdate(bool b)
    {
        m_Update = b;
    }
    #endregion

    #region Custom Event Stuff
    private void ReceiveNotify(CustomEventPacket customEventPacket)
    {
        var handler = customEventPacket.Handler;
        var eventID = customEventPacket.Handler.EventID;

        // manage the interactable and event beacon with messages from the manager
        bool isEventIDFromManager = CustomEventManager.IsEventIDReserved(handler.EventID);
        if (isEventIDFromManager)
        {
            switch((ReservedEventID)eventID)
            {
                case ReservedEventID.DeregisteredEvent:
                {
                    
                }
                break;
                case ReservedEventID.DeregisteredObserver:
                {
                    // nothing for now...
                }
                break;
                default:
                    break;
            }
        }

        if (m_ListensToEvents)
        {
            m_CustomEventNotify(customEventPacket);
        }
    }
    #endregion

    #region Internal Interface
    // the first function this object will call,
    // if inherited
    protected virtual void Init()
    {
    }

    // override this function.
    // so do not worry about having to manually call this function.
    protected virtual void Commit(InteractMessage msg)
    {
        Debug.Log("Interactable: Commit: You do not need to call the base for this function.");
    }

    // typically should be called during overrided init.
    // set your update to a name that is anything but "Update",
    // as it is reserved by Unity. ie. MyUpdate().
    protected void AssignUpdate(CustomUpdate myUpdate)
    {
        if (myUpdate != null)
        {            
            m_Update = true;
            m_CustomUpdate = myUpdate;
        }
    }

    // this assign SHOULD be called during overrided init, or
    // it will never be called by unity.
    // set your start to a name that is anything but "Start",
    // as it is reserved by Unity. ie. MyStart().
    protected void AssignStart(CustomStart myStart)
    {
        if (myStart != null)
        {            
            m_Start = true;
            m_CustomStart = myStart;
        }
    }

    // this assign SHOULD be called during overrided init, or
    // it will never be called by unity.
    // set your start to a name that is anything but "Start",
    // as it is reserved by Unity. ie. MyStart().
    protected void AssignCustomEventReceiveNotify(CustomEventReceiveNotify myNotify)
    {
        if (myNotify != null)
        {            
            m_ListensToEvents = true;
            m_CustomEventNotify = myNotify;
        }
    }

    // this function NEEDS to be called during your overrided init,
    // or this object will not properly work. after init is done,
    // setting the type is locked.
    protected void AssignInteractionType(Interaction interaction)
    {
        if (!m_Initialized)
            m_Interaction = interaction;
        else
            Debug.Log("Interactable: SetInteractionType: Cannot assign type after initialization.");
    }
    #endregion

    #region Getters/Setters
    public Interaction InteractionType
    {
        get { return m_Interaction; }        
    }

    public bool IsInitialized
    {
        get { return m_Initialized; }        
    }

    public bool IsUpdate
    {
        get
        {
            return m_Update;
        }
    }

    public bool IsStart
    {
        get
        {
            return m_Start;
        }
    }
    #endregion
}
