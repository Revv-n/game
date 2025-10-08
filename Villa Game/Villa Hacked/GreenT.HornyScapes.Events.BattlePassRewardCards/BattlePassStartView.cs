using System;
using GreenT.HornyScapes.BattlePassSpace;
using StripClub.Extensions;
using StripClub.UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.Events.BattlePassRewardCards;

public class BattlePassStartView : MonoView<CalendarModel>
{
	[SerializeField]
	private LocalizedTextMeshPro _titleField;

	[SerializeField]
	private LocalizedTextMeshPro _titleShadow;

	[SerializeField]
	private LocalizedTextMeshPro _description;

	[SerializeField]
	private MonoTimer _offerTimer;

	[SerializeField]
	private Image _background;

	[SerializeField]
	private Image _headerImage;

	[SerializeField]
	private Image _rewardPreview;

	[SerializeField]
	private Image _leftGirlImage;

	[SerializeField]
	private Image _rightGirlImage;

	private TimeHelper _timeHelper;

	private BattlePassProvider _provider;

	private BattlePassStateService _stateService;

	private IDisposable _providerStream;

	[Inject]
	public void Constructor(TimeHelper timeHelper, BattlePassStateService stateService, BattlePassProvider provider)
	{
		_timeHelper = timeHelper;
		_stateService = stateService;
		_provider = provider;
	}

	private void Awake()
	{
		Initialize();
	}

	private void Initialize()
	{
		if (_stateService.HaveActiveBattlePass())
		{
			ApplyData(_provider.CalendarChangeProperty.Value);
		}
		_providerStream = ObservableExtensions.Subscribe<(CalendarModel, BattlePass)>(Observable.Where<(CalendarModel, BattlePass)>((IObservable<(CalendarModel, BattlePass)>)_provider.CalendarChangeProperty, (Func<(CalendarModel, BattlePass), bool>)(((CalendarModel calendar, BattlePass battlePass) tuple) => tuple.calendar != null)), (Action<(CalendarModel, BattlePass)>)ApplyData);
	}

	private void ApplyData((CalendarModel calendar, BattlePass battlePass) tuple)
	{
		Set(tuple.calendar);
		_titleField.Init(tuple.battlePass.Bundle.TitleKeyLoc);
		_titleShadow.Init(tuple.battlePass.Bundle.TitleKeyLoc);
		_description.Init(tuple.battlePass.Bundle.DescriptionKeyLoc);
		_offerTimer.Init(tuple.calendar.Duration, _timeHelper.UseCombineFormat);
		BattlePass.ViewSettings currentViewSettings = tuple.battlePass.CurrentViewSettings;
		_leftGirlImage.sprite = currentViewSettings.LeftGirl;
		_rightGirlImage.sprite = currentViewSettings.RightGirl;
		_headerImage.sprite = currentViewSettings.HeaderImage;
		_rewardPreview.sprite = currentViewSettings.RewardPreview;
		_background.sprite = currentViewSettings.StartWindowBackground;
	}

	private void OnDestroy()
	{
		_providerStream?.Dispose();
	}
}
