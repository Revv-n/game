using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.Bank.UI;
using GreenT.HornyScapes.Sellouts.Mappers;
using GreenT.HornyScapes.Sellouts.Models;
using GreenT.HornyScapes.Sellouts.Providers;
using GreenT.HornyScapes.Sellouts.Services;
using GreenT.HornyScapes.Sellouts.Views;
using GreenT.Localizations;
using GreenT.UI;
using StripClub.UI;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.Sellouts.Windows;

public sealed class SelloutUiSetter : MonoView<Sellout>
{
	private const int Zero = 0;

	private const int One = 1;

	private const string TitleKey = "ui.sellout_{0}.name";

	[SerializeField]
	private Image _background;

	[SerializeField]
	private TMP_Text[] _titles;

	[SerializeField]
	private Button _tutorialButton;

	[SerializeField]
	private Button _bankButton;

	[SerializeField]
	private Image _currencyIcon;

	[SerializeField]
	private Image _leftReward;

	[SerializeField]
	private Image _rightReward;

	[SerializeField]
	private Canvas _leftRewardCanvas;

	[SerializeField]
	private Canvas _rewardsCanvas;

	[SerializeField]
	private OpenSection _sectionOpener;

	[SerializeField]
	private CanvasOrderModifier _leftRewardCanvasOrderModifier;

	[SerializeField]
	private CanvasOrderModifier _rewardsCanvasOrderModifier;

	[SerializeField]
	private SelloutScrollSnapController _scrollSnapController;

	private SelloutMapperProvider _selloutMapperProvider;

	private SelloutRewardsMapperProvider _selloutRewardsMapperProvider;

	private SelloutRewardsTracker _selloutRewardsTracker;

	private RewardViewProvider _rewardViewProvider;

	private IWindowsManager _windowsManager;

	private LocalizationService _localizationService;

	private SelloutTutorialWindow _selloutTutorialWindow;

	private IDisposable _trackerStream;

	private readonly List<SelloutRewardView> _rewardViews = new List<SelloutRewardView>(32);

	private readonly CompositeDisposable _disposables = new CompositeDisposable();

	[Inject]
	private void Init(SelloutMapperProvider selloutMapperProvider, SelloutRewardsMapperProvider selloutRewardsMapperProvider, SelloutRewardsTracker selloutRewardsTracker, RewardViewProvider rewardViewProvider, IWindowsManager windowsManager, LocalizationService localizationService)
	{
		_selloutMapperProvider = selloutMapperProvider;
		_selloutRewardsMapperProvider = selloutRewardsMapperProvider;
		_selloutRewardsTracker = selloutRewardsTracker;
		_rewardViewProvider = rewardViewProvider;
		_windowsManager = windowsManager;
		_localizationService = localizationService;
	}

