using System.Linq;
using UnityEngine;

namespace StripClub.Stories;

public class SpeakersView : MonoBehaviour
{
	[SerializeField]
	[Tooltip("1 - left1, 2 - left2, 3 - right1, 4 - right2")]
	private SpeakerView[] speakers = new SpeakerView[4];

	private SpeakerView _speakerView;

	public void ApplySpeaker(int characterID, bool isFirstApply = false)
	{
		if (!isFirstApply)
		{
			SpeakerView speakerView = _speakerView;
			if ((object)speakerView != null && speakerView.CharacterID == characterID)
			{
				_speakerView?.UnFade();
				return;
			}
		}
		if ((bool)_speakerView)
		{
			_speakerView.Fade();
		}
		_speakerView = speakers.FirstOrDefault((SpeakerView s) => s.CharacterID == characterID);
		_ = _speakerView.transform.parent;
		Vector3 localPosition = _speakerView.transform.localPosition;
		_speakerView.transform.localPosition = localPosition;
		_speakerView.UnFade();
	}

	public void InitSpeakers(int[,] speakers)
	{
		SpeakerView[] array = this.speakers;
		foreach (SpeakerView obj in array)
		{
			obj.gameObject.SetActive(value: false);
			obj.CharacterID = -1;
		}
		for (int j = 0; j <= speakers.GetUpperBound(0); j++)
		{
			SpeakerView obj2 = this.speakers[speakers[j, 1]];
			int speaker = speakers[j, 0];
			obj2.SetSpeaker(speaker);
		}
	}

	public bool HasSpeakers(int[,] speakers)
	{
		bool flag = false;
		for (int i = 0; i <= speakers.GetUpperBound(0); i++)
		{
			SpeakerView speakerView = this.speakers[speakers[i, 1]];
			int num = speakers[i, 0];
			flag |= speakerView.CharacterID == num;
		}
		return flag;
	}
}
