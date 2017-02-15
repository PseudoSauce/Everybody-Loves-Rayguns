using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyTypes;


// inherit this for your interaction.
// all unity API based functions happen through this class.
// please override the Init function, and initialize your own custom
// Update in it. (Call AssignUpdate to be recognized for unity protocol updates)
public class Interactable : MonoBehaviour {  
      
    protected delegate void CustomUpdate(float deltaTime);
    protected delegate void CustomStart();

    //---------------------------------------------------------  
    #region Internal Fields
    protected Interaction m_Interaction; 

    // child class assigns these delegates if they want to have gameobject functionality
    private CustomUpdate m_CustomUpdate;
    private CustomStart m_CustomStart;

    private bool m_Update;
    private bool m_Start;
    private bool m_Initialized;

    // keeps track of the amount of delay has gone over the modified update
    private float m_accumulatedDelay;
    private float m_NextTickDelay;
    private bool m_CanTick = true;
    #endregion

    #region Public Fields
    // custom tick
    [SerializeField, Header("Update Properties")]
    [Tooltip("Allows this specific instance to be on its own default timer. " +
             "UseModifiedUpdate must be enabled.")]
    public float UpdateMinimumTick = 0.02f;
    [Tooltip("Use a custom tick on this specific instance.")]
    public bool UseModifiedUpdate = false;
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
            if (UseModifiedUpdate)
            {
                if (m_CanTick)
                {
                    m_CustomUpdate(m_accumulatedDelay);
                    StartCoroutine(CustomTick(m_NextTickDelay));
                }                    
            }        
            else
            {
                m_CustomUpdate(Time.deltaTime);
            }
        }
    }
    #endregion

    #region Internal
    // called on awake.
    // calls the child's class init.
    private void Initialize()
    {
        Init();
        m_Initialized = true;        
    }

    private IEnumerator CustomTick(float delay)
    {
        m_CanTick = false;
        m_accumulatedDelay = Time.realtimeSinceStartup;

        yield return new WaitForSeconds(delay);

        m_accumulatedDelay = Time.realtimeSinceStartup - m_accumulatedDelay;

        // decide the wait for the next tick
        float next = UpdateMinimumTick+m_accumulatedDelay;
        m_NextTickDelay = next >= 0 ? next : 0;

        //Debug.Log(m_NextTickDelay);

        m_CanTick = true;
    }
    #endregion

    #region Public Interface
    // call this function to interact with the object. acts as the base interface.
    // this function calls the user defined Commit function
    public void Interact(InteractMessage message)
    {
        if (message.interaction == InteractionType)
            Commit(message.msg);
        else
            Debug.Log("Interactable: '" + message.interaction + "' is not a valid interaction for this component.");
    }

    // set whether this interactable will be updated each tick.    
    public void SetUpdate(bool b)
    {
        m_Update = b;
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
    protected virtual void Commit(string msg)
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
            m_NextTickDelay = UpdateMinimumTick;
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
        set
        {
            m_Update = value;

            // reset values if set to false
            if (value == false)
            {
                StopCoroutine("CustomTick");
                m_accumulatedDelay = 0;
                m_NextTickDelay = UpdateMinimumTick;
            }
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
