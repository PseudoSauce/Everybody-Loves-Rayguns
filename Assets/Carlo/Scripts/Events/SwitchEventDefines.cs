using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ObjectTags
{
    None, Cube, Sphere, Prism, Red, Blue, Yellow, Player,
}

public enum CustomSwitchEvent
{
    OpenDoor = 7000, CloseDoor = 7001, ExtendBridge = 7002, RetractBridge = 7003,
    SpawnNewObject = 7004, DestroyOldObject = 7005
}

public enum CustomChestEvent
{
    OpenChest = 8000, CloseChest = 8001, PickupTreasure = 8002
}

struct DoorOpenEventHandler : ICustomEventHandler
{
    public uint DoorID;
    public uint EventID
    {
        get { return (uint)CustomSwitchEvent.OpenDoor; }
    }
}

struct DoorCloseEventHandler : ICustomEventHandler
{
    public uint DoorID;
    public uint EventID
    {
        get { return (uint)CustomSwitchEvent.CloseDoor; }
    }
}

struct ChestEventHandler : ICustomEventHandler
{
    public uint ChestID;
    public CustomChestEvent chestEvent;
    public uint EventID
    {
        get { return (uint)chestEvent; }
    }
}

struct ExtendBridgeEventHandler : ICustomEventHandler
{
    public uint EventID
    {
        get { return (uint)CustomSwitchEvent.ExtendBridge; }
    }
}

struct RetractBridgeEventHandler : ICustomEventHandler
{
    public uint EventID
    {
        get { return (uint)CustomSwitchEvent.RetractBridge; }
    }
}

struct SpawnObjectEventHandler : ICustomEventHandler
{
    public uint EventID
    {
        get { return (uint)CustomSwitchEvent.SpawnNewObject; }
    }
}

struct DestroyObjectEventHandler : ICustomEventHandler
{
    public uint EventID
    {
        get { return (uint)CustomSwitchEvent.DestroyOldObject; }
    }
}
