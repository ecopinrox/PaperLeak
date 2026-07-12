using UnityEngine;

public class Dialogue
{
    public readonly string text;
    public readonly string name;
    public readonly Sprite portrait;

    public Dialogue(string text, string name, Sprite sprite)
    {
        this.text = text;
        this.name = name;
        portrait = sprite;
    }
}
