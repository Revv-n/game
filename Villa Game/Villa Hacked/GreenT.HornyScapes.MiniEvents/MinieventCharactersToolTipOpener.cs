using GreenT.HornyScapes.Characters;
using GreenT.HornyScapes.ToolTips;
using StripClub.UI.Rewards;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace GreenT.HornyScapes.MiniEvents;

public class MinieventCharactersToolTipOpener : DropViewToolTipOpener
{
	[SerializeField]
	protected MiniEventTaskCharactersRewardItemView _sourceView;

	private int _id;

	private GirlPromoOpener _girlPromoOpener;

	private CharacterManager _characterManager;

	private const string CHARACTER_LOCALIZATION_KEY = "ui.hint.girl.";

	private const string SKIN_LOCALIZATION_KEY = "content.character.skins.{0}.name";

	[Inject]
	private void InnerInit(GirlPromoOpener girlPromoOpener, CharacterManager characterManager)
	{
		_girlPromoOpener = girlPromoOpener;
		_characterManager = characterManager;
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
		string text = ((_sourceView.GirlID > 0) ? "ui.hint.girl." : "content.character.skins.{0}.name");
		int id = ((_sourceView.GirlID > 0) ? _sourceView.GirlID : _sourceView.SkinID);
		if (_sourceView.GirlID > 0)
		{
			base.Settings.KeyText = text + id.ToString().ToLower();
		}
		else
		{
			base.Settings.KeyText = string.Format(localizationKey, _sourceView.SkinID);
		}
		_id = id;
	}

	public override void OnPointerClick(PointerEventData eventData)
	{
		if (_sourceView.GirlID > 0)
		{
			SetSettings();
			_girlPromoOpener.TryToOpenGirlPromo(_characterManager.Get(_id));
		}
		else
		{
			base.OnPointerClick(eventData);
		}
	}
}
