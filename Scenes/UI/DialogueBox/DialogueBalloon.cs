using System;
using System.Linq;
using Godot;
using Godot.Collections;
using UI.Dialogue;
using System.Linq;
using System.Text.RegularExpressions;
using System.Diagnostics;
namespace DialogueManagerRuntime
{
  public partial class DialogueBalloon : CanvasLayer
  {
    [Export] public string NextAction = "ui_accept";
    [Export] public string SkipAction = "ui_cancel";


    Control balloon;
    RichTextLabel characterLabel;
    RichTextLabel dialogueLabel;
    VBoxContainer responsesMenu;
    MarginContainer responses;

    Resource resource;
    Array<Variant> temporaryGameStates = new Array<Variant>();
    bool isWaitingForInput = false;
    bool willHideBalloon = false;

    DialogueLine dialogueLine;
    DialogueLine DialogueLine
    {
      get => dialogueLine;
      set
      {
        isWaitingForInput = false;
        balloon.FocusMode = Control.FocusModeEnum.All;
        balloon.GrabFocus();

        if (value == null)
        {
          QueueFree();
          return;
        }

        dialogueLine = value;
        UpdateDialogue();
      }
    }

    TextureRect portrait;
    public override void _Ready()
    {
      balloon = GetNode<Control>("%Balloon");
      characterLabel = GetNode<RichTextLabel>("%CharacterLabel");
      dialogueLabel = GetNode<RichTextLabel>("%DialogueLabel");
      responsesMenu = GetNode<VBoxContainer>("%ResponsesMenu");
      responses =  GetNode<MarginContainer>("%Responses");
      portrait = GetNode<TextureRect>("%Portrait");
      portrait.Hide(); //Assume that there is no Portrait to shot
      balloon.Hide();

      balloon.GuiInput += (@event) =>
      {
        if ((bool)dialogueLabel.Get("is_typing"))
        {
          bool mouseWasClicked = @event is InputEventMouseButton && (@event as InputEventMouseButton).ButtonIndex == MouseButton.Left && @event.IsPressed();
          bool skipButtonWasPressed = @event.IsActionPressed(SkipAction);
          if (mouseWasClicked || skipButtonWasPressed)
          {
            GetViewport().SetInputAsHandled();
            dialogueLabel.Call("skip_typing");
            return;
          }
        }

        if (!isWaitingForInput) return;
        if (dialogueLine.Responses.Count > 0) return;

        GetViewport().SetInputAsHandled();

        if (@event is InputEventMouseButton && @event.IsPressed() && (@event as InputEventMouseButton).ButtonIndex == MouseButton.Left)
        {
          Next(dialogueLine.NextId);
        }
        else if (@event.IsActionPressed(NextAction) && GetViewport().GuiGetFocusOwner() == balloon)
        {
          Next(dialogueLine.NextId);
        }
      };

      if (string.IsNullOrEmpty((string)responsesMenu.Get("next_action")))
      {
        responsesMenu.Set("next_action", NextAction);
      }
      responsesMenu.Connect("response_selected", Callable.From((DialogueResponse response) =>
      {
        Next(response.NextId);
      }));

      DialogueManager.Mutated += OnMutated;
    }


    public override void _ExitTree()
    {
      DialogueManager.Mutated -= OnMutated;
    }


    public override void _UnhandledInput(InputEvent @event)
    {
      // Only the balloon is allowed to handle input while it's showing
      GetViewport().SetInputAsHandled();
    }


    public override async void _Notification(int what)
    {
      // Detect a change of locale and update the current dialogue line to show the new language
      if (what == NotificationTranslationChanged && IsInstanceValid(dialogueLabel))
      {
        float visibleRatio = dialogueLabel.VisibleRatio;
        DialogueLine = await DialogueManager.GetNextDialogueLine(resource, DialogueLine.Id, temporaryGameStates);
        if (visibleRatio < 1.0f)
        {
          dialogueLabel.Call("skip_typing");
        }
      }
    }


