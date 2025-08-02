using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    public string speakerName;
    public Sprite portrait;
    [TextArea(2, 5)] public string text;
    public MeowerType meowerType;
}

public enum MeowerType { Mom, Kitten }
