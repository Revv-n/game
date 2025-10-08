using System;
using GreenT.HornyScapes.Animations;
using Merge;
using StripClub.Model.Shop.Data;
using StripClub.UI;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes;

public class EventRatingWindowView : PopupWindow
{
	[SerializeField]
	private Image _background;

	[SerializeField]
	private Image _girl;

	[SerializeField]
	private LocalizedTextMeshPro _title;

	[SerializeField]
	private LocalizedTextMeshPro _titleShadow;

	[SerializeField]
	private LocalizedTextMeshPro _description;

	[SerializeField]
	private EventRatingView _globalRatingView;

	[SerializeField]
	private EventRatingView _groupRatingView;

	[SerializeField]
	private EventRatingView _soloGlobalRatingView;

	[SerializeField]
	private EventRatingView _soloGroupRatingView;

	[SerializeField]
	private RatingUpdater _globalRatingUpdater;

	[SerializeField]
	private RatingUpdater _soloGlobalRatingUpdater;

	[SerializeField]
	private RatingUpdater _soloGroupRatingUpdater;

	[SerializeField]
	private Button _globalRatingButton;

	[SerializeField]
	private Button _groupRatingButton;

	[SerializeField]
	private Button _exitButton;

	[SerializeField]
	private GameObject _globalRatingButtonActiveFrame;

	[SerializeField]
	private GameObject _groupRatingButtonActiveFrame;

	[SerializeField]
	private GameObject _infoButton;

	private BundlesProviderBase _bundlesProviderBase;

	private RatingDataManager _ratingDataManager;

	private Button _currentActiveButton;

	private bool _isBothRatingsActive;

	private const string BACKGROUND_BUNDLE_KEY = "_Background_Rating";

	private const string GIRL_BUNDLE_KEY = "_Girl_Rating";

	private const string TITLE_LOCALIZATION_KEY = "event_rating_title_{0}";

	private const string DESCRIPTION_LOCALIZATION_KEY = "event_rating_descr_{0}";

	private const string TITLE_IS_OVER_LOCALIZATION_KEY = "event_rating_over_title_{0}";

	public event Action<int, int> LeaderboardUpdated;

	protected override void Awake()
	{
		base.Awake();
		_globalRatingView.LeaderboardUpdated += OnLeaderboardUpdated;
		_groupRatingView.LeaderboardUpdated += OnLeaderboardUpdated;
		_soloGlobalRatingView.LeaderboardUpdated += OnLeaderboardUpdated;
		_soloGroupRatingView.LeaderboardUpdated += OnLeaderboardUpdated;
		_globalRatingButton.onClick.AddListener(OnGlobalRatingButtonClick);
		_groupRatingButton.onClick.AddListener(OnGroupRatingButtonClick);
		_exitButton.onClick.AddListener(OnExitButtonClick);
		SetActiveInfo(isActive: true);
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		_globalRatingView.LeaderboardUpdated -= OnLeaderboardUpdated;
		_groupRatingView.LeaderboardUpdated -= OnLeaderboardUpdated;
		_soloGlobalRatingView.LeaderboardUpdated -= OnLeaderboardUpdated;
		_soloGroupRatingView.LeaderboardUpdated -= OnLeaderboardUpdated;
		_globalRatingButton.onClick.RemoveListener(OnGlobalRatingButtonClick);
		_groupRatingButton.onClick.RemoveListener(OnGroupRatingButtonClick);
		_exitButton.onClick.RemoveListener(OnExitButtonClick);
	}

	[Inject]
	private void Init(RatingDataManager ratingDataManager, BundlesProviderBase bundlesProviderBase)
	{
		_ratingDataManager = ratingDataManager;
		_bundlesProviderBase = bundlesProviderBase;
	}

	public override void Open()
	{
		base.Open();
		_currentActiveButton.onClick.Invoke();
	}

	public void InitDescription(int eventId, bool isOver = false)
	{
		TrySetupLocalization(isOver ? "event_rating_over_title_{0}" : "event_rating_title_{0}", eventId, _title);
		TrySetupLocalization(isOver ? "event_rating_over_title_{0}" : "event_rating_title_{0}", eventId, _titleShadow);
		TrySetupLocalization("event_rating_descr_{0}", eventId, _description);
	}

	public void InitButtons(int globalRatingId)
	{
		_currentActiveButton = ((globalRatingId != 0) ? _globalRatingButton : _groupRatingButton);
		_currentActiveButton.onClick.Invoke();
	}

