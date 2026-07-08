using System.Diagnostics;
using System.Linq;
using Godot;
using Godot.Collections;

public static class DeserializeVectorUtils
{
    public static bool DeserializeVector2(Dictionary dictionary, out Vector2 result)
    {
        result = new();
        bool missingBoth = true;  
        bool failedAny = false;      
        if(dictionary.Keys.Any(key => key.ToString().ToLower().Equals("x")))
        {
            bool success = float.TryParse(dictionary[dictionary.Keys.First(key => key.ToString().ToLower().Equals("x"))].ToString(), out float value);
            if(success)
            {
                result.X = value;
                missingBoth = false;
            }
            else
            {
                Debug.WriteLine($"Failed to parse X: {dictionary[dictionary.Keys.First(key => key.ToString().ToLower().Equals("x"))].ToString()}");
                failedAny = true;
            }
        }        
        if(dictionary.Keys.Any(key => key.ToString().ToLower().Equals("y")))
        {
            bool success = float.TryParse(dictionary[dictionary.Keys.First(key => key.ToString().ToLower().Equals("y"))].ToString(), out float value);
            if(success)
            {
                result.Y = value;
                missingBoth = false;
            }
            else
            {
                Debug.WriteLine($"Failed to parse Y: {dictionary[dictionary.Keys.First(key => key.ToString().ToLower().Equals("y"))].ToString()}");
                failedAny = true;
            }
        }
        return !missingBoth && !failedAny;
    }
    
    public static bool DeserializeVector3(Dictionary dictionary, out Vector3 result)
    {
        result = new();
        bool missingAll = true;  
        bool failedAny = false;      
        if(dictionary.Keys.Any(key => key.ToString().ToLower().Equals("x")))
        {
            bool success = float.TryParse(dictionary[dictionary.Keys.First(key => key.ToString().ToLower().Equals("x"))].ToString(), out float value);
            if(success)
            {
                result.X = value;
                missingAll = false;
            }
            else
            {
                Debug.WriteLine($"Failed to parse X: {dictionary[dictionary.Keys.First(key => key.ToString().ToLower().Equals("x"))].ToString()}");
                failedAny = true;
            }
        }        
        if(dictionary.Keys.Any(key => key.ToString().ToLower().Equals("y")))
        {
            bool success = float.TryParse(dictionary[dictionary.Keys.First(key => key.ToString().ToLower().Equals("y"))].ToString(), out float value);
            if(success)
            {
                result.Y = value;
                missingAll = false;
            }
            else
            {
                Debug.WriteLine($"Failed to parse Y: {dictionary[dictionary.Keys.First(key => key.ToString().ToLower().Equals("y"))].ToString()}");
                failedAny = true;
            }
        }
        if(dictionary.Keys.Any(key => key.ToString().ToLower().Equals("z")))
        {
            bool success = float.TryParse(dictionary[dictionary.Keys.First(key => key.ToString().ToLower().Equals("z"))].ToString(), out float value);
            if(success)
            {
                result.Z = value;
                missingAll = false;
            }
            else
            {
                Debug.WriteLine($"Failed to parse Z: {dictionary[dictionary.Keys.First(key => key.ToString().ToLower().Equals("z"))].ToString()}");
                failedAny = true;
            }
        }
        return !missingAll && !failedAny;
    }
    public static bool DeserializeVector4(Dictionary dictionary, out Vector4 result)
    {
        result = new();
        bool missingAll = true;
        bool failedAny = false;        
        if(dictionary.Keys.Any(key => key.ToString().ToLower().Equals("x")))
        {
            bool success = float.TryParse(dictionary[dictionary.Keys.First(key => key.ToString().ToLower().Equals("x"))].ToString(), out float value);
            if(success)
            {
                result.X = value;
                missingAll = false;
            }
            else
            {
                Debug.WriteLine($"Failed to parse X: {dictionary[dictionary.Keys.First(key => key.ToString().ToLower().Equals("x"))].ToString()}");
                failedAny = true;
            }
        }        
        if(dictionary.Keys.Any(key => key.ToString().ToLower().Equals("y")))
        {
            bool success = float.TryParse(dictionary[dictionary.Keys.First(key => key.ToString().ToLower().Equals("y"))].ToString(), out float value);
            if(success)
            {
                result.Y = value;
                missingAll = false;
            }
            else
            {
                Debug.WriteLine($"Failed to parse Y: {dictionary[dictionary.Keys.First(key => key.ToString().ToLower().Equals("y"))].ToString()}");
                failedAny = true;
            }
        }
        if(dictionary.Keys.Any(key => key.ToString().ToLower().Equals("z")))
        {
            bool success = float.TryParse(dictionary[dictionary.Keys.First(key => key.ToString().ToLower().Equals("z"))].ToString(), out float value);
            if(success)
            {
                result.Z = value;
                missingAll = false;
            }
            else
            {
                Debug.WriteLine($"Failed to parse Z: {dictionary[dictionary.Keys.First(key => key.ToString().ToLower().Equals("z"))].ToString()}");
                failedAny = true;
            }
        }
        if(dictionary.Keys.Any(key => key.ToString().ToLower().Equals("w")))
        {
            bool success = float.TryParse(dictionary[dictionary.Keys.First(key => key.ToString().ToLower().Equals("w"))].ToString(), out float value);
            if(success)
            {
                result.W = value;
                missingAll = false;
            }
            else
            {
                Debug.WriteLine($"Failed to parse W: {dictionary[dictionary.Keys.First(key => key.ToString().ToLower().Equals("w"))].ToString()}");
                failedAny = true;
            }
        }
        return !missingAll && !failedAny;
    }
}