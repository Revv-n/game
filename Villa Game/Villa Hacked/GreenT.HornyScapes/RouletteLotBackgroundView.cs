using StripClub.Model.Shop;
using StripClub.UI;
using StripClub.UI.Shop;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes;

public class RouletteLotBackgroundView : MonoView<RouletteLot>
{
	public class RouletteLotContainerManager : ViewManager<RouletteLot, RouletteLotBackgroundView>
	{
	}

	[SerializeField]
	protected Image _background;

	protected SignalBus _signalBus;

	protected ICurrencyProcessor _currencyProcessor;

	[Inject]
	private void Init(SignalBus signalBus, ICurrencyProcessor currencyProcessor)
	{
		_signalBus = signalBus;
		_currencyProcessor = currencyProcessor;
	}

	private void OnValidate()
	{
		if (!_background)
		{
			_background = GetComponentInChildren<Image>();
		}
	}

	private void OnEnable()
	{
		if (base.Source != null)
		{
			if (!base.Source.IsViewed)
			{
				ApplyCullState(_background.canvasRenderer.cull);
			}
			if (!base.Source.IsViewed)
			{
				_background.onCullStateChanged.AddListener(ApplyCullState);
			}
		}
	}

	private void OnDisable()
	{
		_background.onCullStateChanged.RemoveListener(ApplyCullState);
	}

	private void Start()
	{
		ApplyCullState(_background.canvasRenderer.cull);
	}

	public override void Set(RouletteLot lot)
	{
		_background.onCullStateChanged.RemoveListener(ApplyCullState);
		base.Set(lot);
		if (!base.Source.IsViewed)
		{
			ApplyCullState(_background.canvasRenderer.cull);
		}
		if (!base.Source.IsViewed)
		{
			_background.onCullStateChanged.AddListener(ApplyCullState);
		}
	}

	private void ApplyCullState(bool isCull)
	{
		if (!isCull && !base.Source.IsViewed)
		{
			_background.onCullStateChanged.RemoveListener(ApplyCullState);
			base.Source.IsViewed = true;
			ViewUpdateSignal viewUpdateSignal = new ViewUpdateSignal(this);
			_signalBus.Fire<ViewUpdateSignal>(viewUpdateSignal);
		}
	}

	public virtual void Purchase()
	{
		RouletteLot source = base.Source;
		if (source.Purchase())
		{
			source.SendPurchaseNotification(base.Source.Wholesale);
			ViewUpdateSignal viewUpdateSignal = new ViewUpdateSignal(this);
			_signalBus.Fire<ViewUpdateSignal>(viewUpdateSignal);
		}
	}
}
