using GreenT.HornyScapes.ToolTips;
using StripClub.UI.Rewards;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace GreenT.HornyScapes.MiniEvents;

public class SummonRouletteDropViewToolTipOpener : DropViewToolTipOpener
{
	[SerializeField]
	private SummonRouletteCardDropView _sourceView;

	private int _storedId;

	private GirlPromoOpener _girlPromoOpener;

	[Inject]
	private void InnerInit(GirlPromoOpener girlPromoOpener)
	{
		_girlPromoOpener = girlPromoOpener;
	}

	protected override void OnValidate()
	{
		base.OnValidate();
		if (localizationKey == string.Empty)
		{
			localizationKey = "ui.hint.girl.";
		}
	}

	protected override void SetSettings()
	{
		if (_storedId != _sourceView.Source.ID)
		{
			base.Settings.KeyText = localizationKey + _sourceView.Source.ID;
			_storedId = _sourceView.Source.ID;
		}
	}

	public override void OnPointerClick(PointerEventData eventData)
	{
		_girlPromoOpener.TryToOpenGirlPromo(_sourceView.Source);
	}
}
