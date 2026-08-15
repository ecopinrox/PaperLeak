using Ink.Runtime;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Conversation", menuName = "Scriptable Objects/Conversation")]
public class ConversationSO : ScriptableObject
{
    [SerializeField] TextAsset conversationScript;
    [SerializeField] List<PortraitCollectionSO> characterPortraitCollections;
    [field: SerializeField] public List<GameObject> ActorPrefabs { get; private set; }
    [field: SerializeField] public List<Vector2Int> ActorStartPos { get; private set;  }

    public IEnumerator<Dialogue> GetEnumerator()
    {
        Story story = new(conversationScript.text);

        while(story.canContinue)
        {
            string dialogue = story.Continue();
            List<string> tags = story.currentTags;

            yield return ConstructDialogue(dialogue, tags);
        }
    }

    Dialogue ConstructDialogue(string text, List<string> tags)
    {
        return new(
            text, 
            GetName(ParseCharacter(tags)), 
            GetPortrait(ParseCharacter(tags), ParsePortrait(tags)),
            ParseIcon(tags),
            ParseBGM(tags),
            GetActorStates(tags)
        );
    }

    int ParseCharacter(List<string> tags)
    {
        if (tags.Count < 1) return 0;
        if (!int.TryParse(tags[0], out int characterIndex)) return 0;
        return characterIndex;
    }

    int ParsePortrait(List<string> tags)
    {
        if (tags.Count < 2) return 0;
        if (!int.TryParse(tags[1], out int portraitIndex)) return 0;
        return portraitIndex;
    }

    int? ParseIcon(List<string> tags)
    {
        if (tags.Count < 3) return null;
        if (!int.TryParse(tags[2], out int iconIndex)) return null;
        return iconIndex;
    }

    int? ParseBGM(List<string> tags)
    {
        if (tags.Count < 4) return null;
        if (!int.TryParse(tags[3], out int bgmIndex)) return null;
        return bgmIndex;
    }

    List<string> GetActorStates(List<string> tags)
    {
        if (tags.Count < 5) return new();
        return tags.GetRange(4, tags.Count - 4);
    }

    string GetName(int characterIndex)
    {
        return characterPortraitCollections[characterIndex].Name;
    }

    Sprite GetPortrait(int characterIndex, int portraitIndex)
    {
        return characterPortraitCollections[characterIndex].GetPortrait(portraitIndex);
    }
}
