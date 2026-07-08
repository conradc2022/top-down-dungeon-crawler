using Godot;
using System;

namespace UI.Dialog
{
    public partial class DialogArea : Area2D
    {
        [Export]
        public string Key;
        private bool active = false;

        //If is active and the player interacts with it:
        public void Signal()
        {
            EmitSignal(SignalBus.SignalName.DisplayDialog, Key);
        }
    }
}
