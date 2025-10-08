using TMPro;
using UnityEngine;

namespace StripClub.UI.Shop;

public class DropChanceView : MonoBehaviour, IView
{
	public class Manager : ViewManager<DropChanceView>
	{
	}

	[SerializeField]
	private StatableComponent[] statableComponents;

	[SerializeField]
	private TextMeshProUGUI chance;

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

	public void Display(bool isOn)
	{
		base.gameObject.SetActive(isOn);
	}

	public bool IsActive()
	{
		return base.gameObject.activeSelf;
	}

	public void Set(int rarity, decimal chance)
	{
		StatableComponent[] array = statableComponents;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Set(rarity);
		}
		this.chance.text = chance + "%";
	}
}
