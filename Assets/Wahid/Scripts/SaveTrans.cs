using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreTransform {
    private Transform m_Transform;
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 localScale;
    public StoreTransform(Transform aTransform) {
        m_Transform = aTransform;
    }
    public StoreTransform LocalPosition() {
        position = m_Transform.localPosition;
        return this;
    }
    public StoreTransform Position() {
        position = m_Transform.position;
        return this;
    }
    public StoreTransform LocalRotation() {
        rotation = m_Transform.localRotation;
        return this;
    }
    public StoreTransform Rotation() {
        rotation = m_Transform.rotation;
        return this;
    }
    public StoreTransform Scale() {
        localScale = m_Transform.localScale;
        return this;
    }
    public StoreTransform AllLocal() {
        return LocalPosition().LocalRotation().Scale();
    }
    public StoreTransform AllWorld() {
        return Position().Rotation().Scale();
    }
}

public class RestoreTransform {
    private Transform m_Transform;
    private StoreTransform m_Data;
    public RestoreTransform(Transform aTransform, StoreTransform aData) {
        m_Transform = aTransform;
        m_Data = aData;
    }
    public RestoreTransform LocalPosition() {
        m_Transform.localPosition = m_Data.position;
        return this;
    }
    public RestoreTransform Position() {
        m_Transform.position = m_Data.position;
        return this;
    }
    public RestoreTransform LocalRotation() {
        m_Transform.localRotation = m_Data.rotation;
        return this;
    }
    public RestoreTransform Rotation() {
        m_Transform.rotation = m_Data.rotation;
        return this;
    }
    public RestoreTransform Scale() {
        m_Transform.localScale = m_Data.localScale;
        return this;
    }
    public RestoreTransform AllLocal() {
        return LocalPosition().LocalRotation().Scale();
    }
    public RestoreTransform AllWorld() {
        return Position().Rotation().Scale();
    }

}


public static class TransformSerializationExtension {
    public static StoreTransform Save(this Transform aTransform) {
        return new StoreTransform(aTransform);
    }

    public static RestoreTransform Load(this Transform aTransform, StoreTransform aData) {
        return new RestoreTransform(aTransform, aData);
    }
}