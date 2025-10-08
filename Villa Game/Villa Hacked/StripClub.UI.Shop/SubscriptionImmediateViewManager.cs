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

public class SubscriptionImmediateViewManager : SmallCardsViewManager
{
	private const string BindID = "immediate";

	public SubscriptionImmediateViewManager(FakeAssetService fakeAssetService, [Inject(Id = "immediate")] MergeItemDropView.Manager itemOptionViewManager, [Inject(Id = "immediate")] CurrencyDropView.Manager currencyOptionViewManager, [Inject(Id = "immediate")] BoosterDropView.Manager boosterOptionManager, [Inject(Id = "immediate")] CardDropView.Manager cardOptionViewManager, [Inject(Id = "immediate")] SkinDropView.Manager skinOptionViewManager, [Inject(Id = "immediate")] LevelDropView.Manager levelManager, [Inject(Id = "immediate")] DecorationDropView.Manager decorationOptionViewManager, [Inject(Id = "immediate")] DecorationDropViewWithRarity.Manager decorationOptionViewWithRarityManager, [Inject(Id = "immediate")] LootboxDropView.Manager lootboxOptionViewManager, LootboxCollection lootboxCollection, CardsCollection cards, MergeIconService iconProvider, RoomManager house, SkinManager skinManager, DecorationManager decorationManager, BoosterMapperManager boosterMapperManager, GameItemConfigManager gameItemConfigManager, [Inject(Id = "immediate")] PresentDropView.Manager presentOptionViewManager)
		: base(itemOptionViewManager, currencyOptionViewManager, boosterOptionManager, cardOptionViewManager, cards, iconProvider, skinOptionViewManager, skinManager, decorationManager, house, decorationOptionViewManager, fakeAssetService, decorationOptionViewWithRarityManager, levelManager, boosterMapperManager, lootboxOptionViewManager, lootboxCollection, gameItemConfigManager, presentOptionViewManager)
	{
	}
}
