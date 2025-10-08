using System;
using StripClub.Extensions;
using StripClub.Model;
using StripClub.UI;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Messenger.UI;

public class ReplyEnergyView : MonoBehaviour
{
	[SerializeField]
	private TMP_Text value;

	[SerializeField]
	private TMP_Text maxValue;

	[SerializeField]
	private LocalizedTextMeshPro counter;

	private RestorableValue<int> messages;

	private IDisposable counterStream;

	[Inject]
	private void Init(StripClub.Model.IPlayerBasics playerBasics)
	{
		messages = playerBasics.Replies;
		if (base.gameObject.activeInHierarchy)
		{
			Init();
		}
	}

	private void SetMaxReplyEnergyView(int minValue, int maxValue)
	{
		this.maxValue.text = maxValue.ToString();
	}

	private void SetReplyEnergyView(int value)
	{
		this.value.text = value.ToString();
		DisplayTimeUntilNextRestore();
	}

	private void SetCounter(TimeSpan timeUntilNextRestore)
	{
		counter.gameObject.SetActive(timeUntilNextRestore > TimeSpan.Zero);
		string text = timeUntilNextRestore.ToString("m\\:ss");
		counter.SetArguments(text);
	}

	private void OnEnable()
	{
		if (messages != null)
		{
			Init();
		}
	}

	private void Init()
	{
		SetReplyEnergyView(messages.Value);
		messages.OnValueChanged += SetReplyEnergyView;
		SetMaxReplyEnergyView(messages.Min, messages.Max);
		messages.OnBoundsChanged += SetMaxReplyEnergyView;
	}

	private void DisplayTimeUntilNextRestore()
	{
		counterStream?.Dispose();
		counterStream = Observable.Timer(TimeSpan.Zero, messages.CheckFrequency, Scheduler.MainThreadIgnoreTimeScale).TakeWhile((long _) => messages.TimeUntilNextRestore > TimeSpan.Zero).DoOnCompleted(delegate
		{
			SetCounter(messages.TimeUntilNextRestore);
		})
			.Subscribe(delegate
			{
				SetCounter(messages.TimeUntilNextRestore);
			})
			.AddTo(this);
	}

	private void OnDisable()
	{
		messages.OnValueChanged -= SetReplyEnergyView;
		messages.OnBoundsChanged -= SetMaxReplyEnergyView;
		counterStream?.Dispose();
	}
}
