using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.Card.Bonus;
using GreenT.HornyScapes.Characters;
using GreenT.HornyScapes.Constants;
using GreenT.HornyScapes.Events;
using GreenT.HornyScapes.GameItems;
using GreenT.HornyScapes.MergeCore.GameItemBox;
using GreenT.Localizations;
using GreenT.Types;
using Merge;
using StripClub.Extensions;
using StripClub.Model;
using StripClub.Model.Cards;
using StripClub.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.MergeCore.UI;

public class InfoPanel : MonoBehaviour
{
	private const string KeyMergeItemMax = "ui.merge.item.max";

	private const string KeyMergeItem = "ui.merge.item";

	private const string KeyLockBox = "ui.merge.item.title.lock";

	private const string KeyLockWeb = "ui.merge.item.lock";

	private const string KeyLockBubble = "ui.merge.item.bubble";

	private const string KeyLockChest = "ui.merge.chest.lock";

	private const string KeyUnlockChest = "ui.merge.chest.unlock";

	private const string KeyCollectableAndMergableItem = "item.collect.info";

	private const string KeyCollectable = "item.collect.max.info";

	[SerializeField]
	private Image icon;

	[SerializeField]
	private Image bubbleIcon;

	[SerializeField]
	private LocalizedTextMeshPro nameLabel;

	[SerializeField]
	private GameObject levelContainer;

	[SerializeField]
	private LocalizedTextMeshPro levelLabel;

	[SerializeField]
	private LocalizedTextMeshPro descriptionLabel;

	[SerializeField]
	private Button infoButton;

	[SerializeField]
	private GameObject bonusArea;

	[SerializeField]
	private GameObject clickSpawnArea;

	[SerializeField]
	private GameObject respawnTimer;

	[SerializeField]
	private LocalizedTextMeshPro spawnAmount;

	[SerializeField]
	private TMP_Text respawnTime;

	[SerializeField]
	private StackNextItemsContainer nextItemsContainer;

	[SerializeField]
	private StackNextItemsContainer nextItemsContainerTest;

	[SerializeField]
	private StackNextItemsContainer currentItemsContainerTest;

	[SerializeField]
	private GameObject normalState;

	[SerializeField]
	private List<ModuleActionOperator> operators;

	[SerializeField]
	private EventGirlCardView cardView;

	[SerializeField]
	private StatableComponent spawnerStatable;

	public GameObject UnSelectedView;

	private IConstants<ILocker> lockerConstant;

	private ModifyController modifyController;

	private GameSettings gameSettings;

	private CardsCollection cards;

	private TimeHelper timeHelper;

	private LocalizationService _localizationService;

	private GameItemConfigManager gameItemConfigManager;

	public GameItem SelectedItem { get; private set; }

	public event Action OnUnsell;

	public event Action OnInfo;

	public event Action<GIBox.Base> OnAction;

	[Inject]
	public void Init(IConstants<ILocker> lockerConstant, GameSettings gameSettings, ModifyController modifyController, CardsCollection cards, TimeHelper timeHelper, LocalizationService localizationService, GameItemConfigManager gameItemConfigManager)
	{
		this.lockerConstant = lockerConstant;
		this.gameSettings = gameSettings;
		this.modifyController = modifyController;
		this.cards = cards;
		this.timeHelper = timeHelper;
		_localizationService = localizationService;
		this.gameItemConfigManager = gameItemConfigManager;
	}

	public void Set(GameItem gi)
	{
		SelectedItem = gi;
		if (gi == null)
		{
			SetDefault();
			return;
		}
		ChangeState(isActive: true);
		if (TryGetLockedIcon(out var sprite2))
		{
			icon.sprite = sprite2;
			nameLabel.Init("ui.merge.item.title.lock");
			levelContainer.SetActive(value: false);
			bubbleIcon.SetActive(active: false);
			infoButton.SetActive(active: false);
		}
		else
		{
			levelContainer.SetActive(value: true);
			icon.sprite = gi.Icon;
			nameLabel.Init(gi.Config.GameItemName);
			levelLabel.SetArguments(gi.Config.Level);
			bubbleIcon.SetActive(gi.Data.HasModule(GIModuleType.Bubble));
			infoButton.SetActive(active: true);
		}
		LocalizeGameItemDescription(gi);
		ClickSpawnHandler(gi);
		StackHandler(gi);
		MixerHandler(gi);
		ShowOperators(gi);
		bool TryGetLockedIcon(out Sprite sprite)
		{
			if (gi.TryGetBox<Locked>(out var box) && box.LockedIcon != null)
			{
				sprite = box.LockedIcon.sprite;
				return true;
			}
			sprite = null;
			return false;
		}
	}

