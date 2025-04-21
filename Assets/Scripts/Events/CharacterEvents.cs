using UnityEngine;
using UnityEngine.Events;

public class CharacterEvents
{
	public static UnityAction<GameObject, int> characterDmged;
	public static UnityAction<GameObject, int> characterHealed;
}
