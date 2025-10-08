using System;
using System.Collections.Generic;
using GreenT.HornyScapes.Presents.Models;
using GreenT.HornyScapes.Presents.Services;
using StripClub.UI;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Presents.UI;

public class PresentsWindowSetter : MonoView
{
	[SerializeField]
	private PresentToolTip _toolTip;

	private IViewManager<Present, PresentView> _presentsViewManager;

	private PresentsViewTapTracker _presentsViewTapTracker;

	private readonly List<PresentView> _presentViews = new List<PresentView>(4);

	private readonly Subject<IReadOnlyList<PresentView>> _presentViewsShowed = new Subject<IReadOnlyList<PresentView>>();

	public IObservable<IReadOnlyList<PresentView>> PresentViewsShowed => _presentViewsShowed.AsObservable();

	[Inject]
	public void Init(IViewManager<Present, PresentView> presentsViewManager, PresentsViewTapTracker presentsViewTapTracker)
	{
		_presentsViewManager = presentsViewManager;
		_presentsViewTapTracker = presentsViewTapTracker;
	}

	public void Set(IEnumerable<Present> source)
	{
		_presentViews.Clear();
		DisplayPresents(source);
	}

	private void OnEnable()
	{
		_presentViewsShowed?.OnNext(_presentViews);
		_presentsViewTapTracker.Track(_presentsViewManager.VisibleViews);
	}

	private void OnDisable()
	{
		_presentsViewTapTracker.Untrack();
	}

	private void DisplayPresents(IEnumerable<Present> source)
	{
		_presentsViewManager.HideAll();
		foreach (Present item in source)
		{
			PresentView presentView = _presentsViewManager.Display(item);
			presentView.TooltipRequested.Subscribe(ShowTooltip).AddTo(this);
			_presentViews.Add(presentView);
		}
	}

	private void ShowTooltip((PresentToolTipOpener PresentToolTipOpener, PresentView PresentView) info)
	{
		info.PresentToolTipOpener.ShowToolTip(parent: info.PresentView.GetToolTipContainer(), toolTip: _toolTip);
	}
}
