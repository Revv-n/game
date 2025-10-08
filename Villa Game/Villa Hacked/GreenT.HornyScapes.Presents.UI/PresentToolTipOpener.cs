using System;
using System.Collections;
using GreenT.HornyScapes.ToolTips;
using GreenT.Localizations;
using StripClub.UI;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Presents.UI;

public sealed class PresentToolTipOpener : MonoView<PresentToolTip>
{
	[SerializeField]
	private Transform _container;

	[SerializeField]
	private TailedToolTipSettings _settings;

	[SerializeField]
	private string _notEnoughtKey = "ui.hint.not_enough_present";

	private LocalizationService _localizationService;

	private PresentToolTip _toolTip;

	private Coroutine _hideCoroutine;

	private readonly Subject<(PresentToolTipOpener PresentToolTipOpener, PresentView PresentView)> _requested = new Subject<(PresentToolTipOpener, PresentView)>();

	public IObservable<(PresentToolTipOpener PresentToolTipOpener, PresentView PresentView)> Requested => Observable.AsObservable<(PresentToolTipOpener, PresentView)>((IObservable<(PresentToolTipOpener, PresentView)>)_requested);

	[Inject]
	public void Init(LocalizationService localizationService)
	{
		_localizationService = localizationService;
	}

	public void Request(PresentView presentView)
	{
		_requested?.OnNext((this, presentView));
	}

	public void ShowToolTip(PresentToolTip toolTip, Transform parent)
	{
		_settings.KeyText = _localizationService.Text(_notEnoughtKey);
		_settings.Parent = parent;
		if (_toolTip != toolTip)
		{
			_toolTip = toolTip;
			_toolTip.Set(_settings);
		}
		_toolTip.transform.SetParent(parent, worldPositionStays: false);
		_toolTip.Display(display: true);
		if (_hideCoroutine != null)
		{
			StopCoroutine(_hideCoroutine);
		}
		_hideCoroutine = StartCoroutine(HideToolTipDelayed(toolTip.TooltipDuration));
	}

	public void HideToolTip()
	{
		if (_toolTip != null)
		{
			_toolTip.Display(display: false);
		}
		if (_hideCoroutine != null)
		{
			StopCoroutine(_hideCoroutine);
			_hideCoroutine = null;
		}
	}

	private void Awake()
	{
		if (_settings != null)
		{
			InitSettings(_settings);
		}
	}

	private void OnValidate()
	{
		if (_container == null)
		{
			_container = base.transform;
		}
	}

	private void InitSettings(TailedToolTipSettings settings)
	{
		_settings = UnityEngine.Object.Instantiate(settings);
		_settings.Parent = _container;
	}

	private IEnumerator HideToolTipDelayed(float delay)
	{
		yield return new WaitForSeconds(delay);
		HideToolTip();
	}
}
