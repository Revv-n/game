using System;
using GreenT.HornyScapes.MergeCore.GameItemBox;
using Merge;
using StripClub.Model;
using TMPro;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes.MergeCore;

public class BubbleActionOperator : ModuleActionOperatorSimple
{
	private Bubble cachedBox;

	[SerializeField]
	private GameObject timerContainer;

	[SerializeField]
	private TMP_Text timer;

	private IDisposable currencyStream;

	public override GIModuleType Type => GIModuleType.Bubble;

	public Bubble BubbleBox { get; private set; }

	public override GIBox.Base GetBox()
	{
		return BubbleBox;
	}

	protected override void Start()
	{
		base.Start();
		currencyStream = CurrencyProcessor.GetCountReactiveProperty(CurrencyType.Hard).Subscribe(SetState);
	}

	private void OnDestroy()
	{
		currencyStream?.Dispose();
	}

	public override void SetBox(GIBox.Base box)
	{
		BubbleBox = box as Bubble;
		SetState(BubbleBox.Data.OpenPrice);
		cachedBox = BubbleBox;
		timerContainer.SetActive(value: true);
		timer.SetActive(active: true);
		block.SetButtonLabelText(BubbleBox.Data.OpenPrice.ToString());
		UpdateTimer();
		BubbleBox.OnTimerTick += AtTimerTick;
		BubbleBox.OnTimerComplete += AtTimerComplete;
		base.IsActive = true;
	}

	private void AtTimerTick(TimerStatus timerInfo)
	{
		UpdateTimer();
	}

	private void UpdateTimer()
	{
		timer.text = Seconds.ToLabeledString(BubbleBox.Data.MainTimer.TimeLeft, 1);
	}

	private void AtTimerComplete(IControlClocks sender)
	{
		Deactivate();
	}

	public override void Deactivate()
	{
		base.Deactivate();
		timerContainer.SetActive(value: false);
		timer.SetActive(active: false);
		if (cachedBox != null)
		{
			cachedBox.OnTimerTick -= AtTimerTick;
			cachedBox.OnTimerComplete -= AtTimerComplete;
		}
		cachedBox = null;
	}
}
