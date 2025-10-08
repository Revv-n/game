using System;
using GreenT.HornyScapes.Events.Content;
using GreenT.HornyScapes.MergeStore;
using Merge;
using StripClub.Model.Shop;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.MergeCore.Collection;

public class CollectionWindowItem : MonoBehaviour, IPoolReturner
{
	[SerializeField]
	private Image icon;

	[SerializeField]
	private Image arrow;

	[SerializeField]
	private Text countLabel;

	[SerializeField]
	private Image lockedIcon;

	[SerializeField]
	private Sprite activeArrow;

	[SerializeField]
	private Sprite passiveArrow;

	[SerializeField]
	private TwoColors counterColors;

	[SerializeField]
	private MergeStoreNotifiedButton _mergeStoreNotifiedButton;

	private IMergeIconProvider _iconProvider;

	public CollectionController.CollectionMemento.CItem Node { get; private set; }

	public Action ReturnInPool { get; set; }

	[Inject]
	public void Init(IMergeIconProvider iconProvider)
	{
		_iconProvider = iconProvider;
	}

	public void SetManager(IMergeIconProvider iconProvider, ItemsCluster assortment, ContentSelectorGroup contentSelectorGroup, ICurrencyProcessor currencyProcessor, GameSettings gameSettings, NoCurrencyTabOpener noCurrencyTabOpener)
	{
		_iconProvider = iconProvider;
		_mergeStoreNotifiedButton.Init(assortment, contentSelectorGroup, currencyProcessor, gameSettings, noCurrencyTabOpener);
	}

	public void SetCItem(CollectionController.CollectionMemento.CItem node, bool needShowAllCollection = false)
	{
		_mergeStoreNotifiedButton.Set(node.Key, ButtonPosition.InfoTab);
		Node = node;
		countLabel.text = Node.Key.ID.ToString();
		bool flag = needShowAllCollection || _mergeStoreNotifiedButton.Use || Node.Opened;
		icon.sprite = (flag ? _iconProvider.GetSprite(node.Key) : null);
		SetOpened(flag);
	}

	public CollectionWindowItem SetArrowActive(bool active)
	{
		arrow.sprite = (active ? activeArrow : passiveArrow);
		return this;
	}

	public CollectionWindowItem SetArrowVisible(bool visible)
	{
		arrow.SetActive(visible);
		return this;
	}

	private void SetOpened(bool opened)
	{
		lockedIcon.SetActive(!opened);
		icon.SetActive(opened);
		countLabel.color = counterColors.GetColor(opened);
	}
}
