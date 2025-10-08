using System.Collections.Generic;
using StripClub.Model;
using StripClub.Model.Shop;
using Zenject;

namespace GreenT.HornyScapes;

public sealed class RouletteBankSummonLot : RouletteSummonLot
{
	public int BankTabId { get; private set; }

	public int GoToBankTabId { get; private set; }

	public RouletteBankSummonLot(RouletteBankSummonMapper mapper, Price<int> singlePrice, Price<int> wholesalePrice, ILocker locker, IPurchaseProcessor purchaseProcessor, RouletteDropService rouletteDropService, IEnumerable<LinkedContent> mainDropSettings, IEnumerable<LinkedContent> secondaryDropSettings, SignalBus signalBus)
		: base(mapper, singlePrice, wholesalePrice, locker, purchaseProcessor, rouletteDropService, mainDropSettings, secondaryDropSettings, signalBus)
	{
		BankTabId = mapper.bank_tab_id;
		GoToBankTabId = mapper.go_to_banktab;
		_localizationBase = "content.shop.bank.roulette_summon.{0}";
	}

	protected override void SetupUniqueKey()
	{
		string text = "lot.bank.roulette.summon.";
		_uniqueKey = text + base.ID;
	}
}
