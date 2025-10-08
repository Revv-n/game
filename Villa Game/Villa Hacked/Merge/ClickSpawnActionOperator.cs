using System;
using StripClub.Model;
using TMPro;
using UniRx;
using UnityEngine;

namespace Merge;

public class ClickSpawnActionOperator : ModuleActionOperatorSimple
{
	[SerializeField]
	private GameObject timerContainer;

	[SerializeField]
	private TMP_Text timer;

	private IDisposable currencyStream;

	public override GIModuleType Type => GIModuleType.ClickSpawn;

	public GIBox.ClickSpawn ClickSpawnBox { get; private set; }

	public override GIBox.Base GetBox()
	{
		return ClickSpawnBox;
	}

	protected override void Start()
	{
		base.Start();
		currencyStream = ObservableExtensions.Subscribe<int>((IObservable<int>)CurrencyProcessor.GetCountReactiveProperty(CurrencyType.Hard), (Action<int>)SetState);
	}

	private void OnDestroy()
	{
		currencyStream?.Dispose();
	}

	public override void SetBox(GIBox.Base box)
	{
		ClickSpawnBox = box as GIBox.ClickSpawn;
		SetState(ClickSpawnBox.SpeedUpPrice);
		block.SetButtonLabelText(ClickSpawnBox.SpeedUpPrice.ToString());
		timerContainer.SetActive(value: true);
		timer.SetActive(active: true);
		ClickSpawnBox.OnTimerActiveChange += AtTimerActiveChange;
		ClickSpawnBox.OnTimerTick += AtTick;
		base.gameObject.SetActive(ClickSpawnBox.IsTimerVisible);
		base.IsActive = true;
	}

	private void AtTick(TimerStatus obj)
	{
		block.SetButtonLabelText(ClickSpawnBox.SpeedUpPrice.ToString());
		UpdateTimer();
	}

	private void UpdateTimer()
	{
		timer.text = Seconds.ToLabeledString(ClickSpawnBox.Data.MainTimer.TimeLeft, 1);
	}

	private void AtTimerActiveChange(bool active)
	{
		base.gameObject.SetActive(active);
		if (!active)
		{
			Deactivate();
		}
	}

	public override void Deactivate()
	{
		base.Deactivate();
		timerContainer.SetActive(value: false);
		timer.SetActive(active: false);
		if (ClickSpawnBox != null)
		{
			ClickSpawnBox.OnTimerActiveChange -= AtTimerActiveChange;
			ClickSpawnBox.OnTimerTick -= AtTick;
		}
		ClickSpawnBox = null;
	}
}
