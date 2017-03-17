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
    // you can get access to the actual CustomEventManager through a property, but you
    // should probably not do that unless you need to. The manager itself has some more functionality.
    protected sealed class CustomEventBeacon : ICustomEventInvoker, ICustomEventObserver
    {
        // a log of recorded events by ID
        private struct CustomEventLog
        {
            public List<EventID> SentLog
            {
                get { return m_sentLog; }
            }

            public List<EventID> ReceivedLog
            {
                get { return m_receivedLog; }
            }

            public List<EventID> ManagerLog
            {
                get { return m_sentLog; }
            }

            private List<EventID> m_sentLog;
            private List<EventID> m_receivedLog;
            private List<EventID> m_managerLog;
        }

        private WeakReference m_customEventManager;
        private WeakReference m_interactable;

        private LinkedList<EventID> m_eventsSubscribedTo;
        private CustomEventLog m_log;

        private bool m_isInitialized;
        private bool m_logEvents;

        internal CustomEventBeacon(Interactable eventReceiver, CustomEventManager eventManager, bool logEvents = false)
        {
            m_customEventManager = new WeakReference(eventManager);
            m_interactable = new WeakReference(eventReceiver);
            m_eventsSubscribedTo = new LinkedList<EventID>();
            m_log = new CustomEventLog();

            m_logEvents = logEvents;

            if (eventReceiver && eventManager)
            {
                m_isInitialized = true;
            }
        }

        // EventLogs are ordered. here for convenience.
        #region Log Stuff 
        public ICollection<EventID> EventReceivedLog
        {
            get { return m_log.ReceivedLog; }
        }

        public ICollection<EventID> EventSentLog
        {
            get { return m_log.SentLog; }
        }

        public ICollection<EventID> EventReceivedManagerLog
        {
            get { return m_log.ManagerLog; }
        }

        public void ClearLog()
        {
            m_log.ManagerLog.Clear();
            m_log.SentLog.Clear();
            m_log.ReceivedLog.Clear();
        }
        #endregion

        #region Event Stuff
        // request an event to occur using a handler defined with
        // the appropriate event id.
        // returns whether the request was sent successfully to at least one
        // of the observers.
        // this does not necessarely suggest that the event was successful,
        // since the event requirements are defined individually.
        public bool InvokeEvent(ICustomEventHandler customHandler)
        {
            bool notifySuccessful = false;

            if (IsActive)
            {
                var manager = (CustomEventManager)m_customEventManager.Target;   

                notifySuccessful = manager.NotifyObservers(this, customHandler);

                // log the sent event if successful
                if (m_logEvents && notifySuccessful)
                {
                    m_log.SentLog.Add(customHandler.EventID);
                }
            }

            return notifySuccessful;
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

            // perhaps your deregistering, regardless that this beacon
            // is active... which will probably never make sense
            foreach (var customEvent in requestedEvents)
            {
                m_eventsSubscribedTo.Remove(customEvent);
            }
        }

        // deregister the interactable from an event
        public void DeregisterFromAllEvents()
        {
            // subscribe to events requested by the interactable
            if (IsActive)
            {
                var manager = (CustomEventManager)m_customEventManager.Target;

                foreach (var customEvent in m_eventsSubscribedTo)
                {
                    manager.DeregisterFromEvent(this, customEvent);
                }
            }

            m_eventsSubscribedTo.Clear();
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

                var handler = customEventPacket.Handler;

                // manage the interactable and event beacon with messages from the manager
                bool isManagerHandler = CustomEventManager.IsManagerHandler(handler);
                
                // could be from the manager itself... in his great honor
                if (isManagerHandler)
                {
                    // log the event if set to
                    if (m_logEvents)
                    {
                        m_log.ManagerLog.Add(customEventPacket.Handler.EventID);
                    }
                    // delegate to the interactable
                    interactable.ReceiveManagerNotify((ICustomEventManagerHandler)handler);
                }
                else
                {
                    // log the event if set to
                    if (m_logEvents)
                    {
                        m_log.ReceivedLog.Add(customEventPacket.Handler.EventID);
                    }

                    // delegate to the interactable
                    interactable.ReceiveNotify(customEventPacket);
                }                
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
    protected delegate void CustomEventReceiveNotify(CustomEventPacket handlerPacket);
    protected delegate void CustomEventReceiveManagerNotify(ICustomEventManagerHandler handler);

    //---------------------------------------------------------  
    #region Internal Fields
    protected Interaction m_Interaction; 

    // child class creates these custom delegates for additional functionality
    // using the assign functions
    private CustomUpdate m_CustomUpdate;
    private CustomStart m_CustomStart;
    // required to receive custom events from an event manager!
    private CustomEventReceiveNotify m_CustomEventNotify;
    private CustomEventReceiveManagerNotify m_CustomEventManagerNotify;

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
    [SerializeField, Header("Interactable Stuff")]
    private bool m_IgnoreUpdate = false;
    private bool m_IgnoreInteractions = false;

    [Header("Custom Event Stuff")]
    [SerializeField, Tooltip("Whether to keep a log of all received events, by their EventID. Stored as a continuous list.")]
    private bool m_RecordEvents = true;
    [SerializeField]
    private bool m_IgnoreManagerEvents = false,
                 m_IgnoreCustomEvents = false;
    [SerializeField, Tooltip("Events to register during initialization. (not a requirement if are just invoking).")]
    private EventID[] m_InitialEventsToRegisterTo;
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
        // seems ambiguous, but the m_update refers to
        // whether it was registered, while ignore update
        // is a toggle that can be changed whenever required
        if (!m_IgnoreUpdate && m_Update)
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
        // create the event beacon
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
        
        // register initial events specified in the editor
        // if this interactable was set to 
        if (m_ListensToEvents)
        {
            m_EventBeacon.RegisterEvents(m_InitialEventsToRegisterTo);  
        }
    }
    #endregion

    #region Public Interface
    // call this function to interact with the object. acts as the base interface.
    // this function calls the user defined Commit function
    public void Interact(InteractMessage message)
    {
        if (!m_IgnoreInteractions && message.interaction == InteractionType)
        {
            Commit(message);
        }
    }
    #endregion

    #region Custom Event Stuff
    // listens for invoker messages.
    // use AssignReceiveNotify to subscribe.
    private void ReceiveNotify(CustomEventPacket customEventPacket)
    {
        if (!m_IgnoreCustomEvents && m_ListensToEvents)
        {
            m_CustomEventNotify(customEventPacket);
        }
    }

    // listens for the event manager's messages
    // use AssignReceiveNotify to subscribe.
    private void ReceiveManagerNotify(ICustomEventManagerHandler handler)
    {
        if (!m_IgnoreManagerEvents && m_ListensToEvents)
        {
            m_CustomEventManagerNotify(handler);
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
    protected bool AssignUpdate(CustomUpdate myUpdate)
    {
        if (myUpdate != null)
        {            
            m_Update = true;
            m_CustomUpdate = myUpdate;

            return true;
        }

        return false;
    }

    // this assign SHOULD be called during overrided init, or
    // it will never be called by unity.
    // set your start to a name that is anything but "Start",
    // as it is reserved by Unity. ie. MyStart().
    protected bool AssignStart(CustomStart myStart)
    {
        if (myStart != null)
        {            
            m_Start = true;
            m_CustomStart = myStart;

            return true;
        }

        return false;
    }
        
    // set your two event listener functions.
    // otherwise this interactable does not have access to custom events.
    // it is the job of your interactable to define these functions to intercept events in the way you want.
    protected bool AssignCustomEventReceiveNotify(CustomEventReceiveNotify myNotify, CustomEventReceiveManagerNotify myManagerNotify)
    {
        if (myNotify != null && myManagerNotify != null)
        {            
            m_ListensToEvents = true;
            m_CustomEventNotify = myNotify;
            m_CustomEventManagerNotify = myManagerNotify;

            return true;
        }

        return false;
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
    // use this beacon for registering custom events, etc.
    protected CustomEventBeacon EventBeacon
    {
        get { return m_EventBeacon; }
    }

    public Interaction InteractionType
    {
        get { return m_Interaction; }        
    }

    public bool IsInitialized
    {
        get { return m_Initialized; }        
    }

    public bool IgnoreUpdate
    {
        get { return m_IgnoreUpdate; }
        set { m_IgnoreUpdate = value; }
    }

    public bool IgnoreInteractions
    {
        get { return m_IgnoreInteractions; }
        set { m_IgnoreInteractions = value; }
    }

    public bool IgnoreManagerEvents
    {
        get { return m_IgnoreManagerEvents; }
        set { m_IgnoreManagerEvents = value; }
    }

    public bool IgnoreCustomEvents
    {
        get { return m_IgnoreCustomEvents; }
        set { m_IgnoreCustomEvents = value; }
    }

    // whether this interactable was initialized with an update
    public bool IsUpdatable
    {
        get { return m_Update; }
    }

    // whether this interactable was initialized with a start
    public bool IsStart
    {
        get { return m_Start; }
    }
    #endregion
}
