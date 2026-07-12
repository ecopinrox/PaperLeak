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

            if(!TryParseTags(tags, out int characterIndex, out int portraitIndex))
            {
                Debug.LogWarning($"Error with tag parsing at line \"{dialogue}\"");
            }

            yield return new(dialogue, GetName(characterIndex), GetPortrait(characterIndex, portraitIndex));
        }
    }

    bool TryParseTags(List<string> tags, out int characterIndex, out int portraitIndex)
    {
        characterIndex = 0;
        portraitIndex = 0;

        if (tags.Count > 1)
        {
            return int.TryParse(tags[0], out characterIndex) && int.TryParse(tags[1], out portraitIndex);
        }
        else if (tags.Count > 0)
        {
            return int.TryParse(tags[0], out portraitIndex);
        }

        return true;
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
