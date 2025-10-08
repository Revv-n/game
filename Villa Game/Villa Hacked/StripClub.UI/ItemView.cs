using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StripClub.UI;

public class ItemView : MonoBehaviour, IView
{
	internal class Manager : ViewManager<ItemView>
	{
	}

	public const string countText = "{0}/{1}";

	public const string enoughtItemsCount = "<color=#{2}>{0}/{1}</color>";

	[SerializeField]
	private Image itemIcon;

	[SerializeField]
	private TMP_Text itemCount;

	[SerializeField]
	private Color filledCountTextColor = Color.white;

	public int SiblingIndex
	{
		get
		{
			return base.transform.GetSiblingIndex();
		}
		set
		{
			base.transform.SetSiblingIndex(value);
		}
	}

	public void Set(Sprite itemIcon, int availableCount, int necessaryCount)
	{
		this.itemIcon.sprite = itemIcon;
		itemCount.text = string.Format((availableCount >= necessaryCount) ? "<color=#{2}>{0}/{1}</color>" : "{0}/{1}", availableCount, necessaryCount, ColorUtility.ToHtmlStringRGBA(filledCountTextColor));
	}

	public void Display(bool isOn)
	{
		base.gameObject.SetActive(isOn);
	}

	public bool IsActive()
	{
		return base.gameObject.activeSelf;
	}
}
