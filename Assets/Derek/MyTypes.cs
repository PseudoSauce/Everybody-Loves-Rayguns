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
        SCALING, TELEPORTING
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
}
    
