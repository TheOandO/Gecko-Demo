using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

public class DialogueManager : MonoBehaviour
{
	public static DialogueManager Instance;

	public Image characterIcon;
	public TextMeshProUGUI characterName;
	public TextMeshProUGUI dialogueArea;

	private Queue<DialogueLine> lines;
	public bool isDialogueActive = false;

	public float typingSpeed = 0.2f;
	public Animator animator;

	private bool isTyping = false;
	private string currentLine;

	private PlayerInput playerInput;
	[SerializeField] GameObject submitButton;

	private PlayableDirector playableDirector;

	private void Awake()
	{
		if (Instance == null)
			Instance = this;

		lines = new Queue<DialogueLine>();

		playerInput = FindObjectOfType<PlayerInput>();
	}

	public void StartDialogue(Dialogue dialogue, PlayableDirector playableDirector = null)
	{
		if (Time.timeScale == 0) return;

		isDialogueActive = true;
		animator.Play("show");
		lines.Clear();
		GameManager.Instance.timer.PauseTimer();

		submitButton.SetActive(false);

		foreach (DialogueLine dialogueLine in dialogue.dialogueLines)
		{
			lines.Enqueue(dialogueLine);
		}

		playerInput.SwitchCurrentActionMap("UI");

		this.playableDirector = playableDirector;

		DisplayNextDialogueLine();
	}

	public void DisplayNextDialogueLine()
	{
		if (isTyping || Time.timeScale == 0)
		{
			return;
		}

		if (lines.Count == 0)
		{
			if (isDialogueActive)
			{
				EndDialogue();

			}
			
			return;
		}

		DialogueLine currentLine = lines.Dequeue();
		characterIcon.sprite = currentLine.character.icon;
		characterName.text = currentLine.character.name;

		StopAllCoroutines();
		StartCoroutine(TypeSentence(currentLine));
	}

	IEnumerator TypeSentence(DialogueLine dialogueLine)
	{
		dialogueArea.text = "";
		currentLine = dialogueLine.line;
		isTyping = true;

		submitButton.SetActive(false);

		foreach (char letter in dialogueLine.line.ToCharArray())
		{
			dialogueArea.text += letter;
			AudioManager.Instance.PlaySFX(AudioManager.Instance.voice);
			yield return new WaitForSeconds(typingSpeed);
		}

		isTyping = false;
		submitButton.SetActive(true);
	}

	void EndDialogue()
	{
		isDialogueActive = false;
		animator.Play("hide");
		GameManager.Instance.timer.ResumeTimer();

		playerInput.SwitchCurrentActionMap("Player");

		if (playableDirector != null)
		{
			playableDirector.Play();
		}
	}
}

