using System;
using GreenT.HornyScapes.Events;
using Merge;
using Merge.Core.Inventory;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.MergeCore.Inventory;

public class InventoryWindowItem : MonoBehaviour
{
	[SerializeField]
	private InventoryWindowBuyButton purchaseButton;

	[SerializeField]
	private Image itemIcon;

	[SerializeField]
	private Image background;

	[SerializeField]
	private Image lockIcon;

	[SerializeField]
	private Button itemButton;

	private MergeIconService _iconProvider;

	public InventoryWindowBuyButton Button => purchaseButton;

	public Vector3 Anchor => itemIcon.transform.position;

	public InventoryController.GeneralData.ItemNode ContainedNode { get; private set; }

	public event Action<InventoryWindowItem> OnItemClick;

	[Inject]
	public void Init(MergeIconService iconProvider)
	{
		_iconProvider = iconProvider;
	}

	public void SetItem(InventoryController.GeneralData.ItemNode node)
	{
		ContainedNode = node;
		itemIcon.sprite = _iconProvider.GetSprite(node.Item.Key);
		itemIcon.SetActive(active: true);
		background.enabled = true;
		lockIcon.SetActive(active: false);
	}

	public void SetEmpty()
	{
		ContainedNode = null;
		itemIcon.SetActive(active: false);
		background.enabled = true;
		lockIcon.SetActive(active: false);
	}

	public void SetLocked()
	{
		ContainedNode = null;
		itemIcon.SetActive(active: false);
		background.enabled = false;
		lockIcon.SetActive(active: true);
	}

	private void OnEnable()
	{
		itemButton.onClick.AddListener(AtMainClick);
	}

	private void OnDisable()
	{
		itemButton.onClick.RemoveAllListeners();
	}

	private void AtMainClick()
	{
		if (ContainedNode != null)
		{
			this.OnItemClick?.Invoke(this);
		}
	}
}
