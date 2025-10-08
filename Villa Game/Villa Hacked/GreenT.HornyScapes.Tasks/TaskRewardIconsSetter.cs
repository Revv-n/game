using System.Collections.Generic;
using GreenT.Types;
using StripClub.Model;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.Tasks;

public class TaskRewardIconsSetter : MonoBehaviour
{
	[SerializeField]
	private List<GameObject> Stars;

	[SerializeField]
	private List<Image> StarImages;

	[SerializeField]
	private List<GameObject> Jewels;

	[SerializeField]
	private List<GameObject> Contracts;

	[SerializeField]
	private TMP_Text CurrencyCount;

	private GameSettings gameSettings;

	[Inject]
	private void InnerInit(GameSettings gameSettings)
	{
		this.gameSettings = gameSettings;
	}

	public void Set(CurrencyLinkedContent linkedContent)
	{
		switch (linkedContent.Currency)
		{
		case CurrencyType.Star:
			SetRewardsIcons(linkedContent, StarImages, Stars);
			SetRewardsCount(linkedContent);
			break;
		case CurrencyType.Jewel:
			SetRewardsIcons(linkedContent, null, Jewels, isSettable: false);
			SetRewardsCount(linkedContent);
			break;
		case CurrencyType.Contracts:
			SetRewardsIcons(linkedContent, null, Contracts, isSettable: false);
			SetRewardsCount(linkedContent);
			break;
		}
	}

	private void SetRewardsIcons(CurrencyLinkedContent linkedContent, List<Image> images, List<GameObject> showObjects, bool isSettable = true)
	{
		if (isSettable)
		{
			foreach (Image image in images)
			{
				image.sprite = gameSettings.CurrencySettings[linkedContent.Currency, default(CompositeIdentificator)].Sprite;
			}
		}
		HideAll();
		showObjects[0].SetActive(value: true);
	}

	private void SetRewardsCount(CurrencyLinkedContent linkedContent)
	{
		CurrencyCount.text = $"{linkedContent.Quantity}";
	}

	private void HideAll()
	{
		foreach (GameObject star in Stars)
		{
			star.SetActive(value: false);
		}
		foreach (GameObject jewel in Jewels)
		{
			jewel.SetActive(value: false);
		}
		foreach (GameObject contract in Contracts)
		{
			contract.SetActive(value: false);
		}
	}
}
