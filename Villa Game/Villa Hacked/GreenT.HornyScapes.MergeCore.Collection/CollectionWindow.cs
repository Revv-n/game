using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.Card.Bonus;
using GreenT.HornyScapes.Characters;
using GreenT.HornyScapes.Events;
using GreenT.HornyScapes.Events.Content;
using GreenT.HornyScapes.GameItems;
using GreenT.HornyScapes.Info.UI;
using GreenT.HornyScapes.MergeStore;
using GreenT.Multiplier;
using Merge;
using Merge.Core.Collection;
using Merge.Core.Windows;
using StripClub.Model.Cards;
using StripClub.Model.Shop;
using StripClub.UI;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.MergeCore.Collection;

public class CollectionWindow : PopupWindow
{
	private class GIDataComparer : IEqualityComparer<GIData>
	{
		public bool Equals(GIData x, GIData y)
		{
			if (x != null && y != null)
			{
				return x.Key == y.Key;
			}
			return false;
		}

		public int GetHashCode(GIData data)
		{
			return data.Key.GetHashCode();
		}
	}

	[SerializeField]
	private CollectionWindowItem prefab;

	[SerializeField]
	private Transform parent;

	[SerializeField]
	private int columns = 4;

	[SerializeField]
	private Image iconItem;

	[SerializeField]
	private ContainerWindowGrid grid;

	[SerializeField]
	private LocalizedTextMeshPro namelbl;

	[SerializeField]
	private LocalizedTextMeshPro levelLbl;

	[SerializeField]
	private List<ContainerWindowItem> items;

	[SerializeField]
	private InfoBonusesView bonusesView;

	public GIConfig Source;

	private SmartPool<CollectionWindowItem> pool;

	private CompareGIData compareGIData;

	private CardsCollection cards;

	private IGenericViewManager<ICard> viewManager;

	private MergeIconService _iconProvider;

	private ItemsCluster _assortment;

	private GameSettings _gameSettings;

	private ContentSelectorGroup _contentSelectorGroup;

	private ICurrencyProcessor _currencyProcessor;

	private NoCurrencyTabOpener _noCurrencyTabOpener;

	private MultiplierManager _multiplierManager;

	[Inject]
	public void Init(CardsCollection cards, IGenericViewManager<ICard> viewManager, MergeIconService iconProvider, ItemsCluster assortment, ContentSelectorGroup contentSelectorGroup, ICurrencyProcessor currencyProcessor, GameSettings gameSettings, NoCurrencyTabOpener noCurrencyTabOpener, MultiplierManager multiplierManager)
	{
		this.cards = cards;
		this.viewManager = viewManager;
		_iconProvider = iconProvider;
		_assortment = assortment;
		_contentSelectorGroup = contentSelectorGroup;
		_currencyProcessor = currencyProcessor;
		_gameSettings = gameSettings;
		_noCurrencyTabOpener = noCurrencyTabOpener;
		_multiplierManager = multiplierManager;
	}

	public void Init()
	{
		compareGIData = new CompareGIData();
		pool = new SmartPool<CollectionWindowItem>(prefab, parent).ActivateDefaultTransformValidation();
	}

