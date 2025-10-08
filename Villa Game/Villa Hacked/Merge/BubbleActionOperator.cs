using System;
using StripClub.Model;
using TMPro;
using UniRx;
using UnityEngine;

namespace Merge;

public class BubbleActionOperator : ModuleActionOperatorSimple
{
	private GIBox.Bubble cachedBox;

	[SerializeField]
	private TMP_Text timer;

	private IDisposable currencyStream;

	public override GIModuleType Type => GIModuleType.Bubble;

	public GIBox.Bubble BubbleBox { get; private set; }

	public override GIBox.Base GetBox()
	{
		return BubbleBox;
	}

	protected override void Start()
	{
		currencyStream = ObservableExtensions.Subscribe<int>((IObservable<int>)CurrencyProcessor.GetCountReactiveProperty(CurrencyType.Hard), (Action<int>)SetState);
	}

	private void OnDestroy()
	{
		currencyStream?.Dispose();
	}

	public override void SetBox(GIBox.Base box)
	{
		BubbleBox = box as GIBox.Bubble;
		SetState(BubbleBox.Data.OpenPrice);
		cachedBox = BubbleBox;
		timer.SetActive(active: true);
		block.SetButtonLabelText(BubbleBox.Data.OpenPrice.ToString());
		UpdateTimer();
		BubbleBox.OnTimerTick += AtTimerTick;
		BubbleBox.OnTimerComplete += AtTimerComplete;
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
		timer.SetActive(active: false);
		if (cachedBox != null)
		{
			cachedBox.OnTimerTick -= AtTimerTick;
			cachedBox.OnTimerComplete -= AtTimerComplete;
		}
		cachedBox = null;
	}
}
