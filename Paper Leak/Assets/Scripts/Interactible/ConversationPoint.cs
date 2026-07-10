using UnityEngine;

public class ConversationPoint : Interactible
{
    bool interacted = false;

    public override void Interact()
    {
        if (!interacted)
        {
            Debug.Log("conversation start");
            interacted = true;
        }
        else
        {
            Debug.Log("conversation start again");
        }
    }
}
