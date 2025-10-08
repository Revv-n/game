using GreenT.AssetBundles;
using GreenT.HornyScapes.Bank.UI;
using GreenT.HornyScapes.Booster;
using GreenT.HornyScapes.Characters.Skins;
using GreenT.HornyScapes.Characters.Skins.UI;
using GreenT.HornyScapes.Events;
using GreenT.HornyScapes.GameItems;
using GreenT.HornyScapes.Lootboxes;
using GreenT.HornyScapes.Meta;
using GreenT.HornyScapes.Meta.Decorations;
using GreenT.HornyScapes.Presents.UI;
using GreenT.HornyScapes.UI;
using StripClub.Model.Cards;
using StripClub.Model.Shop.UI;
using Zenject;

namespace StripClub.UI.Shop;

public class SubscriptionRechargeViewManager : SmallCardsViewManager
{
	private const string BindID = "recharge";

	public SubscriptionRechargeViewManager(FakeAssetService fakeAssetService, [Inject(Id = "recharge")] MergeItemDropView.Manager itemOptionViewManager, [Inject(Id = "recharge")] CurrencyDropView.Manager currencyOptionViewManager, [Inject(Id = "recharge")] BoosterDropView.Manager boosterOptionManager, [Inject(Id = "recharge")] CardDropView.Manager cardOptionViewManager, [Inject(Id = "recharge")] SkinDropView.Manager skinOptionViewManager, [Inject(Id = "recharge")] LevelDropView.Manager levelManager, [Inject(Id = "recharge")] DecorationDropView.Manager decorationOptionViewManager, [Inject(Id = "recharge")] DecorationDropViewWithRarity.Manager decorationOptionViewWithRarityManager, [Inject(Id = "recharge")] LootboxDropView.Manager lootboxOptionViewManager, LootboxCollection lootboxCollection, CardsCollection cards, MergeIconService iconProvider, RoomManager house, SkinManager skinManager, DecorationManager decorationManager, BoosterMapperManager boosterMapperManager, GameItemConfigManager gameItemConfigManager, [Inject(Id = "recharge")] PresentDropView.Manager presentOptionViewManager)
		: base(itemOptionViewManager, currencyOptionViewManager, boosterOptionManager, cardOptionViewManager, cards, iconProvider, skinOptionViewManager, skinManager, decorationManager, house, decorationOptionViewManager, fakeAssetService, decorationOptionViewWithRarityManager, levelManager, boosterMapperManager, lootboxOptionViewManager, lootboxCollection, gameItemConfigManager, presentOptionViewManager)
	{
	}
}
