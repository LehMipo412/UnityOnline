using System.Collections.Generic;
using UnityEngine;

public class JsonTesting : MonoBehaviour
{
    //Stats instance;
    public Stats stick = new();
    public bool isInsideStatblock = false;


    //Check this changes with the json file open to understand how it works. Use "Testing" gameobject and change the values inside. Press Spacebar to save and "V" to load. 

    //Use this to find your name and value with the format json is using. Use Contains like i used in the SaveToJson.
    public string JsonFormat(string name, string value)
    {
        return $"\"{name}\": {value},";
    }

    //Saves only if not duplicated
    public void SaveToJson()
    {
        string stats = JsonUtility.ToJson(stick, true);
        string filepath = Application.persistentDataPath + "/Stats.json";

        string[] stats2;
        //ReadAllLines returns a string array.
        stats2 = System.IO.File.ReadAllLines(filepath);

        //Shows how "ReadAllLines" works opposed to "ReadAllText" which print the whole json.
       

        Debug.Log(filepath);

        //This checks for duplicates
        if (System.IO.File.ReadAllText(filepath).Contains((stats)))
        {
            return;
        }
        if (!System.IO.File.ReadAllText(filepath).Contains((JsonFormat("id", $"{stick.id}"))))
        {
            System.IO.File.AppendAllText(filepath, stats);
        }
        else
        {
            for (int i = 0; i < stats2.Length; i++)
            {
                Debug.Log(stats2[i]);
                if (stats2[i].Contains(JsonFormat("id", "1")))
                {
                    if(stats2[i].Contains((JsonFormat("id", $"{stick.id}"))))
                    {
                        isInsideStatblock = true;
                    }
                    if (isInsideStatblock)
                    {
                        if (stats2[i].Contains("hp"))
                        {
                            if(stats2[i].Length == 9)
                            {

                            }
                        }
                    }
                }
            }
        }

       
    }
    //To Open the Json - Copy the debug log line with the file path name and paste it into your explorer path. You can then open the json and see the changes in real time. 
    public void LoadFromJson()
    {
        string filepath = Application.persistentDataPath + "/Stats.json";
        string[] stats2;
        //ReadAllLines returns a string array.
        stats2 = System.IO.File.ReadAllLines(filepath);

        //Shows how "ReadAllLines" works opposed to "ReadAllText" which print the whole json.
        for (int i = 0; i < stats2.Length; i++)
        {
            Debug.Log(stats2[i]);
            if (stats2[i].Contains(JsonFormat("id", "1")))
            {
                Debug.Log("Player with id 1 found ");
            }
        }
    }

}

//Example of stat stick like you're using.
[System.Serializable]
public class Stats
{
    public int id;
    public float hp;
    public string name;
}