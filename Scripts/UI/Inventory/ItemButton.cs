using Godot;
using System;

namespace UI.Inventory
{
    public partial class ItemButton : Button
    {
        [Export]
        public string ItemName;
        [Export]
        public int Cost;
        [Export]
        public int Count;
        [Export]
        public string OwnerName;

        private Label itemNameLabel;
        private Label ownerLabel;
        private Label costLabel;
        private Label countLabel;
        private HBoxContainer costContainer;
        private HBoxContainer countContainer;

        public override void _Ready()
        {
            itemNameLabel = GetNode<Label>("ItemName");
            ownerLabel = GetNode<Label>("OwnerName");
            costLabel = GetNode<Label>("Cost/CostTotal");
            costContainer = GetNode<HBoxContainer>("Cost");
            countLabel = GetNode<Label>("Count/CountTotal");
            countContainer = GetNode<HBoxContainer>("Count");
            SetName(ItemName);
            SetCost(Cost);
            SetCost(Count);
            SetOwnerName(OwnerName);
            
        }
        public void SetName(string name)
        {
            itemNameLabel.Text = name;
        }
        public void SetCost(int current)
        {
            if(current >= 0)
            {
                costLabel.Text = current.ToString();
                if(current <= 0)
                {
                    costContainer.Visible = false;
                }
                else
                {
                    costContainer.Visible = true;
                }
            }
        }
        public void SetCount(int current)
        {
            if(current >= 1)
            {
                countLabel.Text = current.ToString();
                if(current <= 1)
                {
                    countContainer.Visible = false;
                }
                else
                {
                    countContainer.Visible = true;
                }
            }
        }
        
        public void SetOwnerName(string name)
        {
            ownerLabel.Text = name;
        }
    }
}
