using System.Collections.Generic;
using System.Linq;
using GreenT.Data;
using GreenT.HornyScapes.BattlePassSpace.Data;
using GreenT.HornyScapes.BattlePassSpace.RewardCards;
using GreenT.HornyScapes.Events;
using GreenT.HornyScapes.MergeCore;
using GreenT.HornyScapes.Saves;
using StripClub.Model;
using StripClub.Model.Data;
using StripClub.NewEvent.Save;

namespace GreenT.HornyScapes.BattlePassSpace;

public class BattlePassDataBuilder : ICalendarDataBuilder
{
	private readonly ISaver saver;

	private readonly MergeFieldProvider mergeFieldProvider;

	private readonly BattlePassStateService _battlePassStateService;

	private readonly SimpleCurrencyFactory simpleCurrencyFactory;

	private readonly BattlePassSettingsProvider _battlePassSettingsProvider;

	private readonly PreloadContentService _preloadContentService;

	public BattlePassDataBuilder(SimpleCurrencyFactory simpleCurrencyFactory, ISaver saver, MergeFieldProvider mergeFieldProvider, BattlePassStateService battlePassStateService, PreloadContentService preloadContentService, BattlePassSettingsProvider battlePassSettingsProvider)
	{
		this.saver = saver;
		this.mergeFieldProvider = mergeFieldProvider;
		this.simpleCurrencyFactory = simpleCurrencyFactory;
		_battlePassStateService = battlePassStateService;
		_preloadContentService = preloadContentService;
		_battlePassSettingsProvider = battlePassSettingsProvider;
	}

	public void CreateData(CalendarModel calendarModel, string saveKey)
	{
		if (_battlePassSettingsProvider.TryGetBattlePass(calendarModel.BalanceId, out var battlePass))
		{
			BattlePassDataCreateCase battlePassDataCreateCase = new BattlePassDataCreateCase();
			battlePassDataCreateCase.BattlePass = battlePass;
			string saveKey2 = $"currency_battle_passe_{battlePassDataCreateCase.BattlePass.ID}";
			SimpleCurrency currency = simpleCurrencyFactory.Create(CurrencyType.BP, 0, saveKey2);
			BattlePasLevelInfoCase battlePasLevelInfoCase = new BattlePasLevelInfoCase(battlePass, currency, saver);
			battlePassDataCreateCase.BattlePasLevelInfoCase = battlePasLevelInfoCase;
			BattlePassRewardDataLogics battlePassRewardDataLogics = new BattlePassRewardDataLogics(battlePass);
			saver.Add(battlePassRewardDataLogics);
			battlePassDataCreateCase.RewardDataLogics = battlePassRewardDataLogics;
			string saveKey3 = $"startData_battle_passe_{battlePassDataCreateCase.BattlePass.ID}";
			BattlePassStartData battlePassStartData = new BattlePassStartData(saver, saveKey3, battlePass.PremiumPurchasedLocker);
			battlePassStartData.Initialize(battlePass.PremiumPurchaseProperty);
			battlePassDataCreateCase.BattlePassStartData = battlePassStartData;
			CreateRewardPairData(battlePassDataCreateCase, battlePass);
			BattlePassMergedCurrencyDataCase battlePassMergedCurrencyDataCase = new BattlePassMergedCurrencyDataCase($"merged_currency_battle_pass{battlePassDataCreateCase.BattlePass.ID}");
			battlePassMergedCurrencyDataCase.Reset();
			saver.Add(battlePassMergedCurrencyDataCase);
			battlePassDataCreateCase.MergedCurrencyDataCase = battlePassMergedCurrencyDataCase;
			battlePassDataCreateCase.BattlePass.Data.Initialization(battlePassDataCreateCase);
			battlePassDataCreateCase.BattlePass.Initialization();
			PreloadLootBoxes(battlePass);
		}
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
