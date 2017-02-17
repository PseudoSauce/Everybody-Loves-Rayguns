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
        public InteractMessage(Interaction interaction, string msg)
        {
            this.msg = msg;
            this.interaction = interaction;
        }

        public string msg;
        public Interaction interaction;
    }
}
    