	private void StackHandler(GameItem gi)
	{
		UnSubscribeOnStackChanged();
		if (gi.HasBox(GIModuleType.Stack) && gi.AllowInteraction(GIModuleType.Stack))
		{
			GIBox.Stack box = gi.GetBox<GIBox.Stack>();
			SubscribeOnStackChanged();
			SetupGirlView(gi);
			UpdateStackContainer(box);
			bonusArea.SetActive(value: true);
			nextItemsContainer.gameObject.SetActive(value: true);
			nextItemsContainerTest.gameObject.SetActive(value: true);
			currentItemsContainerTest.gameObject.SetActive(value: true);
		}
		else if (nextItemsContainer.gameObject.activeSelf)
		{
			nextItemsContainer.Clear();
			nextItemsContainerTest.Clear();
			currentItemsContainerTest.Clear();
			nextItemsContainer.gameObject.SetActive(value: false);
			nextItemsContainerTest.gameObject.SetActive(value: false);
			currentItemsContainerTest.gameObject.SetActive(value: false);
		}
	}

	private void SubscribeOnStackChanged()
	{
		Controller<StackController>.Instance.OnPullItem += OnStackChanged;
		Controller<StackController>.Instance.OnPushItem += OnStackChanged;
	}

	private void UnSubscribeOnStackChanged()
	{
		Controller<StackController>.Instance.OnPullItem -= OnStackChanged;
		Controller<StackController>.Instance.OnPushItem -= OnStackChanged;
	}

	private void OnStackChanged(GameItem stackItem, GameItem gameItem)
	{
		if (!(SelectedItem == null) && SelectedItem.HasBox(GIModuleType.Stack))
		{
			GIBox.Stack box = SelectedItem.GetBox<GIBox.Stack>();
			GIBox.Stack box2 = stackItem.GetBox<GIBox.Stack>();
			if (box == box2)
			{
				UpdateStackContainer(box);
			}
		}
	}

	private void UpdateStackContainer(GIBox.Stack stack)
	{
		ShowOperators(stack.Parent);
		List<GIKey> nextItems = stack.nextItems;
		List<GIKey> items = stack.Data.Items.Select((WeightNode<GIData> item) => item.value.Key).ToList();
		nextItemsContainer.Clear();
		nextItemsContainerTest?.Clear();
		currentItemsContainerTest?.Clear();
		nextItemsContainer.SetStack(stack);
		nextItemsContainerTest?.SetItems(nextItems, isFindSpawner: false);
		currentItemsContainerTest?.SetItems(items, isFindSpawner: false);
	}

	private void MixerHandler(GameItem gi)
	{
		UnSubscribeOnMixerChanged();
		if (gi.HasBox(GIModuleType.Mixer) && gi.AllowInteraction(GIModuleType.Mixer))
		{
			GIBox.Mixer box = gi.GetBox<GIBox.Mixer>();
			SubscribeOnMixerChanged();
			SetupGirlView(gi);
			UpdateMixerContainer(box);
			descriptionLabel.SetActive(active: false);
			bonusArea.SetActive(value: true);
			clickSpawnArea.SetActive(value: true);
			bool active = box.ActiveRecipe.Time > 0;
			respawnTimer.SetActive(active);
		}
	}

	private void SubscribeOnMixerChanged()
	{
		Controller<MixerController>.Instance.OnUpdateMixer += OnMixerChanged;
	}

	private void UnSubscribeOnMixerChanged()
	{
		Controller<MixerController>.Instance.OnUpdateMixer -= OnMixerChanged;
	}

	private void OnMixerChanged(GameItem mixerItem)
	{
		if (SelectedItem == null || !SelectedItem.HasBox(GIModuleType.Mixer))
		{
			return;
		}
		GIBox.Mixer box = SelectedItem.GetBox<GIBox.Mixer>();
		GIBox.Mixer box2 = mixerItem.GetBox<GIBox.Mixer>();
		if (box == box2)
		{
			if (box.ActiveRecipe != null)
			{
				UpdateMixerContainer(box);
			}
			else if (SelectedItem.HasBox(GIModuleType.Stack))
			{
				UnSubscribeOnMixerChanged();
				Set(SelectedItem);
			}
		}
	}

	private void UpdateMixerContainer(GIBox.Mixer mixer)
	{
		int modifiedMaxItemAmount = mixer.ModifiedMaxItemAmount;
		spawnAmount.SetArguments(mixer.Data.Amount, modifiedMaxItemAmount);
		TimeSpan timeSpan = TimeSpan.FromSeconds(mixer.ModifiedMaxMixingTime);
		respawnTime.text = timeHelper.UseCombineFormat(timeSpan);
	}

