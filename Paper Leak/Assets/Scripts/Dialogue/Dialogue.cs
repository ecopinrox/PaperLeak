using System.Collections.Generic;
using UnityEngine;

public class Dialogue
{
    public readonly string text;
    public readonly string name;
    public readonly Sprite portrait;
    public readonly int? iconIndex;
    public readonly int? bgmIndex;
    public readonly List<string> actorStates;

    public Dialogue(string text, string name, Sprite portrait, int? iconIndex, int? bgmIndex, List<string> actorStates)
    {
        this.text           = text;
        this.name           = name;
        this.portrait       = portrait;
        this.iconIndex      = iconIndex;
        this.bgmIndex       = bgmIndex;
        this.actorStates    = actorStates;
    }
}
