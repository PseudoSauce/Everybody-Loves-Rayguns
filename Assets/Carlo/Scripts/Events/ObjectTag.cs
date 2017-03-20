using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectTag : MonoBehaviour
{
    [SerializeField]
    private ObjectTags m_tag = ObjectTags.None;
    
    public ObjectTags objectTag { get { return m_tag; } }
}
