using System.Collections.Generic;
using System.Linq;
using GreenT.Data;
using GreenT.HornyScapes.BattlePassSpace;
using GreenT.HornyScapes.BattlePassSpace.Data;
using GreenT.HornyScapes.BattlePassSpace.RewardCards;
using GreenT.HornyScapes.MergeCore;
using GreenT.HornyScapes.Saves;
using StripClub.Model;
using StripClub.Model.Data;

namespace GreenT.HornyScapes.Events;

public sealed class EventBattlePassDataBuilder
{
	private readonly BattlePassStateService _battlePassStateService;

	private readonly ISaver _saver;

	private readonly MergeFieldProvider _mergeFieldProvider;

	private readonly SimpleCurrencyFactory _simpleCurrencyFactory;

	private readonly PreloadContentService _preloadContentService;

	public EventBattlePassDataBuilder(BattlePassStateService battlePassStateService, ISaver saver, MergeFieldProvider mergeFieldProvider, SimpleCurrencyFactory simpleCurrencyFactory, PreloadContentService preloadContentService)
	{
		_battlePassStateService = battlePassStateService;
		_saver = saver;
		_mergeFieldProvider = mergeFieldProvider;
		_simpleCurrencyFactory = simpleCurrencyFactory;
		_preloadContentService = preloadContentService;
	}

	public void CreateData(BattlePass battlePass, CurrencyType currencyType, string saveKey)
	{
		BattlePassDataCreateCase battlePassDataCreateCase = new BattlePassDataCreateCase
		{
			BattlePass = battlePass
		};
		string saveKey2 = $"currency_event_battle_pass_{battlePassDataCreateCase.BattlePass.ID}";
		SimpleCurrency currency = _simpleCurrencyFactory.Create(currencyType, 0, saveKey2);
		BattlePasLevelInfoCase battlePasLevelInfoCase = new BattlePasLevelInfoCase(battlePass, currency, _saver);
		battlePassDataCreateCase.BattlePasLevelInfoCase = battlePasLevelInfoCase;
		BattlePassRewardDataLogics battlePassRewardDataLogics = new BattlePassRewardDataLogics(battlePass);
		_saver.Add(battlePassRewardDataLogics);
		battlePassDataCreateCase.RewardDataLogics = battlePassRewardDataLogics;
		string saveKey3 = $"startData_event_battle_pass_{battlePassDataCreateCase.BattlePass.ID}";
		BattlePassStartData battlePassStartData = new BattlePassStartData(_saver, saveKey3, battlePass.PremiumPurchasedLocker);
		battlePassStartData.Initialize(battlePass.PremiumPurchaseProperty);
		battlePassDataCreateCase.BattlePassStartData = battlePassStartData;
		CreateRewardPairData(battlePassDataCreateCase, battlePass);
		BattlePassMergedCurrencyDataCase battlePassMergedCurrencyDataCase = new BattlePassMergedCurrencyDataCase($"merged_currency_event_battle_pass{battlePassDataCreateCase.BattlePass.ID}");
		battlePassMergedCurrencyDataCase.Reset();
		_saver.Add(battlePassMergedCurrencyDataCase);
		battlePassDataCreateCase.MergedCurrencyDataCase = battlePassMergedCurrencyDataCase;
		battlePassDataCreateCase.BattlePass.Data.Initialization(battlePassDataCreateCase);
		battlePassDataCreateCase.BattlePass.Initialization();
		PreloadLootBoxes(battlePass);
	}

	private void PreloadLootBoxes(BattlePass battlePass)
	{
		IEnumerable<LinkedContent> linkedContents = from reward in battlePass.AllRewardContainer.GetRewardsByLinkedContent<LootboxLinkedContent>()
			select reward.Content as LootboxLinkedContent into lootbox
			select lootbox?.Lootbox.PrepareContent();
		_preloadContentService.PreloadRewards(linkedContents);
	}

	private void CreateRewardPairData(BattlePassDataCreateCase dataCreateCase, BattlePass battlePass)
	{
		int startProgressLevel = 0;
		int maxLevel = battlePass.AllRewardContainer.GetMaxLevel();
		List<BattlePassRewardPairData> list = new List<BattlePassRewardPairData>(maxLevel);
		foreach (IGrouping<int, int> item2 in from p in battlePass.AllRewardContainer.Levels
			orderby p
			select p into level
			group level by level)
		{
			foreach (int item3 in item2)
			{
				bool isLast = item3 >= maxLevel;
				BattlePassRewardPairData item = new BattlePassRewardPairData(battlePass, startProgressLevel, item3, isLast);
				list.Add(item);
				startProgressLevel = item3;
			}
		}
		dataCreateCase.RewardPairQueue = new BattlePassRewardPairQueue(list);
	}
}
