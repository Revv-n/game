using System;
using StripClub.Model;
using TMPro;
using UniRx;
using UnityEngine;

namespace Merge;

public class MixerActionOperator : ModuleActionOperatorSimple
{
	[SerializeField]
	private GameObject timerContainer;

	[SerializeField]
	private TMP_Text timer;

	private IDisposable currencyStream;

	public override GIModuleType Type => GIModuleType.Mixer;

	public GIBox.Mixer MixerBox { get; private set; }

	public override GIBox.Base GetBox()
	{
		return MixerBox;
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
		MixerBox = box as GIBox.Mixer;
		SetState(MixerBox.SpeedUpPrice);
		block.SetButtonLabelText(MixerBox.SpeedUpPrice.ToString());
		timerContainer.SetActive(value: true);
		timer.SetActive(active: true);
		MixerBox.OnTimerActiveChange += AtTimerActiveChange;
		MixerBox.OnTimerTick += AtTick;
		base.gameObject.SetActive(MixerBox.Data.CurrentState == ModuleDatas.Mixer.StateMixer.Mixing);
		base.IsActive = true;
	}

	private void AtTick(TimerStatus obj)
	{
		block.SetButtonLabelText(MixerBox.SpeedUpPrice.ToString());
		UpdateTimer();
	}

	private void UpdateTimer()
	{
		timer.text = Seconds.ToLabeledString(MixerBox.Data.MainTimer.TimeLeft, 1);
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
		if (MixerBox != null)
		{
			MixerBox.OnTimerActiveChange -= AtTimerActiveChange;
			MixerBox.OnTimerTick -= AtTick;
		}
		MixerBox = null;
	}
}
