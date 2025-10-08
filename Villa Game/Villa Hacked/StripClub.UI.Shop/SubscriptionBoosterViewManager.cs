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

public class SubscriptionBoosterViewManager : SmallCardsViewManager
{
	private const string BindID = "booster";

	public SubscriptionBoosterViewManager(FakeAssetService fakeAssetService, [Inject(Id = "booster")] MergeItemDropView.Manager itemOptionViewManager, [Inject(Id = "booster")] CurrencyDropView.Manager currencyOptionViewManager, [Inject(Id = "booster")] BoosterDropView.Manager boosterOptionManager, [Inject(Id = "booster")] CardDropView.Manager cardOptionViewManager, [Inject(Id = "booster")] SkinDropView.Manager skinOptionViewManager, [Inject(Id = "booster")] LevelDropView.Manager levelManager, [Inject(Id = "booster")] DecorationDropView.Manager decorationOptionViewManager, [Inject(Id = "booster")] DecorationDropViewWithRarity.Manager decorationOptionViewWithRarityManager, [Inject(Id = "booster")] LootboxDropView.Manager lootboxOptionViewManager, LootboxCollection lootboxCollection, CardsCollection cards, MergeIconService iconProvider, RoomManager house, SkinManager skinManager, DecorationManager decorationManager, BoosterMapperManager boosterMapperManager, GameItemConfigManager gameItemConfigManager, [Inject(Id = "booster")] PresentDropView.Manager presentOptionViewManager)
		: base(itemOptionViewManager, currencyOptionViewManager, boosterOptionManager, cardOptionViewManager, cards, iconProvider, skinOptionViewManager, skinManager, decorationManager, house, decorationOptionViewManager, fakeAssetService, decorationOptionViewWithRarityManager, levelManager, boosterMapperManager, lootboxOptionViewManager, lootboxCollection, gameItemConfigManager, presentOptionViewManager)
	{
	}
}
