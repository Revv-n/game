using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using GreenT.AssetBundles;
using GreenT.HornyScapes.Events;
using GreenT.Types;
using StripClub.Extensions;
using StripClub.UI;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.MiniEvents;

public class MiniEventView : TabView<MiniEvent>
{
	[SerializeField]
	private LocalizedTextMeshPro _title;

	[SerializeField]
	private TMP_Text _animatedText;

	[SerializeField]
	private MonoTimer _eventTimer;

	[SerializeField]
	private GameObject _eventTimerRoot;

	[SerializeField]
	private GameObject _arrow;

	[SerializeField]
	private Image _background;

	[SerializeField]
	private MiniEventTabPimpView _miniEventTabPimpView;

	[Header("SELECT ANIMATION SETTINGS")]
	[SerializeField]
	private LayoutElement _scaleRoot;

	[SerializeField]
	private Color _selectedColor;

	[SerializeField]
	private Vector2 _selectedSize;

	[SerializeField]
	private Vector2 _defaultSize;

	[SerializeField]
	private float _changeSizeTime;

	private TimeHelper _timeHelper;

	private MiniEventsBundlesProvider _bundlesProvider;

	private MiniEventTimerController _miniEventTimerController;

	private CompositeDisposable _compositeDisposable;

	private readonly string MINI_TITLE_KEY = "ui.minievent.event_minititle{0}.name";

	public CompositeIdentificator Identificator => base.Source.Identificator;

	[Inject]
	private void Init(TimeHelper timeHelper, MiniEventsBundlesProvider bundlesProvider, MiniEventTimerController miniEventTimerController)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Expected O, but got Unknown
		_timeHelper = timeHelper;
		_bundlesProvider = bundlesProvider;
		_miniEventTimerController = miniEventTimerController;
		_compositeDisposable = new CompositeDisposable();
	}

	private void OnDestroy()
	{
		CompositeDisposable compositeDisposable = _compositeDisposable;
		if (compositeDisposable != null)
		{
			compositeDisposable.Clear();
		}
		CompositeDisposable compositeDisposable2 = _compositeDisposable;
		if (compositeDisposable2 != null)
		{
			compositeDisposable2.Dispose();
		}
	}

	public override void Set(MiniEvent source)
	{
		base.Set(source);
		_miniEventTabPimpView.StartTrack(base.Source);
		SetName();
		SetBackground();
		GenericTimer duration = _miniEventTimerController.TryGetTimerByID(base.Source.EventId, base.Source.CalendarId);
		ApplyTimer(duration);
		base.Source.IsSpawned.Value = true;
	}

	public void SetState(CompositeIdentificator evnetIdentificator)
	{
		bool isSelected = base.Source.Identificator == evnetIdentificator;
		DOTweenModuleUI.DOColor(_animatedText, isSelected ? _selectedColor : Color.white, _changeSizeTime);
		DOTweenModuleUI.DOPreferredSize(_scaleRoot, isSelected ? _selectedSize : _defaultSize, _changeSizeTime).OnComplete(delegate
		{
			_arrow.SetActive(isSelected);
		});
	}

	public void ForceSelfInteract()
	{
		OnInteractButtonClick();
	}

	protected override void OnInteractButtonClick()
	{
		_miniEventWindowView.InteractMiniEventView(base.Source.Identificator, base.Source.EventId, base.Source.IsMultiTabbed, base.Source.CurrencyIdentificator, base.Source.ConfigType);
		base.Source.WasFirstTimeSeen.Value = true;
	}

	private void SetName()
	{
		string key = string.Format(MINI_TITLE_KEY, base.Source.EventId);
		_title.Init(key);
	}

	private void SetBackground()
	{
		Sprite sprite = null;
		if (base.Source.ViewSetting.HasValue)
		{
			sprite = base.Source.ViewSetting.Value.Tab;
		}
		else
		{
			IAssetBundle assetBundle = _bundlesProvider.TryGet(base.Source.EventId);
			MiniEventBundleData miniEventBundleData = assetBundle.LoadAllAssets<MiniEventBundleData>().FirstOrDefault();
			if (assetBundle != null)
			{
				sprite = miniEventBundleData.TabBackground;
			}
		}
		_background.sprite = sprite;
	}

	private void ApplyTimer(GenericTimer duration)
	{
		bool flag = duration.TimeLeft > TimeSpan.Zero;
		_eventTimerRoot.SetActive(flag);
		_eventTimer.Init(duration, _timeHelper.UseCombineFormat);
		CompositeDisposable compositeDisposable = _compositeDisposable;
		if (compositeDisposable != null)
		{
			compositeDisposable.Clear();
		}
		if (flag)
		{
			DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<GenericTimer>(duration.OnTimeIsUp, (Action<GenericTimer>)delegate
			{
				_eventTimerRoot.SetActive(value: false);
			}), (ICollection<IDisposable>)_compositeDisposable);
		}
	}
}
