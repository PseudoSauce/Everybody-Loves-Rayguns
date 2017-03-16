using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyTypes;

public class ActionManager : MonoBehaviour {
    MultiDict<MessengerResult> m_associatedValues;         // messengers can be linked together for a specific event
    [SerializeField] int m_keyRange = int.MaxValue;

    List<MessengerComponent> m_Messengers;

    private void Awake()
    {
        m_associatedValues = new MultiDict<MessengerResult>();
    }


    private void Update()
    {
        //Debug.Log(GetResultOfAssociation(3));
    }

    // use this to signal result is still valid or not (ie... player moved off a switch, so set it back to false)
    public void MessengerTrigger(int associationID, MessengerComponent messenger, bool result)
    {
        SetValueOfMessengerPair(associationID, messenger, result);

        Debug.Log(GetResultOfAssociation(associationID));

        if(GetResultOfAssociation(associationID))
        {
            foreach(MessengerResult res in m_associatedValues[associationID])
            {                
                if (res.isBase)
                {                    
                    res.messenger.Interact(new InteractMessage(Interaction.MESSENGER, "Complete"));
                }
            }
        }
    }

    // returns false typically if the messenger was already associated with that key
    public bool SubscribeToID(int ID, MessengerComponent messenger, bool isBase)
    {
        bool result = false;

        bool hasKey = m_associatedValues.ContainsKey(ID);     

        if (hasKey)
        {
            result = m_associatedValues.AddEntry(ID, new MessengerResult(messenger, isBase));
        }
        else
        {
            m_associatedValues.Add(ID, new List<MessengerResult>());
            result = m_associatedValues.AddEntry(ID, new MessengerResult(messenger, isBase));
        }

        Debug.Log(messenger.name + "(" + ID + "): subscribed.");

        return result;
    }

    // TODO 0:
    public void Unsuscribe(int ID, MessengerComponent messenger)
    {

    }

    private void SetValueOfMessengerPair(int associationID, MessengerComponent messenger, bool result)
    {
        if (m_associatedValues.ContainsKey(associationID))
        {
            int associationCount = m_associatedValues[associationID].Count;

            for (int i = 0; i < associationCount; ++i)
            {
                if (m_associatedValues[associationID][i].messenger == messenger)
                {
                    m_associatedValues[associationID][i] = new MessengerResult(messenger, result);
                    break;
                }
            }
        }
    }

    // returns nothing if failed to find messenger
    private string GetValueOfMessengerPair(int associationID, MessengerComponent messenger)
    {
        if (m_associatedValues.ContainsKey(associationID))
        {
            int associationCount = m_associatedValues[associationID].Count;

            for (int i = 0; i < associationCount; ++i)
            {
                if (m_associatedValues[associationID][i].messenger == messenger)
                {
                    return m_associatedValues[associationID][i].result.ToString();
                }
            }
        }

        return "";
    }

    // returns nothing if failed to find messenger
    private bool GetResultOfAssociation(int associationID)
    {
        bool result = false;

        if (m_associatedValues.ContainsKey(associationID))
        {
            foreach (MessengerResult mesResult in m_associatedValues[associationID])
            {
                result = mesResult.result;
            }
        }

        return result;
    }
}
