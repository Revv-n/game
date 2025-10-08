using System.Collections.Generic;
using GreenT.Data;
using GreenT.HornyScapes.MiniEvents;
using StripClub.Model;
using StripClub.Model.Shop;
using StripClub.Model.Shop.Data;
using Zenject;

namespace GreenT.HornyScapes;

public class RouletteSummonLot : RouletteLot, ISavableState
{
	protected string _uniqueKey;

	protected string _localizationBase = "content.shop.roulette_summon.{0}";

	protected const string TITLE_KEY = ".title";

	protected const string DESCRIPTION_KEY = ".desc";

	protected const string GUARANTEED_DESCRIPTION_KEY = ".guaranteed.desc";

	public string ViewType { get; }

	public bool CurrentWholesale { get; protected set; }

	public ContentSource Source { get; }

	public IEnumerable<LinkedContent> MainDropSettings { get; private set; }

	public IEnumerable<LinkedContent> SecondaryDropSettings { get; private set; }

	public Price<int> SinglePrice { get; private set; }

	public Price<int> WholesalePrice { get; private set; }

	public string LocalizationKey => string.Format(_localizationBase, base.ID);

	public string TitleKey => LocalizationKey + ".title";

	public string DescriptionKey => LocalizationKey + ".desc";

	public string GuaranteedDescriptionKey => LocalizationKey + ".guaranteed.desc";

	public RouletteSummonLot(RouletteSummonMapper mapper, Price<int> singlePrice, Price<int> wholesalePrice, ILocker locker, IPurchaseProcessor purchaseProcessor, RouletteDropService rouletteDropService, IEnumerable<LinkedContent> mainDropSettings, IEnumerable<LinkedContent> secondaryDropSettings, SignalBus signalBus)
		: base(mapper.id, locker, purchaseProcessor, mapper.source, rouletteDropService, signalBus)
	{
		Source = mapper.content_source;
		ViewType = mapper.view_type;
		MainDropSettings = mainDropSettings;
		SecondaryDropSettings = secondaryDropSettings;
		SinglePrice = singlePrice;
		WholesalePrice = wholesalePrice;
		SetupUniqueKey();
	}

	public override string UniqueKey()
	{
		return _uniqueKey;
	}

	public void Setup(bool wholesale)
	{
		base.CurrentPrice = (wholesale ? WholesalePrice : SinglePrice);
		base.Wholesale = wholesale;
	}

	protected virtual void SetupUniqueKey()
	{
		string text = "lot.roulette.summon.";
		_uniqueKey = text + base.ID;
	}
}
