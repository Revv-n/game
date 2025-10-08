using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.Cheats.UI;

public class RewindTimeCheat : MonoBehaviour
{
	[SerializeField]
	protected TMP_InputField mainInputField;

	[SerializeField]
	protected TMP_InputField daysInputField;

	[SerializeField]
	protected TMP_InputField hoursInputField;

	[SerializeField]
	protected TMP_InputField minutesInputField;

	[SerializeField]
	protected TMP_InputField secondsInputField;

	[SerializeField]
	protected Button rewindButton;

	private TimeSpan time;

	private TimeRewinder rewindTime;

	[Inject]
	public void Init(TimeRewinder rewindTime)
	{
		this.rewindTime = rewindTime;
	}

	public void Apply()
	{
		SetTime();
		Rewind();
	}

	public void Rewind()
	{
		if (!time.Equals(TimeSpan.Zero))
		{
			rewindTime.Rewind(time);
			time = TimeSpan.Zero;
			mainInputField.text = string.Empty;
			daysInputField.text = string.Empty;
			hoursInputField.text = string.Empty;
			minutesInputField.text = string.Empty;
			secondsInputField.text = string.Empty;
		}
	}

	private void SetTime()
	{
		if (!string.IsNullOrEmpty(mainInputField.text))
		{
			string text = mainInputField.text;
			TimeSpan result2;
			if (int.TryParse(text, out var result))
			{
				time = TimeSpan.FromSeconds(result);
			}
			else if (TimeSpan.TryParse(text, out result2))
			{
				time = result2;
			}
		}
		else
		{
			int.TryParse(daysInputField.text, out var result3);
			int.TryParse(hoursInputField.text, out var result4);
			int.TryParse(minutesInputField.text, out var result5);
			int.TryParse(secondsInputField.text, out var result6);
			time = default(TimeSpan);
			time = time.Add(TimeSpan.FromDays(result3));
			time = time.Add(TimeSpan.FromHours(result4));
			time = time.Add(TimeSpan.FromMinutes(result5));
			time = time.Add(TimeSpan.FromSeconds(result6));
		}
	}

	protected virtual void OnEnable()
	{
		mainInputField.onValueChanged.AddListener(Validate);
		daysInputField.onValueChanged.AddListener(Validate);
		hoursInputField.onValueChanged.AddListener(Validate);
		minutesInputField.onValueChanged.AddListener(Validate);
		secondsInputField.onValueChanged.AddListener(Validate);
		rewindButton.onClick.AddListener(Apply);
	}

	protected virtual void OnDisable()
	{
		mainInputField.onValueChanged.RemoveListener(Validate);
		daysInputField.onValueChanged.RemoveListener(Validate);
		hoursInputField.onValueChanged.RemoveListener(Validate);
		minutesInputField.onValueChanged.RemoveListener(Validate);
		secondsInputField.onValueChanged.RemoveListener(Validate);
		rewindButton.onClick.RemoveListener(Apply);
	}

	public virtual void Validate(string param)
	{
		rewindButton.interactable = int.TryParse(param, out var _);
	}
}
