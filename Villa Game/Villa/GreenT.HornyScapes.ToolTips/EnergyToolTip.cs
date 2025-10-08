using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using GreenT.Bonus;
using GreenT.HornyScapes.Bank.UI;
using GreenT.HornyScapes.Booster;
using GreenT.HornyScapes.Booster.Effect;
using GreenT.HornyScapes.Events;
using GreenT.UI;
using StripClub.Extensions;
using StripClub.UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.ToolTips;

public class EnergyToolTip : AnimatedToolTipView<TailedToolTipSettings>
{
	internal class Manager : MonoViewManager<TailedToolTipSettings, EnergyToolTip>
	{
	}

	private const int BoosterDisabled = 0;

	private const int BoosterEnabled = 1;

	[SerializeField]
	private LocalizedTextMeshPro localizedText;

	[SerializeField]
	private RectTransform _tail;

	[SerializeField]
	private MonoTimer _energyCapTimer;

	[SerializeField]
	private StatableComponentGroup _energyCapStatable;

	[SerializeField]
	private MonoTimer _energyRechargeTimer;

	[SerializeField]
	private StatableComponentGroup _energyRechargeStatable;

	[SerializeField]
	private int _goToTabID;

	[SerializeField]
	private OpenSection _sectionOpener;

	[SerializeField]
	private WindowOpener _windowOpener;

	[SerializeField]
	private MonoContentSetter _contentSetter;

	[SerializeField]
	private Button _openSectionButton;

	private IDisposable _boosterSetupStream;

	private Sequence showSequence;

	private TimeHelper _timeHelper;

	private BoosterStorage _boosterStorage;

	[Inject]
	private void Construct(BoosterStorage boosterStorage, TimeHelper timeHelper)
	{
		_boosterStorage = boosterStorage;
		_timeHelper = timeHelper;
	}

	public override void Set(TailedToolTipSettings settings)
	{
		base.Set(settings);
		SetupText(settings);
		SetupTail();
		SetupBoosterInfo();
		SetupButton();
		_boosterSetupStream?.Dispose();
		_boosterSetupStream = _boosterStorage.Collection.ObserveEveryValueChanged((IEnumerable<BoosterModel> collection) => collection.Count()).Subscribe(delegate
		{
			SetupBoosterInfo();
		});
		showAnimation.Init();
		hideAnimation.Init();
	}

	private void SetupButton()
	{
		_sectionOpener.Set(_goToTabID);
		_openSectionButton.OnClickAsObservable().Subscribe(delegate
		{
			_contentSetter.Set();
			_windowOpener.OpenOnly();
			_sectionOpener.Open();
		}).AddTo(this);
	}

	private void SetupBoosterInfo()
	{
		TimeSpan value;
		bool maxBoosterTime = GetMaxBoosterTime(BonusType.IncreaseBaseEnergy, out value);
		if (maxBoosterTime)
		{
			_energyCapTimer.Init(value, _timeHelper.UseCombineFormat);
		}
		_energyCapStatable.Set(maxBoosterTime ? 1 : 0);
		bool maxBoosterTime2 = GetMaxBoosterTime(BonusType.IncreaseEnergyRechargeSpeed, out value);
		if (maxBoosterTime2)
		{
			_energyRechargeTimer.Init(value, _timeHelper.UseCombineFormat);
		}
		_energyRechargeStatable.Set(maxBoosterTime2 ? 1 : 0);
	}

	private bool GetMaxBoosterTime(BonusType bonusType, out TimeSpan value)
	{
		value = default(TimeSpan);
		BoosterModel boosterModel = (from model in _boosterStorage.Collection
			where model.Bonus is BoosterIncrementBonus boosterIncrementBonus && boosterIncrementBonus.BonusType == bonusType
			orderby model.Timer.TimeLeft.Seconds
			select model).LastOrDefault();
		if (boosterModel == null)
		{
			return false;
		}
		value = boosterModel.Timer.TimeLeft;
		return true;
	}

	private void SetupTail()
	{
		if (base.Source.TailSettings.TailPosition != Vector3.zero)
		{
			Vector2 anchoredPosition = new Vector2(base.Source.TailSettings.TailPosition.x, _tail.anchoredPosition.y);
			_tail.anchoredPosition = anchoredPosition;
		}
	}

	private void SetupText(TailedToolTipSettings settings)
	{
		if (settings != null)
		{
			base.transform.localPosition = settings.ToolTipPosition;
			string keyText = settings.KeyText;
			localizedText.Init(keyText);
		}
	}

	public override void Display(bool display)
	{
		if (display)
		{
			Show();
		}
		else
		{
			Hide();
		}
	}

	private void Show()
	{
		hideAnimation.Stop();
		showSequence = showAnimation.Play();
		base.Display(display: true);
	}

	private void Hide()
	{
		if (showSequence != null && showSequence.IsActive() && showSequence.IsPlaying())
		{
			showSequence.OnComplete(delegate
			{
				hideAnimation.Play().OnComplete(delegate
				{
					base.Display(display: false);
				});
			});
		}
		else
		{
			hideAnimation.Play().OnComplete(delegate
			{
				base.Display(display: false);
			});
		}
	}
}