	public override void Set(Sellout sellout)
	{
		base.Set(sellout);
		SelloutBundleData bundleData = sellout.BundleData;
		_background.sprite = bundleData.BackgroundSprite;
		_currencyIcon.sprite = bundleData.CurrencySprite;
		_leftReward.sprite = bundleData.LeftRewardSprite;
		_rightReward.sprite = bundleData.RightRewardSprite;
		_sectionOpener.Set(sellout.GoToBankTab);
		_rewardViews.Clear();
		_scrollSnapController.Clear();
		_rewardViewProvider.HideAll();
		List<SelloutRewardsInfo> rewardBlocks = sellout.Rewards;
		List<int> rewardIds = GetRewardIds();
		List<int> rewardFrameIds = GetRewardFrameIds();
		for (int i = 0; i < rewardBlocks.Count; i++)
		{
			SelloutRewardsInfo rewardInfos = rewardBlocks[i];
			SelloutRewardView selloutRewardView = _rewardViewProvider.Display(rewardIds[i], rewardInfos);
			_rewardViews.Add(selloutRewardView);
			_scrollSnapController.Add(selloutRewardView);
			int startValue = ((i != 0) ? sellout.RequirementPoints[i - 1] : 0);
			int endValue = sellout.RequirementPoints[i];
			selloutRewardView.SetTargetPoints(sellout, startValue, endValue);
			selloutRewardView.SetFrame(rewardFrameIds[i]);
			DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<SelloutRewardView>(selloutRewardView.ProgressChanged, (Action<SelloutRewardView>)delegate
			{
				_scrollSnapController.SnapToAppropriateReward();
			}), (ICollection<IDisposable>)_disposables);
			DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<int>(selloutRewardView.RewardClaimed, (Action<int>)delegate
			{
				_scrollSnapController.SnapToAppropriateReward();
			}), (ICollection<IDisposable>)_disposables);
			selloutRewardView.transform.SetAsLastSibling();
		}
		_rewardViewProvider.VisibleViews.First((SelloutRewardView x) => x.Source.RewardInfo == rewardBlocks[0]).SetAsFirstInTrack();
		_selloutRewardsTracker.Set(sellout);
		_trackerStream?.Dispose();
		_trackerStream = ObservableExtensions.Subscribe<SelloutRewardsInfo>(_selloutRewardsTracker.ActiveRewardsChanged, (Action<SelloutRewardsInfo>)CheckReward);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<bool>((IObservable<bool>)sellout.CanPointsTrack, (Action<bool>)delegate(bool canTrack)
		{
			SetActiveBankButton(canTrack);
		}), (ICollection<IDisposable>)_disposables);
		SetActiveBankButton(sellout.CanPointsTrack.Value);
		string key = $"ui.sellout_{sellout.ID}.name";
		TMP_Text[] titles = _titles;
		foreach (TMP_Text title in titles)
		{
			DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<string>((IObservable<string>)_localizationService.ObservableText(key), (Action<string>)delegate(string text)
			{
				title.text = text;
			}), (ICollection<IDisposable>)_disposables);
		}
	}

	private void OnEnable()
	{
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Unit>(Observable.TakeUntilDisable<Unit>(UnityUIComponentExtensions.OnClickAsObservable(_tutorialButton), (Component)this), (Action<Unit>)delegate
		{
			OpenTutorial();
		}), (ICollection<IDisposable>)_disposables);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Unit>(Observable.TakeUntilDisable<Unit>(UnityUIComponentExtensions.OnClickAsObservable(_bankButton), (Component)this), (Action<Unit>)delegate
		{
			OpenBank();
		}), (ICollection<IDisposable>)_disposables);
		SelloutBundleData selloutBundleData = base.Source?.BundleData;
		_leftRewardCanvasOrderModifier.Check(selloutBundleData != null && selloutBundleData.IsLeftRewardForeground);
		_rewardsCanvasOrderModifier.Check(selloutBundleData != null && !selloutBundleData.IsRightRewardForeground);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Unit>(Observable.ObserveOnMainThread<Unit>(Observable.NextFrame((FrameCountType)0)), (Action<Unit>)delegate
		{
			Activate();
		}), (ICollection<IDisposable>)_disposables);
	}

	private void OnDestroy()
	{
		_disposables.Dispose();
		_trackerStream?.Dispose();
	}

	private void Activate()
	{
		if (base.Source != null)
		{
			base.Source.SetShowed();
			_scrollSnapController.SnapToAppropriateReward();
		}
	}

	private List<int> GetRewardIds()
	{
		return GetRewardData((SelloutRewardsMapper mapper) => mapper.id);
	}

	private List<int> GetRewardFrameIds()
	{
		return GetRewardData((SelloutRewardsMapper mapper) => mapper.frame_id);
	}

	private List<T> GetRewardData<T>(Func<SelloutRewardsMapper, T> selector)
	{
		int[] rewards_id = _selloutMapperProvider.Get(base.Source.ID).rewards_id;
		List<T> list = new List<T>(rewards_id.Length);
		int[] array = rewards_id;
		foreach (int id in array)
		{
			SelloutRewardsMapper arg = _selloutRewardsMapperProvider.Get(id);
			list.Add(selector(arg));
		}
		return list;
	}

	private void OpenTutorial()
	{
		_selloutTutorialWindow = _windowsManager.Get<SelloutTutorialWindow>();
		_selloutTutorialWindow.Open();
	}

	private void OpenBank()
	{
		_sectionOpener.Open();
	}

	private void CheckReward(SelloutRewardsInfo rewardsInfo)
	{
		foreach (SelloutRewardView rewardView in _rewardViews)
		{
			rewardView.SetActiveLight(rewardView.Source.RewardInfo == rewardsInfo);
		}
	}

	private void SetActiveBankButton(bool isActive)
	{
		_bankButton.gameObject.SetActive(isActive);
	}
}
