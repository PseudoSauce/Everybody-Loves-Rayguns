using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

/*
 * Create a "specific" interface using this,
 * that will mask your specific message sent through
 * the event handler.
 * 
 * ie. interface IOpenDoor : implements ICustomEventHandler
 * 
 * the event id is the unique id
 * each observer of an eventID will trigger whatever event they are associated with,
 * when this message is sent through the GameEventManager
 */
public interface ICustomEventHandler { 
    uint EventID
    {
        get;
    }
}
