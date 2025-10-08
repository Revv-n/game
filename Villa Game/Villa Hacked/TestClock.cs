using System.Collections;
using UnityEngine;

public class TestClock : MonoBehaviour
{
	[EditorButton("Часы", true)]
	public IEnumerator RunClock(int duration = 3)
	{
		for (int n = 0; n < duration; n++)
		{
			Debug.Log((n % 2 == 0) ? "Tick" : "Tack");
			yield return new WaitForSeconds(1f);
		}
	}
}
