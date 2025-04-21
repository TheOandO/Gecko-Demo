using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
public class DialogueCharacter
{
	public string name;
	public Sprite icon;
}

[System.Serializable]
public class DialogueLine
{
	public DialogueCharacter character;
	[TextArea(3, 10)]
	public string line;
}

[System.Serializable]
public class Dialogue
{
	public List<DialogueLine> dialogueLines = new List<DialogueLine>();
}

public class DialogueTrigger : MonoBehaviour
{
	[SerializeField] private PlayableDirector playableDirector;

	public Dialogue dialogue;

	public void TriggerDialogue()
	{
		DialogueManager.Instance.StartDialogue(dialogue, playableDirector);
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
			gameObject.SetActive(false);
			TriggerDialogue();
		}
	}
}
