using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    public bool OpenMenuInput { get; private set; }
    public bool CloseMenuInput { get; private set; }

	private PlayerInput _playerInput;

    private InputAction _openMenuAction;
    private InputAction _closeMenuAction;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}

		_playerInput = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInput>();
		_openMenuAction = _playerInput.actions["OpenMenu"];
		_closeMenuAction = _playerInput.actions["Escape"];
	}

	private void Update()
	{
		OpenMenuInput = _openMenuAction.WasPressedThisFrame();
		CloseMenuInput = _closeMenuAction.WasPressedThisFrame();
	}
}
