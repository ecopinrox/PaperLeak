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


    string GetName(int characterIndex)
    {
        return characterPortraitCollections[characterIndex].Name;
    }

    Sprite GetPortrait(int characterIndex, int portraitIndex)
    {
        return characterPortraitCollections[characterIndex].GetPortrait(portraitIndex);
    }
}