	public CollectionWindow SetCollection(CollectionController.CollectionMemento.CGroup group, GIConfig giConfig, GameItem selectedItem, bool needTopPart, bool needShowAllCollection = false)
	{
		Source = giConfig;
		bonusesView.Init(giConfig);
		DisplayInfluencingGirls(giConfig);
		pool.ReturnAllItems();
		CollectionWindowItem collectionWindowItem = null;
		int num = 0;
		foreach (CollectionController.CollectionMemento.CItem item in group.Items)
		{
			CollectionWindowItem collectionWindowItem2 = pool.Pop();
			collectionWindowItem2.SetManager(_iconProvider, _assortment, _contentSelectorGroup, _currencyProcessor, _gameSettings, _noCurrencyTabOpener);
			collectionWindowItem2.SetCItem(item, needShowAllCollection);
			collectionWindowItem2.transform.SetAsLastSibling();
			if (collectionWindowItem != null)
			{
				collectionWindowItem.SetArrowVisible(++num % columns != 0).SetArrowActive(item.Opened && collectionWindowItem.Node.Opened);
			}
			collectionWindowItem = collectionWindowItem2;
		}
		levelLbl.SetArguments(giConfig.Level);
		namelbl.Init(giConfig.GameItemName);
		grid.gameObject.SetActive(needTopPart);
		if (needTopPart)
		{
			items.ForEach(delegate(ContainerWindowItem x)
			{
				x.gameObject.SetActive(value: false);
			});
			ModuleConfigs.ClickSpawn module = giConfig.GetModule<ModuleConfigs.ClickSpawn>();
			ModuleConfigs.AutoSpawn module2 = giConfig.GetModule<ModuleConfigs.AutoSpawn>();
			ModuleConfigs.Stack module3 = giConfig.GetModule<ModuleConfigs.Stack>();
			ModuleConfigs.Mixer module4 = giConfig.GetModule<ModuleConfigs.Mixer>();
			List<GIData> list = new List<GIData>(16);
			AdjustedMultiplierDictionary<int, SummingCompositeMultiplier> spawnerProductionMultipliers = _multiplierManager.SpawnerProductionMultipliers;
			IMultiplier multiplier2;
			if (!giConfig.NotAffectedAll)
			{
				IMultiplier multiplier = spawnerProductionMultipliers.TotalByKey(giConfig.UniqId);
				multiplier2 = multiplier;
			}
			else
			{
				multiplier2 = spawnerProductionMultipliers.GetMultiplier(giConfig.UniqId);
			}
			IMultiplier multiplier3 = multiplier2;
			if (module != null)
			{
				list = module.SpawnPool.Select((WeightNode<GIData> x) => x.value).OrderBy((GIData x) => x, compareGIData).ToList();
				for (int i = 0; i < Mathf.Min(list.Count, items.Count); i++)
				{
					GIData currentItem = list[i];
					GIBox.ClickSpawn box = selectedItem.GetBox<GIBox.ClickSpawn>();
					WeightNode<GIData> weightNode = null;
					if (box != null)
					{
						weightNode = box.ModifiedSpawnPool.FirstOrDefault((WeightNode<GIData> node) => node.value.Key.Equals(currentItem.Key));
					}
					items[i].SetItem(currentItem, ClockAnabled: false).SetActive(weightNode != null && multiplier3 != null && 0f < weightNode.weight + (float)multiplier3.Factor.Value);
				}
			}
			if (module2 != null)
			{
				list.Clear();
				list = module2.SpawnPool.Select((WeightNode<GIData> x) => x.value).OrderBy((GIData x) => x, compareGIData).ToList();
				for (int j = module?.SpawnPool.Count ?? 0; j < Mathf.Min(list.Count + (module?.SpawnPool.Count ?? 0), items.Count); j++)
				{
					GIData currentItem = list[j - (module?.SpawnPool.Count ?? 0)];
					GIBox.AutoSpawn box2 = selectedItem.GetBox<GIBox.AutoSpawn>();
					WeightNode<GIData> weightNode2 = null;
					if (box2 != null)
					{
						weightNode2 = box2.ModifiedSpawnPool.FirstOrDefault((WeightNode<GIData> node) => node.value.Key.Equals(currentItem.Key));
					}
					items[j].SetItem(currentItem, ClockAnabled: true).SetActive(weightNode2 != null && multiplier3 != null && 0f < weightNode2.weight + (float)multiplier3.Factor.Value);
				}
			}
			if (module3 != null && module4 != null)
			{
				list.Clear();
				list = (from recipe in module3.ItemsPool
					select recipe.Result into result
					from item in result
					select item.value).Distinct(new GIDataComparer()).OrderBy((GIData x) => x, compareGIData).ToList();
				for (int k = 0; k < Mathf.Min(list.Count, items.Count); k++)
				{
					GIData currentItem = list[k];
					GIBox.AutoSpawn box3 = selectedItem.GetBox<GIBox.AutoSpawn>();
					WeightNode<GIData> weightNode3 = null;
					if (box3 != null)
					{
						weightNode3 = box3.ModifiedSpawnPool.FirstOrDefault((WeightNode<GIData> node) => node.value.Key.Equals(currentItem.Key));
					}
					items[k].SetItem(currentItem, ClockAnabled: false).SetActive(weightNode3 != null && multiplier3 != null && 0f < weightNode3.weight + (float)multiplier3.Factor.Value);
				}
			}
		}
		iconItem.sprite = _iconProvider.GetSprite(giConfig.Key);
		collectionWindowItem.SetArrowVisible(visible: false);
		return this;
	}

	private void DisplayInfluencingGirls(GIConfig config)
	{
		viewManager.HideAll();
		if (!config.TryGetModule<ModuleConfigs.ClickSpawn>(out var _))
		{
			return;
		}
		foreach (ICharacter item in cards.Collection.OfType<ICharacter>().Where(IsCardHasEffectOnSpawner))
		{
			viewManager.Display(item);
		}
		bool IsCardHasEffectOnSpawner(ICharacter _card)
		{
			if (!(_card.Bonus is CharacterMultiplierBonus characterMultiplierBonus))
			{
				return false;
			}
			if (cards.GetPromoteOrDefault(_card) == null)
			{
				return false;
			}
			if (characterMultiplierBonus.AffectAll && config.NotAffectedAll)
			{
				return false;
			}
			if (!characterMultiplierBonus.AffectAll)
			{
				return characterMultiplierBonus.AffectedSpawnerId.Any((int _id) => _id == config.UniqId);
			}
			return true;
		}
	}
}
