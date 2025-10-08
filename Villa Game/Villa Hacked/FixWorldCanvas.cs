using UnityEngine;

[ExecuteInEditMode]
public class FixWorldCanvas : MonoBehaviour
{
	private void OnEnable()
	{
		GetComponent<Canvas>().worldCamera = Camera.main;
	}
}
