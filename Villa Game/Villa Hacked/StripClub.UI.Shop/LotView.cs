using System.Linq;
using StripClub.Model.Shop;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace StripClub.UI.Shop;

public class LotView : MonoView<Lot>
{
	public class LotContainerManager : ViewManager<LotContainer, ContainerView>
	{
		public void Setup(Sprite background)
		{
			foreach (ContainerView view in views)
			{
				view.LotViews.ToList()[0].background.sprite = background;
			}
		}
	}

	public class Manager : ViewManager<Lot, LotView>
	{
		public void Setup(Sprite background)
		{
			foreach (LotView view in views)
			{
				view.background.sprite = background;
			}
		}
	}

	public class Factory : ViewFactory<Lot, LotView>
	{
		public Factory(DiContainer diContainer, Transform objectContainer, LotView prefab)
			: base(diContainer, objectContainer, prefab)
		{
		}
	}

	[SerializeField]
	protected Image background;

	protected SignalBus signalBus;

	protected ICurrencyProcessor CurrencyProcessor;

	[Inject]
	private void Init(SignalBus signalBus, ICurrencyProcessor currencyProcessor)
	{
		this.signalBus = signalBus;
		CurrencyProcessor = currencyProcessor;
	}

	public override void Set(Lot lot)
	{
		background.onCullStateChanged.RemoveListener(ApplyCullState);
		base.Set(lot);
		if (!base.Source.IsViewed)
		{
			ApplyCullState(background.canvasRenderer.cull);
		}
		if (!base.Source.IsViewed)
		{
			background.onCullStateChanged.AddListener(ApplyCullState);
		}
	}

	private void OnEnable()
	{
		if (base.Source != null)
		{
			if (!base.Source.IsViewed)
			{
				ApplyCullState(background.canvasRenderer.cull);
			}
			if (!base.Source.IsViewed)
			{
				background.onCullStateChanged.AddListener(ApplyCullState);
			}
		}
	}

	private void OnDisable()
	{
		background.onCullStateChanged.RemoveListener(ApplyCullState);
	}

	private void Start()
	{
		ApplyCullState(background.canvasRenderer.cull);
	}

	private void ApplyCullState(bool isCull)
	{
		if (!isCull && !base.Source.IsViewed)
		{
			background.onCullStateChanged.RemoveListener(ApplyCullState);
			base.Source.IsViewed = true;
			ViewUpdateSignal viewUpdateSignal = new ViewUpdateSignal(this);
			signalBus.Fire<ViewUpdateSignal>(viewUpdateSignal);
		}
	}

	private void OnValidate()
	{
		if (!background)
		{
			background = GetComponentInChildren<Image>();
		}
	}

	public virtual void Purchase()
	{
		Lot source = base.Source;
		if (source.Purchase())
		{
			source.SendPurchaseNotification();
			ViewUpdateSignal viewUpdateSignal = new ViewUpdateSignal(this);
			signalBus.Fire<ViewUpdateSignal>(viewUpdateSignal);
		}
	}
}