	private void ClickSpawnHandler(GameItem gi)
	{
		UnSubscribe();
		GIBox.ClickSpawn clickSpawn = gi.Boxes.FirstOrDefault((GIBox.Base x) => x is GIBox.ClickSpawn) as GIBox.ClickSpawn;
		bool flag = clickSpawn != null;
		if (flag)
		{
			SetupSpawnSettingsView(clickSpawn);
			SetupGirlView(gi);
			Subscribe();
		}
		descriptionLabel.SetActive(!flag);
		bonusArea.SetActive(flag);
		clickSpawnArea.SetActive(flag);
		bool active = flag && clickSpawn.Config.RestoreTime > 0f;
		respawnTimer.SetActive(active);
		void OnEmptySpawner(GameItem root, GameItem created)
		{
			GIBox.ClickSpawn box = root.GetBox<GIBox.ClickSpawn>();
			if (box.Data.Amount == 0)
			{
				ShowOperators(root);
			}
			SetupSpawnSettingsView(box);
		}
		void Subscribe()
		{
			Controller<ClickSpawnController>.Instance.OnClickSpawn += OnEmptySpawner;
			Controller<ClickSpawnController>.Instance.OnRefreshSpawner += SetupSpawnSettingsView;
		}
		void UnSubscribe()
		{
			Controller<ClickSpawnController>.Instance.OnClickSpawn -= OnEmptySpawner;
			Controller<ClickSpawnController>.Instance.OnRefreshSpawner -= SetupSpawnSettingsView;
		}
	}

	private void SetupSpawnSettingsView(GIBox.ClickSpawn clickSpawn)
	{
		int num = modifyController.CalcMaxAmount(clickSpawn);
		spawnAmount.SetArguments(clickSpawn.Data.Amount, num);
		TimeSpan timeSpan = TimeSpan.FromSeconds(modifyController.RestoreTime(clickSpawn));
		respawnTime.text = timeHelper.UseCombineFormat(timeSpan);
	}

