using UnityEngine;

public class Dialogue
{
    public readonly string text;
    public readonly string name;
    public readonly Sprite portrait;
    public readonly int? iconIndex;
    public readonly int? bgmIndex;
    public readonly int? actorIndex;
    public readonly int? actorVisualState;
    public readonly int? actorDestination;
    public readonly float actorSpeed;

    public Dialogue(string text, string name, Sprite portrait, int? iconIndex, int? bgmIndex, int? actorIndex, int? actorVisualState, int? actorDestination, float actorSpeed)
    {
        this.text               = text;
        this.name               = name;
        this.portrait           = portrait;
        this.iconIndex          = iconIndex;
        this.bgmIndex           = bgmIndex;
        this.actorIndex         = actorIndex;
        this.actorVisualState   = actorVisualState;
        this.actorDestination   = actorDestination;
        this.actorSpeed         = actorSpeed;
    }
}
