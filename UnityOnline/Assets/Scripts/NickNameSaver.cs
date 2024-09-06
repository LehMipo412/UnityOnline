using UnityEngine;


[System.Serializable]
public class NickNameSaver
{
    [field: SerializeField] public string Name { get; set; }


    public NickNameSaver(string name)
    {
        Name = name;
    }
}
