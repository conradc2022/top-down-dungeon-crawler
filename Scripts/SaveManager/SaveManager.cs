using Dungeon;
using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DataManager;

public partial class SaveManager : Node
{
    public string SaveDirectory = "user://saves/";
    public string SaveFileName = "save.json";
    //Set per player or to random string
    //Move to another location
    //Just prevents the user from modding their save
    public string Key = "1SA324DBA";

    public InitDungeon WorldState;

    public override void _Ready()
    {
        VerifySaveDirectory(SaveDirectory);
    }
    public void VerifySaveDirectory(string directory)
    {
        DirAccess.MakeDirAbsolute(directory);
    }

    public bool SaveData(string filePath)
    {
        try
        {
            FileAccess file = FileAccess.OpenEncryptedWithPass(filePath, FileAccess.ModeFlags.Write, Key);
            if(file == null)
            {
                throw new($"An error occured while saving at: {DateTime.Now.ToString()} : {FileAccess.GetOpenError()}");
            }
            Godot.Collections.Dictionary<string, Godot.Collections.Dictionary> tempData = new() 
            {
                //If in dungeon - this section is populated
                {"map_data", WorldState.dungeonGenerator.SerializeGenerator()}, //Resource with map information
                {"character_data", null}, //Resource containing all character information (of current map)
                {"interactable_data", null}, //Resource containing all interactable information
                
                {"inventory_data", null}, //Party inventory
                //'Always' populated
                {"party_data", null}, //All party characters (including those in character data)
                {"storage_data", null}, //Stored Items and Coins (not on the party in dungeon)
                {"dungeon_data", null}, //Which dungeons/events have been completed
                {"mission_data", null}, //Which quest/sidequests are active
            };

            string jsonString = Json.Stringify(tempData, "\t");
            Debug.WriteLine($"Saving: *{jsonString}*");
            file.StoreString(jsonString);
            file.Close();
            return true;
        }
        catch(Exception ex)
        {
            Debug.WriteLine($"Error: {ex.Message}");
            return false;
        }
    }
    public void LoadData(string filePath)
    {
        try{
            if(!FileAccess.FileExists(filePath))
            {
                throw new($"Cannot open nonexistant file at: {filePath}");
            }
            FileAccess file = FileAccess.OpenEncryptedWithPass(filePath, FileAccess.ModeFlags.Read, Key);
            if(file == null)
            {
                throw new($"An error occured while loading at: {DateTime.Now.ToString()} : {FileAccess.GetOpenError()}");
            }
            string content = file.GetAsText();
            Debug.WriteLine($"Loading: *{content}*");
            file.Close();

            Godot.Collections.Dictionary<string, Godot.Collections.Dictionary> tempData = (Godot.Collections.Dictionary<string, Godot.Collections.Dictionary>)Json.ParseString(content);
            if(tempData.Equals(null))
            {
                
                throw new($"Cannot parse {filePath} as JSON string {content}");
            }
            //Load content to expecting objects
            Debug.WriteLine($"Keys: {tempData.Keys}");
            Godot.Collections.Dictionary mapData = tempData[tempData.Keys.First(key => key.ToString().ToLower().Equals("map_data"))];
            if(mapData != null){
                WorldState.dungeonGenerator.DeserializeGenerator(mapData);
            }
        }
        catch(Exception ex)
        {
            Debug.WriteLine($"Error: {ex.Message} {ex.StackTrace}");
        }
    }
    
}
