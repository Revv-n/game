using System;
using GreenT.HornyScapes.Presents.Models;
using StripClub.UI;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.Presents.UI;

public class PresentView : MonoView<Present>
{
	public class Manager : ViewManager<Present, PresentView>
	{
	}

	public class Factory : ViewFactory<Present, PresentView>
	{
		public Factory(DiContainer diContainer, Transform objectContainer, PresentView prefab)
			: base(diContainer, objectContainer, prefab)
		{
		}
	}

	[SerializeField]
	private Image _presentIcon;

	[SerializeField]
	private TextMeshProUGUI _pointsCount;

	[SerializeField]
	private TextMeshProUGUI _presentCount;

	[SerializeField]
	private ObservablePointerDownTrigger _pointerDownTrigger;

	[SerializeField]
	private ObservablePointerUpTrigger _pointerUpTrigger;

	[SerializeField]
	private PresentToolTipOpener _toolTipOpener;

	[SerializeField]
	private RectTransform _toolTipContainer;

	private IDisposable _countSubscription;

	private readonly int _maxPresentsPoints = 99;

	private readonly Subject<PresentView> _onSet = new Subject<PresentView>();

	public IObservable<PresentView> OnSet => _onSet.AsObservable();

	public IObservable<PointerEventData> PointerDown => _pointerDownTrigger.OnPointerDownAsObservable();

	public IObservable<PointerEventData> PointerUp => _pointerUpTrigger.OnPointerUpAsObservable();

	public IObservable<(PresentToolTipOpener PresentToolTipOpener, PresentView PresentView)> TooltipRequested => _toolTipOpener.Requested;

	public override void Set(Present source)
	{
		base.Set(source);
		SetIcon();
		_pointsCount.text = $"{base.Source.LovePoints}";
		UpdatePresentsCount(base.Source.Container.Count);
		_countSubscription?.Dispose();
		_countSubscription = base.Source.Container.ReactiveCount.Subscribe(UpdatePresentsCount);
		_onSet?.OnNext(this);
	}

	public void RequestTooltip()
	{
		_toolTipOpener.Request(this);
	}

	public RectTransform GetToolTipContainer()
	{
		return _toolTipContainer;
	}

	private void OnDestroy()
	{
		_countSubscription?.Dispose();
	}

	private void SetIcon()
	{
		_presentIcon.sprite = base.Source.Icon;
	}

	private void UpdatePresentsCount(int count)
	{
		_presentCount.text = ((_maxPresentsPoints < count) ? $"{_maxPresentsPoints}+" : $"{count}");
	}
}
