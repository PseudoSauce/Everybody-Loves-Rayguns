using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BallColours
{
    Yellow,
    Red,
    Blue
}

public class BallPuzzle : ActivatorObject {

    [SerializeField]
    private BallReceptor[] m_receptors;

	void Update ()
    {
        bool isComplete = true;
		foreach(BallReceptor b in m_receptors)
        {
            if(!b.IsComplete())
            {
                isComplete = false;
                break;
            }
        }

        if(isComplete)
        {
            base.Activate();
        }
	}
}
