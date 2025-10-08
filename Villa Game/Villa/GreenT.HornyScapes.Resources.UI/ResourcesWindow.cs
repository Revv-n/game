using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.BattlePassSpace.UI.MetaWindow;
using GreenT.HornyScapes.Events.Content;
using GreenT.HornyScapes.MergeCore;
using GreenT.HornyScapes.UI;
using GreenT.Model.Reactive;
using GreenT.Types;
using GreenT.UI;
using StripClub.Model;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Resources.UI;

public class ResourcesWindow : Window
{
	[SerializeField]
	private List<CurrencyView> currencyViews;

	[SerializeField]
	private CollectFlyTargetsContainer _targetsContainer;

	[Inject]
	private ICameraChanger cameraChanger;

	private GreenT.Model.Reactive.ReactiveCollection<CurrencyType> visibleCurrencyTypes;

	private readonly CompositeDisposable currencyStream = new CompositeDisposable();

	private IWindowsManager _windowsManager;

	private BattlePassMetaWindow _battlePassMetaWindow;

	private ContentSelectorGroup _contentSelectorGroup;

	[Inject]
	public void Init(GreenT.Model.Reactive.ReactiveCollection<CurrencyType> visibleCurrencyTypes, IWindowsManager windowsManager, ContentSelectorGroup contentSelectorGroup)
	{
		_contentSelectorGroup = contentSelectorGroup;
		_windowsManager = windowsManager;
		this.visibleCurrencyTypes = visibleCurrencyTypes;
	}

	protected override void Awake()
	{
		base.Awake();
		visibleCurrencyTypes.SetItems(CurrencyType.Soft, CurrencyType.Hard, CurrencyType.Energy, CurrencyType.Star, CurrencyType.Jewel);
		_battlePassMetaWindow = _windowsManager.Get<BattlePassMetaWindow>();
		TrackCurrencyChanges(visibleCurrencyTypes);
	}

	public void SetTransferButton(bool interactable)
	{
		foreach (CurrencyView item in currencyViews.Where((CurrencyView _view) => visibleCurrencyTypes.Contains(_view.Currency)))
		{
			item.SetTransferButtonAvailable(interactable);
		}
	}

	private void OnEnable()
	{
		if (visibleCurrencyTypes != null)
		{
			TrackCurrencyChanges(visibleCurrencyTypes);
		}
	}

	private void OnDisable()
	{
		currencyStream.Clear();
	}

	private void TrackCurrencyChanges(GreenT.Model.Reactive.ReactiveCollection<CurrencyType> visibleCurrencyTypes)
	{
		currencyStream.Clear();
		(from _ in visibleCurrencyTypes.ObserveAdd()
			where IsOpened
			select _ into _event
			select _event.Value).Subscribe(ShowCurrency).AddTo(currencyStream);
		(from _event in visibleCurrencyTypes.ObserveRemove()
			select _event.Value).Subscribe(HideCurrency).AddTo(currencyStream);
		(from _ in visibleCurrencyTypes.ObserveSet()
			where IsOpened
			select _ into _event
			select _event.Items).Subscribe(SetVisibleCurrencies).AddTo(currencyStream);
	}

	private void SetVisibleCurrencies(IEnumerable<CurrencyType> obj)
	{
		foreach (CurrencyView currencyView in currencyViews)
		{
			if (obj.Contains(currencyView.Currency))
			{
				if (!currencyView.IsActive())
				{
					currencyView.Display(display: true);
				}
			}
			else
			{
				currencyView.Display(display: false);
			}
		}
	}

	private void ShowCurrency(CurrencyType currency)
	{
		foreach (CurrencyView item in currencyViews.Where((CurrencyView _view) => _view.Currency == currency))
		{
			if (!item.IsActive())
			{
				item.Display(display: true);
			}
		}
	}

	private void HideCurrency(CurrencyType currency)
	{
		foreach (CurrencyView item in currencyViews.Where((CurrencyView _view) => _view.Currency == currency))
		{
			if (item.IsActive())
			{
				item.Display(display: false);
			}
		}
	}

	private CurrencyView GetCurrencyView(CurrencyType type)
	{
		return currencyViews.FirstOrDefault((CurrencyView _view) => _view.Currency == type);
	}

	public Transform GetCurrencyTransform(CurrencyType type, bool rect = true)
	{
		if (!rect)
		{
			if (type != CurrencyType.MiniEvent)
			{
				return _targetsContainer.GetPosition(type);
			}
			if (_contentSelectorGroup.Current != ContentType.Event)
			{
				return _targetsContainer.GetPosition(type);
			}
			return null;
		}
		if (type == CurrencyType.BP || type == CurrencyType.EventXP)
		{
			return _battlePassMetaWindow.GetIconTransform();
		}
		return GetCurrencyView(type)?.Icon.transform ?? _targetsContainer.GetPosition(type);
	}

	public Vector3 GetMergeCurrencyPosition(CurrencyType type)
	{
		Transform currencyTransform = GetCurrencyTransform(type);
		return cameraChanger.MergeCamera.ScreenToWorldPoint(currencyTransform.position);
	}

	public override void Open()
	{
		base.Open();
		foreach (CurrencyView item in currencyViews.Where((CurrencyView _view) => visibleCurrencyTypes.Contains(_view.Currency)))
		{
			item.Display(display: true);
		}
	}

	public override void Close()
	{
		base.Close();
		foreach (CurrencyView currencyView in currencyViews)
		{
			currencyView.Display(display: false);
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		currencyStream.Dispose();
	}
}
