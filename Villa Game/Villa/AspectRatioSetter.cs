using UnityEngine;

public class AspectRatioSetter
{
	public void SetRatio(float w, float h)
	{
		if ((float)Screen.width / (float)Screen.height > w / h)
		{
			Screen.SetResolution((int)((float)Screen.height * (w / h)), Screen.height, fullscreen: true);
		}
		else
		{
			Screen.SetResolution(Screen.width, (int)((float)Screen.width * (h / w)), fullscreen: true);
		}
	}
}
