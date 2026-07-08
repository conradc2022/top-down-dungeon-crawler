using Godot;
using System;

namespace UI.Moves
{
    public partial class MoveButton : Button
    {
        [Export]
        public string MoveName;
        [Export]
        public int CurrentEP;
        [Export]
        public int MaxEP;
        [Export]
        public string MoveType;
        [Export]
        public Color DefaultColor;
        [Export]
        public Color AlertColor;

        private Label moveNameLabel;
        private Label typeLabel;
        private Label currentEPLabel;
        private Label maxEPLabel;
        private HBoxContainer energyLabel;

        public override void _Ready()
        {
            moveNameLabel = GetNode<Label>("MoveName");
            currentEPLabel = GetNode<Label>("Energy/EPCurrent");
            maxEPLabel = GetNode<Label>("Energy/EPTotal");
            typeLabel = GetNode<Label>("MoveType");
            energyLabel = GetNode<HBoxContainer>("Energy");
            SetName(MoveName);
            SetEnergyPoints(CurrentEP, MaxEP);
            SetType(MoveType);
            
        }
        public void SetName(string name)
        {
            moveNameLabel.Text = name;
        }
        public void SetEnergyPoints(int? current = null, int? max = null)
        {
            if(current != null && current >= 0)
            {
                currentEPLabel.Text = current.ToString();
                if(current <= 0)
                {
                    energyLabel.Modulate = AlertColor;
                }
                else
                {
                    energyLabel.Modulate = DefaultColor;
                }
            }
            if(max != null && max > 0)
            {
                maxEPLabel.Text = max.ToString();
            }
        }
        
        public void SetType(string name)
        {
            typeLabel.Text = name;
        }
    }
}
