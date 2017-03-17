using System;
using UnityEngine;

// just using an enum for simplicity sake.
// this is generally not a good idea because
// events can technically use any number,
// and eventIDs can change meaning during any point
// with deregister/register event
public enum CustomEventExamples
{
    EnemyAlertEvent = 666, BombTriggerEvent = 231,
    DoorOpenEvent = 668
}

struct ExampleEnemyAlertEventHandler : ICustomEventHandler
{
    public uint EventID
    {
        get { return (uint)CustomEventExamples.EnemyAlertEvent; }
    }
}

struct BombTriggerEvent : ICustomEventHandler
{
    public int switchNumber;

    public uint EventID
    {
        get { return (uint)CustomEventExamples.EnemyAlertEvent; }
    }
}

struct DoorOpenEvent : ICustomEventHandler
{
    public uint EventID
    {
        get { return (uint)CustomEventExamples.DoorOpenEvent; }
    }
}
