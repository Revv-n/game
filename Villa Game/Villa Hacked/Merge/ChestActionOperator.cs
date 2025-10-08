using System;
using System.Collections;
using StripClub.Model;
using StripClub.Model.Shop;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Merge;

public class ChestActionOperator : ModuleActionOperator
{
	[SerializeField]
	protected ModuleActionBlock block;

	[SerializeField]
	private GameObject openingBtnView;

	[SerializeField]
	private Button openButton;

	[SerializeField]
	private GameObject closedBtnView;

	[SerializeField]
	private Button closeButton;

	[SerializeField]
	private TMP_Text price;

	[SerializeField]
	private GameObject timerContainer;

	[SerializeField]
	private TMP_Text timerView;

	private GIBox.Chest cachedBox;

	[Inject]
	protected ICurrencyProcessor CurrencyProcessor;

	private IDisposable currencyStream;

	[Inject]
	private ICurrencyProcessor currencyProcessor;

	public override GIModuleType Type => GIModuleType.Chest;

	public GIBox.Chest ChestBox { get; private set; }

	public override event Action<ModuleActionOperator> OnAction;

	public override GIBox.Base GetBox()
	{
		return ChestBox;
	}

	private void Start()
	{
		openButton.AddClickCallback(AtButtonClick);
		closeButton.AddClickCallback(AtButtonClick);
		currencyStream = ObservableExtensions.Subscribe<int>((IObservable<int>)CurrencyProcessor.GetCountReactiveProperty(CurrencyType.Hard), (Action<int>)SetState);
		void AtButtonClick()
		{
			OnAction?.Invoke(this);
		}
	}

	private void OnDestroy()
	{
		currencyStream?.Dispose();
	}

	protected virtual void SetState(int price)
	{
		block.StateView.SetValueColor((!currencyProcessor.IsEnough(CurrencyType.Hard, price)) ? 1 : 0);
	}

	public override void SetBox(GIBox.Base box)
	{
		StartCoroutine(CRT_Rebuild());
		CacheBox(box);
		SetState(ChestBox.SpeedUpPrice);
		bool isOpeningNow = ChestBox.Data.IsOpeningNow;
		bool alreadyOpened = ChestBox.Data.AlreadyOpened;
		bool active = !isOpeningNow && !alreadyOpened;
		openingBtnView.SetActive(isOpeningNow);
		closedBtnView.SetActive(active);
		base.IsActive = true;
		if (isOpeningNow)
		{
			timerContainer.SetActive(value: true);
			timerView.SetActive(active: true);
			UpdateTimer();
			UpdatePrice();
			ChestBox.OnTimerTick += AtTimerTick;
			ChestBox.OnTimerComplete += AtTimerComplete;
		}
	}

	private void UpdateTimer()
	{
		timerView.text = Seconds.ToLabeledString(ChestBox.Data.MainTimer.TimeLeft, 1);
	}

	private void UpdatePrice()
	{
		price.text = ChestBox.SpeedUpPrice.ToString("F0");
	}

	private IEnumerator CRT_Rebuild()
	{
		yield return new WaitForEndOfFrame();
	}

	private void CacheBox(GIBox.Base box)
	{
		if (ChestBox != null)
		{
			ChestBox.OnTimerTick -= AtTimerTick;
			ChestBox.OnTimerComplete -= AtTimerComplete;
		}
		ChestBox = box as GIBox.Chest;
		cachedBox = ChestBox;
	}

	private void AtTimerTick(TimerStatus timerInfo)
	{
		UpdateTimer();
		UpdatePrice();
	}

	private void AtTimerComplete(IControlClocks sender)
	{
		Deactivate();
	}

	public override void Deactivate()
	{
		base.Deactivate();
		timerContainer.SetActive(value: false);
		timerView.SetActive(active: false);
		if (cachedBox != null)
		{
			cachedBox.OnTimerTick -= AtTimerTick;
			cachedBox.OnTimerComplete -= AtTimerComplete;
		}
		cachedBox = null;
	}
}