    public async void Start(Resource dialogueResource, string title, Array<Variant> extraGameStates = null)
    {
      temporaryGameStates = extraGameStates ?? new Array<Variant>();
      isWaitingForInput = false;
      resource = dialogueResource;

      DialogueLine = await DialogueManager.GetNextDialogueLine(resource, title, temporaryGameStates);
    }


    public async void Next(string nextId)
    {
      DialogueLine = await DialogueManager.GetNextDialogueLine(resource, nextId, temporaryGameStates);
    }


    #region Helpers

    private (string, string, string) SplitCharacterName(string characterName)
    {
      Regex regex = new(@"^(?'name'[^|`\n]+)(`(?'alias'[^|`\n]+)`)?(\|(?'exp'[^`|\n]+))?$",RegexOptions.IgnoreCase | RegexOptions.Multiline);
      Match match = regex.Match(characterName);
      if(match.Success)
      {
        string name = match.Groups["name"].Value;
        string exp = match.Groups["exp"].Value;
        string alias = match.Groups["alias"].Value;
        return(string.IsNullOrEmpty(name) ? null : name, string.IsNullOrEmpty(exp) ? null :exp, string.IsNullOrEmpty(alias)? null : alias);
      }
      return (null, null, null);
    }

    private async void UpdateDialogue()
    {
      if (!IsNodeReady())
      {
        await ToSignal(this, SignalName.Ready);
      }

      //name is from the resource file
      //expression is from the expression list
      //alias is what is listed on screen
      if(string.IsNullOrEmpty(dialogueLine.Character))
      {
        characterLabel.Visible = false;

      }
      else{
      (string name, string expression, string alias) = SplitCharacterName(dialogueLine.Character);
      // Set up the character name
      characterLabel.Visible = !string.IsNullOrEmpty(alias ?? name);
      characterLabel.Text = Tr(alias ?? name, "dialogue");
      //Set up the character portrait

      string resourcePath = $"res://Resources/DialoguePortraits/{name.ToLower()}.tres";
      if(FileAccess.FileExists(resourcePath) && expression != null)
      {
        DialoguePortraitResource textureResource = GD.Load<DialoguePortraitResource>(resourcePath);
        string key = (string)textureResource.Expressions.Keys.FirstOrDefault(key => key.ToString().ToLower().Equals(expression.ToLower()));
        string portraitPath = (string)textureResource.Expressions[key];
        portrait.Texture = GD.Load<Texture2D>(portraitPath);
        portrait.Show();
      }
      else
      {
        portrait.Texture = null;
        portrait.Hide();
      }}

      // Set up the dialogue
      dialogueLabel.Hide();
      dialogueLabel.Set("dialogue_line", dialogueLine);

      // Set up the responses
      responsesMenu.Hide();
      responses.Hide();
      responsesMenu.Set("responses", dialogueLine.Responses);

      // Type out the text
      balloon.Show();
      willHideBalloon = false;
      dialogueLabel.Show();
      if (!string.IsNullOrEmpty(dialogueLine.Text))
      {
        dialogueLabel.Call("type_out");
        await ToSignal(dialogueLabel, "finished_typing");
      }

      // Wait for input
      if (dialogueLine.Responses.Count > 0)
      {
        balloon.FocusMode = Control.FocusModeEnum.None;
        responsesMenu.Show();
        responses.Show();
      }
      else if (!string.IsNullOrEmpty(dialogueLine.Time))
      {
        float time = 0f;
        if (!float.TryParse(dialogueLine.Time, out time))
        {
          time = dialogueLine.Text.Length * 0.02f;
        }
        await ToSignal(GetTree().CreateTimer(time), "timeout");
        Next(dialogueLine.NextId);
      }
      else
      {
        isWaitingForInput = true;
        balloon.FocusMode = Control.FocusModeEnum.All;
        balloon.GrabFocus();
      }
    }


    #endregion


    #region signals


    private void OnMutated(Dictionary _mutation)
    {
      isWaitingForInput = false;
      willHideBalloon = true;
      GetTree().CreateTimer(0.1f).Timeout += () =>
      {
        if (willHideBalloon)
        {
          willHideBalloon = false;
          balloon.Hide();
        }
      };
    }


    #endregion
  }
}
