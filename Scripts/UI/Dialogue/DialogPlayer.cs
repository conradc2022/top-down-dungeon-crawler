using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace UI.Dialog
{
    public partial class DialogPlayer : CanvasLayer
    {
        [Export(PropertyHint.File, "*.json")]
        public string SceneTextFile;

        private Dictionary<string, List<string>> sceneText = new();
        private List<string> selectedText = new();
        private bool inProgress;
        private TextureRect background;
        private Label text;
        private int index = 0;

        public override void _Ready()
        {
            text = GetNode<Label>("Label");
            background = GetNode<TextureRect>("Background");

            background.Visible = false;
            text.Visible = false;

            sceneText = LoadText();
            SignalBus signalBus = GetNode<SignalBus>("/root/SignalBus");
            signalBus.DisplayDialog += (string key) => DisplayDialog(key);
        }

        private Dictionary<string, List<string>> LoadText()
        {
            FileInfo fileInfo = new(SceneTextFile);
            if(fileInfo.Exists)
            {
                string jsonContent = File.ReadAllText(SceneTextFile);
                Dictionary<string, List<string>> content = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(jsonContent);
                return content;
            }
            return new();
        }

        private void DisplayDialog(string key)
        {
            if(inProgress)
            {
                DisplayNextLine();
            }
            else
            {
                background.Visible = true;
                text.Visible = true;
                selectedText = sceneText[key];
                inProgress = true;
                index = 0;
                ShowText();
            }
        }

        private void DisplayNextLine()
        {
            if(index >= selectedText.Count)
            {
                FinishDialog();
            }
            else
            {
                ShowText();
            }
        }

        public void ShowText()
        {
            text.Text = selectedText[index];
            index += 1;
        }
    
        public void FinishDialog()
        {
            text.Text = "";
            text.Visible = false;
            background.Visible = false;
            index = 0;
            inProgress = false;

        }
    }
}
