using System;
using GreenT.HornyScapes.MergeCore;
using UnityEngine;
using UnityEngine.UI;

namespace Merge.Core.Collection;

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

	public CollectionController.CollectionMemento.CItem Node { get; private set; }

	public Action ReturnInPool { get; set; }

	public void SetCItem(CollectionController.CollectionMemento.CItem node, bool needShowAllCollection = false)
	{
		Node = node;
		countLabel.text = Node.Key.ID.ToString();
		icon.sprite = (Node.Opened ? IconProvider.GetGISprite(node.Key) : null);
		SetOpened(Node.Opened);
		if (needShowAllCollection)
		{
			icon.sprite = IconProvider.GetGISprite(node.Key);
			SetOpened(needShowAllCollection);
		}
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
