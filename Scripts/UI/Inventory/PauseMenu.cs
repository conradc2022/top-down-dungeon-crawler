using Godot;
using System;

namespace UI
{
    public partial class PauseMenu : Control
    {
        private ColorRect navigator;
        private ColorRect statusOverview;
        private ColorRect moveOverview;
        private ColorRect inventory;
        private ColorRect party;
        private ColorRect settings;
        private ColorRect exitGame;

        public string CurrentLocation;

        public override void _Ready()
        {
            navigator = GetNode<ColorRect>("Navigator");
            statusOverview = GetNode<ColorRect>("StatusOverview");
            moveOverview = GetNode<ColorRect>("MoveOverview");
            inventory = GetNode<ColorRect>("Inventory");
            party = GetNode<ColorRect>("Party");
            settings = GetNode<ColorRect>("Settings");
            exitGame = GetNode<ColorRect>("Exit");

            Navigate("status");
        }

        public void Navigate(string location)
        {
            switch (location.ToLower())
            {
                case "moves":
                    settings.Visible = false;
                    inventory.Visible = false;
                    party.Visible = false;
                    exitGame.Visible = false;
                    moveOverview.Visible = true;
                    statusOverview.Visible = true;
                    break;
                case "inventory":
                    settings.Visible = false;
                    inventory.Visible = true;
                    party.Visible = false;
                    exitGame.Visible = false;
                    moveOverview.Visible = false;
                    statusOverview.Visible = false;
                    break;
                case "party":
                    settings.Visible = false;
                    inventory.Visible = false;
                    party.Visible = true;
                    exitGame.Visible = false;
                    moveOverview.Visible = false;
                    statusOverview.Visible = false;
                    break;
                case "settings":
                    settings.Visible = true;
                    inventory.Visible = false;
                    party.Visible = false;
                    exitGame.Visible = false;
                    moveOverview.Visible = false;
                    statusOverview.Visible = false;
                    break;
                case "exit":
                    settings.Visible = false;
                    inventory.Visible = false;
                    party.Visible = false;
                    exitGame.Visible = true;
                    moveOverview.Visible = false;
                    statusOverview.Visible = false;
                    break;
                case "status":
                default:
                    settings.Visible = false;
                    inventory.Visible = false;
                    party.Visible = false;
                    exitGame.Visible = false;
                    moveOverview.Visible = false;
                    statusOverview.Visible = true;
                    break;

            }

        }

        public void _on_home_pressed()
        {
            Navigate("status");
        }
        public void _on_moves_pressed()
        {
            Navigate("moves");
        }
        public void _on_items_pressed()
        {
            Navigate("inventory");
        }
        public void _on_party_pressed()
        {
            Navigate("party");
        }
        public void _on_options_pressed()
        {
            Navigate("settings");
        }
        public void _on_exit_pressed()
        {
            Navigate("exit");
        }
    }
}