	public void TryActivateElements(int globalRatingId, int groupRatingId, bool isUpdatable = false)
	{
		_isBothRatingsActive = globalRatingId != 0 && groupRatingId != 0;
		_globalRatingButton.SetActive(_isBothRatingsActive);
		_groupRatingButton.SetActive(_isBothRatingsActive);
		_description.gameObject.SetActive(_isBothRatingsActive);
		if (isUpdatable)
		{
			_globalRatingUpdater.Enable(_isBothRatingsActive);
			_soloGlobalRatingUpdater.Enable(!_isBothRatingsActive && globalRatingId != 0);
			_soloGroupRatingUpdater.Enable(!_isBothRatingsActive && groupRatingId != 0);
		}
		else
		{
			_globalRatingUpdater.Enable(isUpdatable);
			_soloGlobalRatingUpdater.Enable(isUpdatable);
			_soloGroupRatingUpdater.Enable(isUpdatable);
		}
		if (_isBothRatingsActive)
		{
			_globalRatingView.RatingController.LeaderboardResponseUpdated -= _groupRatingView.RatingController.OnLeaderboardResponseUpdated;
			_globalRatingView.RatingController.LeaderboardResponseUpdated += _groupRatingView.RatingController.OnLeaderboardResponseUpdated;
		}
	}

	public void InitGlobal(int eventId, int calendarId, int ratingId)
	{
		_globalRatingView.Set(TryGetRatingData(eventId, calendarId, ratingId));
		_globalRatingView.InnerInit();
		_soloGlobalRatingView.Set(TryGetRatingData(eventId, calendarId, ratingId));
		_soloGlobalRatingView.InnerInit();
	}

	public void InitGroup(int eventId, int calendarId, int ratingId)
	{
		_groupRatingView.Set(TryGetRatingData(eventId, calendarId, ratingId));
		_groupRatingView.InnerInit();
		_soloGroupRatingView.Set(TryGetRatingData(eventId, calendarId, ratingId));
		_soloGroupRatingView.InnerInit();
	}

	public void InitBackground(string bundleType)
	{
		TrySetupSprite(bundleType, "_Background_Rating", _background);
		TrySetupSprite(bundleType, "_Girl_Rating", _girl);
	}

	public void ForceAutoUpdate(int globalRatingId, int groupRatingId)
	{
		if (globalRatingId != 0 && groupRatingId != 0)
		{
			_globalRatingView.AutoUpdate();
			_groupRatingView.AutoUpdate();
		}
		if (globalRatingId != 0 && groupRatingId == 0)
		{
			_soloGlobalRatingView.AutoUpdate();
		}
		if (groupRatingId != 0 && globalRatingId == 0)
		{
			_soloGroupRatingView.AutoUpdate();
		}
	}

	public void SetActiveInfo(bool isActive)
	{
		_infoButton.SetActive(isActive);
	}

	private void TrySetupLocalization(string origin, object argument, LocalizedTextMeshPro destination)
	{
		string key = string.Format(origin, argument);
		destination.Init(key);
	}

	private void TrySetupSprite(string bundleType, string key, Image destination)
	{
		string bundleName = bundleType + key;
		Sprite sprite = _bundlesProviderBase.TryFindInConcreteBundle<Sprite>(ContentSource.EventBundle, bundleName);
		destination.sprite = sprite;
	}

	private RatingData TryGetRatingData(int eventId, int calendarId, int ratingId)
	{
		return _ratingDataManager.TryGetRatingData(eventId, calendarId, ratingId);
	}

	private void OnLeaderboardUpdated(int globalPlace, int groupPlace)
	{
		this.LeaderboardUpdated?.Invoke(globalPlace, groupPlace);
	}

	private void OnGlobalRatingButtonClick()
	{
		if (_isBothRatingsActive)
		{
			_soloGroupRatingView.Display(display: false);
			_soloGlobalRatingView.Display(display: false);
			_groupRatingView.Display(display: false);
			_globalRatingView.Display(display: true);
			_groupRatingButtonActiveFrame.SetActive(value: false);
			_globalRatingButtonActiveFrame.SetActive(value: true);
		}
		else
		{
			_groupRatingView.Display(display: false);
			_globalRatingView.Display(display: false);
			_soloGroupRatingView.Display(display: false);
			_soloGlobalRatingView.Display(display: true);
		}
		_currentActiveButton = _globalRatingButton;
	}

	private void OnGroupRatingButtonClick()
	{
		if (_isBothRatingsActive)
		{
			_soloGroupRatingView.Display(display: false);
			_soloGlobalRatingView.Display(display: false);
			_globalRatingView.Display(display: false);
			_groupRatingView.Display(display: true);
			_globalRatingButtonActiveFrame.SetActive(value: false);
			_groupRatingButtonActiveFrame.SetActive(value: true);
		}
		else
		{
			_groupRatingView.Display(display: false);
			_globalRatingView.Display(display: false);
			_soloGlobalRatingView.Display(display: false);
			_soloGroupRatingView.Display(display: true);
		}
		_currentActiveButton = _groupRatingButton;
	}

	private void OnExitButtonClick()
	{
		Close();
	}
}
