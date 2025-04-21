using UnityEngine;

public class TorchToggle : MonoBehaviour
{
	private Animator animator;

	[SerializeField]
	private bool isLit = false;
	[SerializeField]
	private GameObject torchLight;

	public bool IsLit
	{
		get
		{
			return isLit;
		}
		private set
		{
			if (isLit != value)
			{
				isLit = value;

				animator.SetBool("isLit", value);

				torchLight.SetActive(isLit);
			}

		}
	}
	void Start()
	{
		animator = GetComponent<Animator>();
		torchLight.SetActive(isLit);
		animator.SetBool("isLit", isLit);
	}
}
