using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyTypes;

public class TestInteractionScript : MonoBehaviour {
    [SerializeField] Color m_defaultColor;
    [SerializeField] Color m_overColor;

    MeshRenderer m_renderer;

    private void Awake()
    {
        m_renderer = GetComponent<MeshRenderer>();
    }

    private void Start()
    {
        m_renderer.material.color = m_defaultColor;
    }

    private void OnMouseEnter()
    {
        m_renderer.material.color = m_overColor;
    }

    private void OnMouseExit()
    {
        m_renderer.material.color = m_defaultColor;
    }

    private void OnMouseDown()
    {
        InteractMessage msg = new InteractMessage(Interaction.GROWING, "Hello, we are growing.");
        gameObject.SendMessage("Interact", msg);
    }
}
