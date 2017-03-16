using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyTypes;


public class MessengerComponent : Interactable {
    [SerializeField] ActionManager m_Manager;
    public ActionTarget [] m_VisibleTargets;   
    
    private Dictionary<int, ActionTarget> m_Targets;

    // copy key values over during the start
    protected override void Init()
    {
        // make sure the manager exists, otherwise the script is useless
        if (!m_Manager)
        {
            bool result = false;

            GameObject manager = GameObject.FindGameObjectWithTag("ActionManager");

            if (manager)
            {
                result = m_Manager = manager.GetComponent<ActionManager>();
            }
            if (!result)
            {
                Debug.Log(gameObject.name + ": Missing an action component associates with this messsenger component! Removing the component.");
                Destroy(this);
            }            
        }
        
        // setup the "interactable"
        AssignInteractionType(Interaction.MESSENGER);
        AssignStart(MyStart);

        m_Targets = new Dictionary<int, ActionTarget>();

        foreach(ActionTarget target in m_VisibleTargets)
        {
            bool containsKey = m_Targets.ContainsKey(target.ID);

            //ensure no duplicates
            if (containsKey)
            {
                if (m_Targets[target.ID].target != target.target)
                    m_Targets.Add(target.ID, target);
            }
            else
            {
                m_Targets.Add(target.ID, target);
            }                
        }
    }

    private void MyStart()
    {
        // subscribe all associationsc
        SubscribeToManager();
    }

    // place your custom logic here for interaction
    protected override void Commit(InteractMessage msg)
    {
        object[] data = new object[msg.msgData.Count];
        msg.msgData.CopyTo(data, 0);

        if (msg.msg == "Start")
        {
            int id = (int)data[0];
            m_Manager.MessengerTrigger(id, this, true);
        }
        else if (msg.msg == "Stop")
        {
            int id = (int)data[0];
            m_Manager.MessengerTrigger(id, this, false);
        }
        else if (msg.msg == "Complete")
        {
            SendMessage("Interact", new InteractMessage(Interaction.TRIGGER, "Trigger"));
        }
    }

    private void SubscribeToManager()
    {
        foreach(int key in m_Targets.Keys)
        {
            m_Manager.SubscribeToID(key, this, m_Targets[key].isBaseMessenger);
        }
    }
}
