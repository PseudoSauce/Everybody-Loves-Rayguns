using UnityEngine;
using System.Collections.Generic;

namespace MyTypes
{
    public enum Weapon
    {
        GROW, SHRINK, TELEPORT
    }

    public enum Interaction
    {
        SCALING, TELEPORTING, MESSENGER, TRIGGER
    }

    // send the interactable component this message.    
    public struct InteractMessage
    {
        public InteractMessage(Interaction interaction, string msg, params object[] data)
        {
            this.msg = msg;
            this.interaction = interaction;
            this.msgData = data;
        }

        public string msg;
        public ICollection<object> msgData;
        public Interaction interaction;

        public override string ToString()
        {            
            return interaction.ToString();
        }
    }

     [System.Serializable]
     public struct ActionTarget {
         public int ID;
         public Interactable target;
         public bool isBaseMessenger;
     }

    public struct MessengerResult
    {
        public MessengerResult(MessengerComponent messenger, bool isBase, bool result = false)
        {
            this.messenger = messenger;
            this.result = result;
            this.isBase = isBase;
        }

        public MessengerComponent messenger;
        public bool result;
        public bool isBase;
    }
}
    
