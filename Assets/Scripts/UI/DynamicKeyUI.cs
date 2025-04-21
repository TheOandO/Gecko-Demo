using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class DynamicKeyUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tutorialText;
	[SerializeField] private InputActionReference moveActionReference;

	[SerializeField] private string tutorialMessage = "Press {0}/{1} to .";

	private void Update()
	{
		UpdateTutorialText();
	}

	private void UpdateTutorialText()
	{
		string keyboardBinding = GetBindingForControlScheme("Keyboard&Mouse");
		string controllerBinding = GetBindingForControlScheme("Gamepad");

		// Update the tutorial text dynamically based on the template
		tutorialText.text = string.Format(tutorialMessage, keyboardBinding, controllerBinding);
	}

	private string GetBindingForControlScheme(string controlScheme)
	{
		InputAction action = moveActionReference.action;

		for (int i = 0; i < action.bindings.Count; i++)
		{
			var binding = action.bindings[i];

			if (binding.groups.Contains(controlScheme))
			{
				return binding.ToDisplayString();
			}
		}

		return "Unknown";
	}
}
