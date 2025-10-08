using System;
using UnityEngine;
using UnityEngine.UI;

public class VibrationExample : MonoBehaviour
{
	public Text inputTime;

	public Text inputPattern;

	public Text inputRepeat;

	public Text txtAndroidVersion;

	private void Start()
	{
		Vibration.Init();
		Debug.Log("Application.isMobilePlatform: " + Application.isMobilePlatform);
	}

	private void Update()
	{
	}

	public void TapVibrate()
	{
		Vibration.Vibrate();
	}

	public void TapVibrateCustom()
	{
		Vibration.Vibrate(int.Parse(inputTime.text));
	}

	public void TapVibratePattern()
	{
		long[] array = Array.ConvertAll(inputPattern.text.Replace(" ", "").Split(','), long.Parse);
		Debug.Log(array.Length);
		Vibration.Vibrate(array, int.Parse(inputRepeat.text));
	}

	public void TapCancelVibrate()
	{
		Vibration.Cancel();
	}

	public void TapPopVibrate()
	{
		Vibration.VibratePop();
	}

	public void TapPeekVibrate()
	{
		Vibration.VibratePeek();
	}

	public void TapNopeVibrate()
	{
		Vibration.VibrateNope();
	}
}
