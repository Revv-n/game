using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using GreenT.HornyScapes.Analytics;
using GreenT.HornyScapes.Animations;
using GreenT.HornyScapes.Bank.UI;
using GreenT.HornyScapes.Constants;
using GreenT.HornyScapes.Events;
using GreenT.Types;
using Merge;
using Merge.Core.Inventory;
using StripClub.Model;
using StripClub.Model.Shop;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.MergeCore.Inventory;

public class MergeInventoryWindow : PopupWindow
{
	[SerializeField]
	private InventoryWindowItem prefab;

	[SerializeField]
	private InventoryWindowBuyButton buyButton;

	[SerializeField]
	private RectTransform parent;

	[SerializeField]
	private Button closeButton;

	[SerializeField]
	private CurrencyType priceType;

	[SerializeField]
	private OpenSection sectionOpener;

	private SmartPool<InventoryWindowItem> pool;

	[HideInInspector]
	public InventoryController.GeneralData Data;

	public InventorySettingsProvider Config;

	private float widthItem;

	private ICurrencyProcessor currencyProcessor;

	private MergeIconService _iconProvider;

	private int tabID;

	private CurrencyAmplitudeAnalytic currencyAmplitudeAnalytic;

	private int BuySlotPrice => Config.GetSlotUnlockPrice(Data.OpenedSlots);

	public event Action<InventoryController.GeneralData.ItemNode> OnItemClick;

	public event Action OnSlotBuy;

	[Inject]
	public void Init(ICurrencyProcessor currencyProcessor, IConstants<int> constants, MergeIconService iconProvider, CurrencyAmplitudeAnalytic currencyAmplitudeAnalytic)
	{
		this.currencyAmplitudeAnalytic = currencyAmplitudeAnalytic;
		this.currencyProcessor = currencyProcessor;
		_iconProvider = iconProvider;
		tabID = constants["banktab_no_hards"];
	}

	public virtual void Start()
	{
		sectionOpener.Set(tabID);
	}

	public override void Open()
	{
		base.Open();
		fadeCloser.SetActive(active: true);
	}

	public override void Close()
	{
		base.Close();
		fadeCloser.SetActive(active: false);
	}

	public void Init(InventorySettingsProvider config, InventoryController.GeneralData data)
	{
		if (pool != null)
		{
			pool.ReturnAllItems();
		}
		Config = config;
		Data = data;
		pool = new SmartPool<InventoryWindowItem>(prefab, parent).ActivateDefaultTransformValidation();
		GridLayoutGroup component = parent.GetComponent<GridLayoutGroup>();
		widthItem = component.cellSize.x + component.spacing.x - (float)(component.padding.left / 4);
		closeButton.onClick.AddListener(Close);
	}

	public MergeInventoryWindow Repaint()
	{
		pool.ReturnAllItems();
		List<InventoryWindowItem> list = pool.Pop(Config.StartOpenSlotsCount + Config.ClosedSlotsCount, delegate(InventoryWindowItem x)
		{
			x.OnItemClick += AtItemClick;
		});
		list.ForEach(delegate(InventoryWindowItem x)
		{
			x.SetEmpty();
		});
		for (int i = 0; i < Data.StoredItems.Count; i++)
		{
			list[i].Init(_iconProvider);
			list[i].SetItem(Data.StoredItems[i]);
		}
		for (int j = Data.OpenedSlots; j < Config.StartOpenSlotsCount + Config.ClosedSlotsCount; j++)
		{
			list[j].SetLocked();
		}
		ValidateBuyButton();
		return this;
	}

	public void DoFlyObject(GameItem pulled, InventoryController.GeneralData.ItemNode node)
	{
		GIGhost ghost = GIGhost.CreateGhost(pulled);
		ghost.IconRenderer.SetOrder("UIOverlay", 1);
		InventoryWindowItem inventoryWindowItem = pool.ActiveElements.First((InventoryWindowItem x) => x.ContainedNode == node);
		float num = 0.4f;
		Sequence sequence = DOTween.Sequence();
		sequence.Append(EasyBezierTweener.DoBezier(ghost.transform, inventoryWindowItem.Anchor, pulled.Position, num));
		sequence.Join(DOTweenModuleSprite.DOFade(ghost.IconRenderer, 0f, num));
		sequence.onComplete = (TweenCallback)Delegate.Combine(sequence.onComplete, (TweenCallback)delegate
		{
			ghost.Destroy();
		});
	}

	private void ValidateBuyButton()
	{
		if (buyButton != null)
		{
			buyButton.SetActive(active: false);
			buyButton.OnClick -= AtBuy;
		}
		if (Data.OpenedSlots < parent.childCount)
		{
			buyButton = parent.GetChild(Data.OpenedSlots).GetComponent<InventoryWindowItem>().Button;
			buyButton.SetActive(active: true);
			buyButton.OnClick += AtBuy;
			buyButton.Init(currencyProcessor);
			buyButton.SetPrice(BuySlotPrice);
		}
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		if ((bool)buyButton)
		{
			buyButton.OnClick -= AtBuy;
		}
	}

	private void AtBuy()
	{
		int buySlotPrice = BuySlotPrice;
		if (currencyProcessor.TrySpent(priceType, buySlotPrice))
		{
			Data.OpenedSlots++;
			currencyAmplitudeAnalytic.SendSpentEvent(priceType, buySlotPrice, CurrencyAmplitudeAnalytic.SourceType.BoughtCellInventory, ContentType.Main);
			this.OnSlotBuy?.Invoke();
			Repaint();
		}
		else
		{
			sectionOpener.Open();
		}
	}

	private void RecalculateScroll()
	{
		int num = (parent.childCount + 1) / 3 - 7;
		if (num > 0)
		{
			DOTweenModuleUI.DOAnchorPosX(parent, (0f - widthItem) * (float)num, 1f);
		}
	}

	private void AtItemClick(InventoryWindowItem sender)
	{
		this.OnItemClick?.Invoke(sender.ContainedNode);
	}
}
