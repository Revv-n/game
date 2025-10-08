using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Merge.Core.Events;

public class EventMainButton : MonoBehaviour
{
	[SerializeField]
	private Button button;

	[SerializeField]
	private Text timeLabel;

	public void AddCallback(UnityAction callback)
	{
		button.AddClickCallback(callback);
	}

	public void SetTimerStatus(TimerStatus status)
	{
		timeLabel.text = Seconds.ToLabeledString(status.TimeLeft);
	}

	public void SetTimeActive(bool active)
	{
		timeLabel.SetActive(active);
	}
}
