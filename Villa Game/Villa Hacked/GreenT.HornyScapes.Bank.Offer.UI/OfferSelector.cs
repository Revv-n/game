using System;
using System.Linq;
using StripClub.UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.Bank.Offer.UI;

public class OfferSelector : MonoView<OfferSettings>
{
	public class Manager : MonoViewManager<OfferSettings, OfferSelector>
	{
		public override OfferSelector Display(OfferSettings source)
		{
			OfferSelector offerSelector = views.FirstOrDefault((OfferSelector _view) => _view.Source == source && _view.IsActive());
			if (offerSelector == null)
			{
				offerSelector = base.Display(source);
			}
			return offerSelector;
		}

		public override void Hide(OfferSettings source)
		{
			GetViewOrDefault(source)?.Display(display: false);
		}
	}

	private OfferMergePreviewController previewController;

	private IDisposable disposable;

	[field: SerializeField]
	public Button Button { get; private set; }

	[Inject]
	public void Init(OfferMergePreviewController previewController)
	{
		this.previewController = previewController;
	}

	private void OnEnable()
	{
		TrackOfferInFocus();
	}

	private void OnDisable()
	{
		disposable?.Dispose();
	}

	private void TrackOfferInFocus()
	{
		disposable?.Dispose();
		disposable = ObservableExtensions.Subscribe<OfferSettings>((IObservable<OfferSettings>)previewController.OfferInFocus, (Action<OfferSettings>)OnOfferChange);
	}

	private void OnOfferChange(OfferSettings offer)
	{
		bool flag = base.Source == offer;
		Button.interactable = !flag;
	}

	public void ShowOffer()
	{
		previewController.Display(base.Source);
	}

	private void OnValidate()
	{
		if (Button == null)
		{
			Button = GetComponent<Button>();
		}
	}
}
