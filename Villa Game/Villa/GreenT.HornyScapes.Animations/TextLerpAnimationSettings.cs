using System;

namespace GreenT.HornyScapes.Animations;

[Serializable]
public class TextLerpAnimationSettings
{
	public int From;

	public int To;

	public float Duration;

	public string FormatedString;

	public TextLerpAnimationSettings(int from, int to, float duration, string formatedString)
	{
		From = from;
		To = to;
		Duration = duration;
		FormatedString = formatedString;
	}
}
