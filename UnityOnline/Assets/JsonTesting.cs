using System.Collections.Generic;
using UnityEngine;

public class JsonTesting : MonoBehaviour
{
    public Stats stick = new();


    //Check this changes with the json file open to understand how it works. Use "Testing" gameobject and change the values inside. Press Spacebar to save and "V" to load. 
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SaveToJson();
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            LoadFromJson();
        }
    }

    

    public string JsonFormat(string name, string value)
    {
        return $"\"{name}\": {value},";
    }


    public void SaveToJson()
    {
        string stats = JsonUtility.ToJson(stick, true);
        string filepath = Application.persistentDataPath + "/Stats.json";
        Debug.Log(filepath);

        if (System.IO.File.ReadAllText(filepath).Contains((stats)))
        {
            return;
        }

        System.IO.File.AppendAllText(filepath, stats);

    }
    //To Open the Json - Copy the debug log line with the file path name and paste it into your explorer path. You can then open the json and see the changes in real time. 
    public void LoadFromJson()
    {
        string filepath = Application.persistentDataPath + "/Stats.json";
        string[] stats2;
        stats2 = System.IO.File.ReadAllLines(filepath);

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

[System.Serializable]
public class Stats
{
    public int id;
    public int hp;
    public int name;
}