using Merge;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GreenT.HornyScapes.MergeCore.UI;

public class StackNextItemInfo : MonoBehaviour
{
	[SerializeField]
	private Image icon;

	[SerializeField]
	private GameObject counter;

	[SerializeField]
	private TMP_Text countText;

	[SerializeField]
	private Button infoButton;

	[SerializeField]
	private Color32 patrialItemsColor;

	[SerializeField]
	private Color32 fullItemsColor;

	private GIKey giKey;

	public void SetInfo(GIKey giKey)
	{
		this.giKey = giKey;
		infoButton.gameObject.SetActive(value: true);
		infoButton.onClick.AddListener(AtInfoClick);
	}

	public void SetIcon(Sprite itemIcon)
	{
		icon.sprite = itemIcon;
	}

	public void SetCount(int count, int max)
	{
		counter.SetActive(value: true);
		countText.text = $"{count}/{max}";
		countText.color = ((count == max) ? fullItemsColor : patrialItemsColor);
	}

	private void AtInfoClick()
	{
		Controller<CollectionController>.Instance.ShowWindow(giKey);
	}
}
