using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMover : ActivatableObject {

    [SerializeField]
    private Transform m_startPosition;
    [SerializeField]
    private Transform m_endPosition;
    [SerializeField]
    private float m_moveSpeed = 2.5f;

    private bool m_moveToEnd = false;
    private bool m_moveToStart = false;

	void Update ()
    {
        Move();
	}

    public override void ActivateObject()
    {
        base.ActivateObject();
        m_moveToEnd = true;
        m_moveToStart = false;
    }

    public override void DeactivateObject()
    {
        base.DeactivateObject();
        m_moveToEnd = false;
        m_moveToStart = true;
    }

    private void Move()
    {
        if(m_moveToStart)
        {
            Vector3 newPos = Vector3.Lerp(transform.position, m_startPosition.position, m_moveSpeed * Time.deltaTime);
            transform.position = newPos;
            if(Vector3.Distance(transform.position, m_startPosition.position) < 0.5f)
            {
                m_moveToStart = false;
            }
        }
        else if(m_moveToEnd)
        {
            Vector3 newPos = Vector3.Lerp(transform.position, m_endPosition.position, m_moveSpeed * Time.deltaTime);
            transform.position = newPos;
            if (Vector3.Distance(transform.position, m_endPosition.position) < 0.5f)
            {
                m_moveToEnd = false;
            }
        }
    }
}
