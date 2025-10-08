using System;
using GreenT.HornyScapes.Constants;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.Bank.Offer.UI;

public class OfferMergePreviewController : OfferPreviewControllerBase
{
	private IDisposable observableSwipe;

	private OfferPreview preview;

	private TimeSpan swipeTime;

	private ReactiveProperty<OfferSettings> offerInFocus = new ReactiveProperty<OfferSettings>((OfferSettings)null);

	public IReadOnlyReactiveProperty<OfferSettings> OfferInFocus => (IReadOnlyReactiveProperty<OfferSettings>)(object)ReactivePropertyExtensions.ToReadOnlyReactiveProperty<OfferSettings>((IObservable<OfferSettings>)offerInFocus);

	[Inject]
	public void Init(OfferPreview preview, IConstants<int> constants)
	{
		this.preview = preview;
		swipeTime = TimeSpan.FromSeconds(constants["merge_offer_swipe"]);
	}

	protected virtual void OnEnable()
	{
		Display(offerInFocus.Value);
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		observableSwipe?.Dispose();
	}

	public void SwipeTimer(bool isOn)
	{
		observableSwipe?.Dispose();
		if (isOn && preview.IsActive())
		{
			observableSwipe = ObservableExtensions.Subscribe<long>(Observable.Timer(swipeTime, Scheduler.MainThreadIgnoreTimeScale), (Action<long>)delegate
			{
				DisplayNextOffer(preview.Source.SortingNumber);
			});
		}
	}

	public override void Display(OfferSettings offer)
	{
		if (offer != null)
		{
			preview.Set(offer);
			if (!preview.IsActive())
			{
				preview.Display(display: true);
			}
			offerInFocus.Value = offer;
			SwipeTimer(isOn: true);
		}
		else
		{
			if (preview.IsActive())
			{
				preview.Display(display: false);
			}
			offerInFocus.Value = null;
			SwipeTimer(isOn: false);
		}
	}

	protected override void OnOfferUnlocked(OfferSettings offer)
	{
		base.OnOfferUnlocked(offer);
		SwipeTimer(isOn: true);
	}

	protected override void Replace(OfferSettings offer, OfferSettings next)
	{
		base.Replace(offer, next);
		Display(next);
	}

	public virtual void SwitchOffer(bool forward)
	{
		OfferSettings offer = SelectNextOffer(preview.Source.SortingNumber, forward);
		Display(offer);
	}

	protected override void Hide(OfferSettings current)
	{
		if (preview.Source.ID == current.ID)
		{
			base.Hide(current);
			DisplayNextOffer(current.SortingNumber);
		}
	}

	protected virtual void OnDisable()
	{
		SwipeTimer(isOn: false);
	}
}
