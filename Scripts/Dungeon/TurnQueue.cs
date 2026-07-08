using Godot;
using System;
using Character;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
namespace TurnManagement;

public partial class TurnQueue : Node
{
    public Character.Character ActiveCharacter;
    public void Initialize()
    {
        UpdateSpeeds();
        ActiveCharacter = (Character.Character)GetChildren()[0];
    } 
    public void PlayTurn()
    {
        Task.Run(async () => await ActiveCharacter.PlayTurn());
        int newIndex = (ActiveCharacter.GetIndex() + 1) % GetChildCount();
        ActiveCharacter = (Character.Character)GetChildren()[newIndex];
    }
    public void UpdateSpeeds()
    {
        var characters = GetChildren()
        .Where(child => child is Character.Character)
        .Select(child => child)
        .Cast<Character.Character>().OrderByDescending((Character.Character child) => child.Statistics.CurrentStats.Speed).ToList();
        for(int i = 0; i<characters.Count(); i++)
        {
            MoveChild(characters[i], i);
        }
    }
    //Add a character that just spawned in
    public void AddCharacterToQueue(Character.Character character)
    {
        //Assuming that the rest of the list is already sorted, use AddSibling/AddChild to sort this character into the list
        Character.Character previousSibling = null;
        for(int i = 0; i<GetChildCount(); i++)
        {
            Character.Character currentSibling = (Character.Character)GetChild(i);
            if(currentSibling.Statistics.CurrentStats.Speed >= character.Statistics.CurrentStats.Speed)
            {
                previousSibling = currentSibling;
            }
            else
            {
                break;
            }
        }
        if(previousSibling == null)
        {
            AddChild(character);
            MoveChild(character, 0);
        }
        else
        {
            previousSibling.AddSibling(character);
        }
    }
}
