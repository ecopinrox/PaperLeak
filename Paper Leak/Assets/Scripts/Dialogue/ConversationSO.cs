using Ink.Runtime;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Conversation", menuName = "Scriptable Objects/Conversation")]
public class ConversationSO : ScriptableObject
{
    [SerializeField] TextAsset conversationScript;
    [SerializeField] List<PortraitCollectionSO> characterPortraitCollections;

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
        Debug.Log(
            $"Icon: {ParseIcon(tags)}\n" +
            $"BGM: {ParseBGM(tags)}\n" +
            $"Actor index: {ParseActorIndex(tags)}\n" +
            $"Actor visual state: {ParseActorVisualState(tags)}\n" +
            $"Actor destination Y: {ParseActorDestination(tags)}\n" +
            $"Actor speed: {ParseActorSpeed(tags)}\n"
        );


        return new(
            text, 
            GetName(ParseCharacter(tags)), 
            GetPortrait(ParseCharacter(tags), ParsePortrait(tags))
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

    int? ParseActorIndex(List<string> tags)
    {
        if (tags.Count < 5) return null;
        if (!int.TryParse(tags[4], out int actorIndex)) return null;
        return actorIndex;
    }

    int? ParseActorVisualState(List<string> tags)
    {
        if(tags.Count < 6) return null;
        if (!int.TryParse(tags[5], out int actorVisualState)) return null;
        return actorVisualState;
    }

    int? ParseActorDestination(List<string> tags)
    {
        if(tags.Count < 7) return null;
        if (!int.TryParse(tags[6], out int actorVisualState)) return null;
        return actorVisualState;
    }

    float ParseActorSpeed(List<string> tags)
    {
        if (tags.Count < 8) return 3.5f;
        if (!float.TryParse(tags[7], out float actorSpeed)) return 3.5f;
        return actorSpeed;
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
