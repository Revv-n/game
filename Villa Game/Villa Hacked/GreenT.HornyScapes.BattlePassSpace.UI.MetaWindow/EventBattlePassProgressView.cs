using System;
using System.Collections.Generic;
using GreenT.HornyScapes.BattlePassSpace.RewardCards;
using GreenT.HornyScapes.Characters;
using GreenT.HornyScapes.Events;
using GreenT.HornyScapes.Events.BattlePassRewardCards;
using GreenT.HornyScapes.Lootboxes;
using GreenT.Localizations;
using Merge.Meta.RoomObjects;
using StripClub.Extensions;
using StripClub.Model.Character;
using StripClub.UI;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.BattlePassSpace.UI.MetaWindow;

public class EventBattlePassProgressView : MonoView, IInitializable, IDisposable
{
	private const int FreeState = 0;

	private const int PremiumPurchasedState = 1;

	private const int BattlePassCompleteState = 2;

	private const int BattlePassLastChanceFreeState = 3;

	private const int BattlePassLastChancePremiumState = 4;

	[SerializeField]
	private MonoTimer _offerTimer;

	[SerializeField]
	private SliderUpBattlePassProgress _sliderUpBattlePassProgress;

	[SerializeField]
	private ScrollContentScroller _contentScroller;

	[SerializeField]
	private CurrencySpriteAttacher _currencySpriteAttacher;

	[SerializeField]
	private StatableComponentGroup _holderGroup;

	[SerializeField]
	private Image _background;

	[SerializeField]
	private Image _girlImage;

	[SerializeField]
	private Transform _rewardLevelViewContrainer;

	[SerializeField]
	private Button _closeButton;

	[SerializeField]
	private Button _claimAllButton;

	[SerializeField]
	private TMP_Text _descriptionTopField;

	[SerializeField]
	private TMP_Text _descriptionTopShadow;

	[SerializeField]
	private TMP_Text _descriptionBottomField;

	[SerializeField]
	private TMP_Text _descriptionBottomShadow;

	[SerializeField]
	private string _eventDescriptionKey = "ui.battlepass.event.descr";

	[SerializeField]
	private string _lastChanceDescriptionKey = "ui.battlepass.lastchance.descr";

	[SerializeField]
	private string _lastChanceTimerKey = "ui.battlepass.lastchance.ending";

	private TimeHelper _timeHelper;

	private BattlePassRewardLevelManager _battlePassRewardLevelManager;

	private BattlePassMapperProvider _mapperProvider;

	private LastChanceManager _lastChanceManager;

	private LocalizationService _localizationService;

	private EventSettingsProvider _eventSettingsProvider;

	private CharacterProvider _characterProvider;

	private CalendarModel _eventCalendarModel;

	private BattlePass _battlePass;

	private GenericTimer _lastChanceTimer;

	private IDisposable _premiumStream;

	private IDisposable _calendarStateStream;

	private IDisposable _rewardUpdateStream;

	private IDisposable _claimButtonSubscribe;

	private bool _isLastChance;

	private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

	[Inject]
	public void Construct(TimeHelper timeHelper, BattlePassRewardLevelManager battlePassRewardLevelManager, BattlePassMapperProvider mapperProvider, LastChanceManager lastChanceManager, LocalizationService localizationService, EventSettingsProvider eventSettingsProvider, CharacterProvider characterProvider)
	{
		_timeHelper = timeHelper;
		_battlePassRewardLevelManager = battlePassRewardLevelManager;
		_mapperProvider = mapperProvider;
		_lastChanceManager = lastChanceManager;
		_localizationService = localizationService;
		_eventSettingsProvider = eventSettingsProvider;
		_characterProvider = characterProvider;
		_lastChanceTimer = new GenericTimer(TimeSpan.Zero);
	}

	public void Set(CalendarModel eventCalendarModel, BattlePass battlePass)
	{
		_eventCalendarModel = eventCalendarModel;
		_battlePass = battlePass;
		_rewardUpdateStream = ObservableExtensions.Subscribe<RewardWithManyConditions>(battlePass.OnRewardUpdate, (Action<RewardWithManyConditions>)delegate
		{
			CheckCompleteRewards();
		});
		_claimButtonSubscribe = ObservableExtensions.Subscribe<Unit>(UnityUIComponentExtensions.OnClickAsObservable(_claimAllButton), (Action<Unit>)delegate
		{
			battlePass.TryCollectAllRewards();
		});
		_premiumStream = ObservableExtensions.Subscribe<bool>((IObservable<bool>)battlePass.Data.StartData.PremiumPurchasedProperty, (Action<bool>)delegate(bool status)
		{
			int stateNumber = (status ? 1 : 0);
			_holderGroup.Set(stateNumber);
		});
		_calendarStateStream = ObservableExtensions.Subscribe<EntityStatus>(Observable.Where<EntityStatus>((IObservable<EntityStatus>)_eventCalendarModel.CalendarState, (Func<EntityStatus, bool>)((EntityStatus status) => status == EntityStatus.Complete)), (Action<EntityStatus>)delegate
		{
			_holderGroup.Set(2);
		});
	}

