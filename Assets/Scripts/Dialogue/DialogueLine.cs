using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    public string speakerName;
    public string requiredItemName;
    public string missingItemText;
    [TextArea(2, 5)] public string text;
    public Sprite portrait;
}
