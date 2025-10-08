using System.Collections;
using UnityEngine;

public class CoroutineProvider : MonoBehaviour
{
	public static CoroutineProvider DoCoroutine(IEnumerator crt, string name = "CoroutineProvider")
	{
		CoroutineProvider coroutineProvider = new GameObject(name).AddComponent<CoroutineProvider>();
		coroutineProvider.DoCoroutine(crt);
		return coroutineProvider;
	}

	public void Stop()
	{
		StopAllCoroutines();
	}

	public void Destroy()
	{
		Object.Destroy(base.gameObject);
	}

	private void DoCoroutine(IEnumerator crt)
	{
		StartCoroutine(crt);
	}
}
