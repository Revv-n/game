using System;
using GreenT.AssetBundles;
using GreenT.Types;
using StripClub.UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.MiniEvents;

public class MiniEventActivityTabView : TabView<MiniEventActivityTab>
{
	[SerializeField]
	private GameObject _activeFrame;

	[SerializeField]
	private Image _tabBackground;

	[SerializeField]
	private Image _tabIcon;

	[SerializeField]
	private LocalizedTextMeshPro _name;

	[SerializeField]
	private LocalizedTextMeshPro _rewardName;

	[SerializeField]
	private ActivityTabPimpView _activityTabPimpView;

	[SerializeField]
	private Sprite _defaultTabBackground;

	[SerializeField]
	private StatableComponent _tabText;

	private MiniEventActivityTabsLocalizationResolver _miniEventActivityTabsLocalizationResolver;

	private MiniEventsBundlesProvider _bundlesProvider;

	private IDisposable _disposable;

	private const int TEXT_WHITE_STATE = 0;

	private const int TEXT_YELLOW_STATE = 1;

	public CustomGridLayoutAdjuster CustomGridLayoutAdjuster { get; set; }

	[Inject]
	private void Init(MiniEventActivityTabsLocalizationResolver miniEventActivityTabsLocalizationResolver, MiniEventsBundlesProvider bundlesProvider)
	{
		_miniEventActivityTabsLocalizationResolver = miniEventActivityTabsLocalizationResolver;
		_bundlesProvider = bundlesProvider;
	}

	public override void Set(MiniEventActivityTab source)
	{
		base.Set(source);
		_activityTabPimpView.StartTrack(base.Source);
		SetName();
		SetIcon();
		StartTrackContentAvailability();
	}

	public override void Display(bool display)
	{
		base.Display(display);
		if (display)
		{
			CustomGridLayoutAdjuster.OnAddElement();
		}
		else
		{
			CustomGridLayoutAdjuster.OnRemoveElement();
		}
	}

	public void SetState(CompositeIdentificator tabIdentificator, TabType tabType)
	{
		bool flag = tabIdentificator == base.Source.Identificator && base.Source.TabType == tabType;
		_activeFrame.SetActive(flag);
		if (base.Source.TabType != TabType.Shop)
		{
			_tabText.Set(flag ? 1 : 0);
		}
	}

	public void ForceSelfInteract()
	{
		OnInteractButtonClick();
	}

	protected override void OnInteractButtonClick()
	{
		_miniEventWindowView.InteractTabView(base.Source.Identificator, base.Source.TabType);
	}

	private void StartTrackContentAvailability()
	{
		_disposable?.Dispose();
		_disposable = ObservableExtensions.Subscribe<bool>(Observable.Where<bool>((IObservable<bool>)base.Source.IsAnyContentAvailable, (Func<bool, bool>)((bool value) => !value)), (Action<bool>)delegate
		{
			_miniEventWindowView.HandleEmptyActivityTab();
		});
	}

	private void SetName()
	{
		string key = _miniEventActivityTabsLocalizationResolver.GetKey(base.Source.TabType);
		key = string.Format(key, base.Source.Identificator[0]);
		_name.Init(key);
		_rewardName.Init(key);
		_rewardName.gameObject.SetActive(base.Source.TabType == TabType.Shop);
		_name.gameObject.SetActive(base.Source.TabType == TabType.Task);
	}

	private void SetIcon()
	{
		int keyById = _bundlesProvider.GetKeyById(base.Source.Identificator[0], base.Source.TabType);
		IAssetBundle assetBundle = _bundlesProvider.TryGet(keyById);
		if (assetBundle == null)
		{
			return;
		}
		Sprite sprite = assetBundle.LoadAsset<Sprite>(base.Source.IconBundleKey);
		if (!(sprite != null))
		{
			return;
		}
		_tabIcon.sprite = sprite;
		if (base.Source.TabType == TabType.Shop)
		{
			Sprite sprite2 = assetBundle.LoadAsset<Sprite>(base.Source.BackgroundBundleKey);
			if (sprite2 != null)
			{
				_tabBackground.sprite = sprite2;
			}
		}
		else
		{
			_tabBackground.sprite = _defaultTabBackground;
		}
	}
}