	public void Initialize()
	{
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Unit>(UnityUIComponentExtensions.OnClickAsObservable(_closeButton), (Action<Unit>)delegate
		{
			AtClose();
		}), (ICollection<IDisposable>)_compositeDisposable);
	}

	public void Dispose()
	{
		_compositeDisposable.Dispose();
		_premiumStream?.Dispose();
		_calendarStateStream?.Dispose();
		_rewardUpdateStream?.Dispose();
		_claimButtonSubscribe?.Dispose();
	}

	private void TryRestoreState(BattlePass battlePass)
	{
		_holderGroup.Set(battlePass.Data.StartData.PremiumPurchasedProperty.Value ? 1 : 0);
	}

	private void OnEnable()
	{
		if (_battlePass != null)
		{
			if (!_battlePass.Data.StartData.FirstStartedProgress.Value)
			{
				_battlePass.Data.StartData.SetIsNotFirstStartedProgress();
			}
			string text = _mapperProvider.GetEventMapper(_battlePass.ID).bp_resource;
			if (string.IsNullOrEmpty(text))
			{
				text = "bp_points";
			}
			SelectorTools.GetResourceEnumValueByConfigKey(text, out var currency);
			_currencySpriteAttacher.ChangeCurrency(currency);
		}
		CheckLastChance();
		CheckCompleteRewards();
	}

	private void ApplyData(ViewSettings? eventViewSettings, BattlePass battlePass)
	{
		ShowRewardCards(battlePass);
		_sliderUpBattlePassProgress.Reset();
		_sliderUpBattlePassProgress.Set(battlePass);
		int battlePassId = _battlePass.ID;
		int bp_preview_girl_id = _mapperProvider.GetEventMapper(battlePassId).bp_preview_girl_id;
		if (bp_preview_girl_id != 0)
		{
			DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<CharacterStories>(Observable.Do<CharacterStories>(_characterProvider.GetStory(bp_preview_girl_id), (Action<CharacterStories>)delegate(CharacterStories characterStories)
			{
				SetGirlImage(characterStories, battlePassId);
			})), (Component)this);
		}
		if (eventViewSettings.HasValue && eventViewSettings.Value.BattlePassBackground != null)
		{
			_background.sprite = eventViewSettings.Value.BattlePassBackground;
		}
	}

	private void SetDescription(string localizationKey)
	{
		string[] array = _localizationService.Text(localizationKey).Split('\n', StringSplitOptions.None);
		if (array.Length == 2)
		{
			string text = array[0];
			string text2 = array[1];
			_descriptionTopField.text = text;
			_descriptionTopShadow.text = text;
			_descriptionBottomField.text = text2;
			_descriptionBottomShadow.text = text2;
		}
		else if (array.Length == 1)
		{
			string text3 = array[0];
			_descriptionTopField.text = text3;
			_descriptionTopShadow.text = text3;
			_descriptionBottomField.text = string.Empty;
			_descriptionBottomShadow.text = string.Empty;
		}
	}

	private void SetGirlImage(CharacterStories characterStories, int battlePassId)
	{
		if (!(characterStories == null))
		{
			_girlImage.sprite = characterStories.StorySprite;
		}
	}

	private IObservable<ICharacter> TryLoadCharacter(int characterId, int battlePassId)
	{
		try
		{
			return _characterProvider.Get(characterId);
		}
		catch (Exception innerException)
		{
			throw innerException.SendException($"Can't load character {characterId} for battle pass {battlePassId}");
		}
	}

	private void ShowRewardCards(BattlePass battlePass)
	{
		_battlePassRewardLevelManager.HideAll();
		int targetIndex = -1;
		for (int i = 0; i < battlePass.Data.RewardPairQueue.Cases.Count; i++)
		{
			BattlePassRewardPairData battlePassRewardPairData = battlePass.Data.RewardPairQueue.Cases[i];
			BattlePassRewardLevelView battlePassRewardLevelView = _battlePassRewardLevelManager.Display(battlePassRewardPairData);
			battlePassRewardLevelView.UpdateRewardHolder(battlePass);
			battlePassRewardLevelView.transform.SetParent(_rewardLevelViewContrainer);
			battlePassRewardLevelView.transform.SetSiblingIndex(i);
			if (targetIndex == -1 && battlePassRewardPairData.HasAnyRewardComplete())
			{
				targetIndex = i;
			}
		}
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Unit>(Observable.ObserveOnMainThread<Unit>(Observable.NextFrame((FrameCountType)0)), (Action<Unit>)delegate
		{
			_contentScroller.ScrollToIndex(targetIndex);
		}), (Component)this);
	}

	private void AtClose()
	{
		_battlePassRewardLevelManager.HideAll();
	}

	private void CheckLastChance()
	{
		TryRestoreState(_battlePass);
		foreach (LastChance item in _lastChanceManager.Collection)
		{
			if (item.LastChanceType == LastChanceType.EventBP)
			{
				int stateNumber = (_battlePass.Data.StartData.PremiumPurchasedProperty.Value ? 4 : 3);
				_holderGroup.Set(stateNumber);
				TimeSpan timeLeft = TimeSpan.FromSeconds(item.EndDate - GetUTCNowSeconds());
				_lastChanceTimer.Start(timeLeft);
				_isLastChance = true;
				break;
			}
		}
		if (!_isLastChance)
		{
			SetDescription(_eventDescriptionKey);
			int event_id = (_eventCalendarModel.EventMapper as EventMapper).event_id;
			Event @event = _eventSettingsProvider.GetEvent(event_id);
			ApplyData(@event.ViewSettings, _battlePass);
		}
		else
		{
			SetDescription(_lastChanceDescriptionKey);
			ApplyData(null, _battlePass);
		}
		GenericTimer timer = (_isLastChance ? _lastChanceTimer : _eventCalendarModel.Duration);
		_offerTimer.Init(timer, _timeHelper.UseCombineFormat);
	}

	private void CheckCompleteRewards()
	{
		_battlePass.HasUncollectedRewards();
		_claimAllButton.gameObject.SetActive(value: false);
	}

	private long GetUTCNowSeconds()
	{
		return DateTimeOffset.UtcNow.ToUnixTimeSeconds();
	}
}
