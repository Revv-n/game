using System;
using UnityEngine;

[ExecuteInEditMode]
public class ScreenShot : MonoBehaviour
{
	public int multiply = 1;

	private int i;

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			ScreenCapture.CaptureScreenshot("screenshot" + DateTime.Now.GetHashCode() + ".png", multiply);
			i++;
			Debug.Log("Cheeeseee:-)");
		}
	}
}