	private void SetupGirlView(GameItem gi)
	{
		ICharacter character = cards.Collection.OfType<ICharacter>().FirstOrDefault(IsCardHasEffectOnSpawner);
		if (character != null)
		{
			cardView.Set(character);
		}
		spawnerStatable.Set((character == null) ? 1 : 0);
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
			return characterMultiplierBonus.AffectedSpawnerId.Any((int _id) => _id == gi.Config.UniqId);
		}
	}

	private void ShowOperators(GameItem gi)
	{
		foreach (ModuleActionOperator @operator in operators)
		{
			if (@operator.IsActive)
			{
				@operator.Deactivate();
			}
		}
		List<GIBox.Base> actionBoxes = (from x in gi.Boxes
			where x is IActionModule && (x as IActionModule).IsActionEnable
			orderby (x as IActionModule).ActionPriority descending
			select x).ToList();
		List<FilterNode<GIModuleType>> list = (from x in gi.Boxes
			where x is IBlockModulesAction
			select (x as IBlockModulesAction).ActionsFilter).ToList();
		if (list.Any())
		{
			actionBoxes = List.FilterOut(actionBoxes, list, (GIBox.Base b, GIModuleType f) => b.ModuleType == f);
		}
		int i;
		for (i = 0; i < Mathf.Min(actionBoxes.Count, 2); i++)
		{
			if (actionBoxes[i].ModuleType != GIModuleType.AutoSpawn)
			{
				ModuleActionOperator moduleActionOperator = operators.First((ModuleActionOperator x) => x.Type == actionBoxes[i].ModuleType);
				moduleActionOperator.SetActive(moduleActionOperator.Type != GIModuleType.Sell || CheckSellModule(gi));
				moduleActionOperator.SetBox(actionBoxes[i]);
			}
		}
	}

	private bool CheckSellModule(GameItem gi)
	{
		if (!lockerConstant["sell_mergeItem"].IsOpen.Value)
		{
			return false;
		}
		if (gi.Config.Type == GameItemType.Item)
		{
			return true;
		}
		List<GameItem> source = Controller<GameItemController>.Instance.FindItems(gi.Config.Key, gi.Config.Type);
		IEnumerable<GameItem> source2 = source.Where((GameItem _item) => _item.HasBox(GIModuleType.ClickSpawn) && !_item.HasBox(GIModuleType.Locked) && !_item.HasBox(GIModuleType.Bubble));
		IEnumerable<GameItem> source3 = source.Where((GameItem _item) => _item.HasBox(GIModuleType.Stack) && !_item.HasBox(GIModuleType.Locked) && !_item.HasBox(GIModuleType.Bubble));
		if ((source2.Any((GameItem _item) => _item.Config.Level >= gi.Config.Level && _item != gi) || source3.Any((GameItem _item) => _item.Config.Level >= gi.Config.Level && _item != gi)) && !gi.HasBox(GIModuleType.Locked))
		{
			return !gi.HasBox(GIModuleType.Bubble);
		}
		return false;
	}

	private void LocalizeGameItemDescription(GameItem gi)
	{
		if (gi.Data.HasModule(GIModuleType.Locked) && gi.Data.GetModule<ModuleDatas.Locked>().BlocksMerge)
		{
			descriptionLabel.Init("ui.merge.item.lock");
			return;
		}
		if (gi.Data.HasModule(GIModuleType.Bubble))
		{
			descriptionLabel.Init("ui.merge.item.bubble");
			return;
		}
		if (gi.Data.HasModule(GIModuleType.Chest))
		{
			ModuleDatas.Chest module = gi.Data.GetModule<ModuleDatas.Chest>();
			if (!module.AlreadyOpened && !module.IsOpeningNow)
			{
				descriptionLabel.Init("ui.merge.chest.lock");
			}
			else
			{
				descriptionLabel.Init("ui.merge.chest.unlock");
			}
			return;
		}
		ModuleConfigs.Collect.CurrencyParams currencyParams2;
		if (gi.Config.TryGetModule<ModuleConfigs.Collect>(out var result) && gi.Config.TryGetModule<ModuleConfigs.Merge>(out var result2))
		{
			ModuleConfigs.Collect.CurrencyParams currencyParams;
			bool flag = result.TryGetCurrencyParams(out currencyParams);
			if (gi.AllowInteraction(GIModuleType.Merge) && flag)
			{
				string text = gameSettings.CurrencySettings[currencyParams.CurrencyType, default(CompositeIdentificator)].Key;
				if (currencyParams.CurrencyType == CurrencyType.MiniEvent)
				{
					text = string.Format(text, currencyParams.CompositeIdentificator[0]);
				}
				descriptionLabel.Init("item.collect.info", currencyParams.Amount, _localizationService.Text(text));
				return;
			}
		}
		else if (gi.Config.TryGetModule<ModuleConfigs.Collect>(out result) && result.TryGetCurrencyParams(out currencyParams2))
		{
			string text2 = gameSettings.CurrencySettings[currencyParams2.CurrencyType, currencyParams2.CompositeIdentificator].Key;
			if (currencyParams2.CurrencyType == CurrencyType.MiniEvent)
			{
				text2 = string.Format(text2, currencyParams2.CompositeIdentificator[0]);
			}
			descriptionLabel.Init("item.collect.max.info", currencyParams2.Amount, _localizationService.Text(text2));
			return;
		}
		if (gi.Config.TryGetModule<ModuleConfigs.Merge>(out result2) && gi.AllowInteraction(GIModuleType.Merge))
		{
			GIConfig configOrNull = gameItemConfigManager.GetConfigOrNull(result2.MergeResult.Key);
			if (configOrNull != null)
			{
				string gameItemName = configOrNull.GameItemName;
				descriptionLabel.Init("ui.merge.item", _localizationService.Text(gameItemName));
				return;
			}
		}
		descriptionLabel.Init("ui.merge.item.max", _localizationService.Text(gi.Config.GameItemName));
	}

	private void SetDefault()
	{
		ChangeState(isActive: false);
		operators.ForEach(delegate(ModuleActionOperator x)
		{
			x.SetActive(active: false);
		});
	}

	private void ChangeState(bool isActive)
	{
		base.gameObject.SetActive(isActive);
		UnSelectedView.gameObject.SetActive(!isActive);
	}

	private void Start()
	{
		SetDefault();
		infoButton.SetActive(active: true);
		infoButton.onClick.AddListener(AtInfoClick);
		foreach (ModuleActionOperator @operator in operators)
		{
			@operator.OnAction += AtBlockAction;
		}
	}

	private void AtInfoClick()
	{
		Controller<CollectionController>.Instance.ShowWindow(SelectedItem.Config.Key);
	}

	private void AtBlockAction(ModuleActionOperator obj)
	{
		this.OnAction?.Invoke(obj.GetBox());
	}

	protected virtual void OnDisable()
	{
		UnSubscribeOnStackChanged();
		UnSubscribeOnMixerChanged();
		if ((bool)Controller<SelectionController>.Instance)
		{
			Controller<SelectionController>.Instance.ClearSelection();
		}
	}

	private void OnDestroy()
	{
		infoButton.onClick.RemoveAllListeners();
	}
}
