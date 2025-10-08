using GreenT.Types;
using Merge;
using StripClub.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StripClub.Model.Shop.UI;

public class ResourceDropCardBigView : MonoView
{
	public class Manager : ViewManager<ResourceDropCardBigView>
	{
	}

	[SerializeField]
	private Image icon;

	[SerializeField]
	private TMP_Text count;

	[SerializeField]
	private Image background;

	[SerializeField]
	private Image border;

	[SerializeField]
	private Sprite borderSprite;

	public CurrencyType SourceCurrency { get; private set; }

	public CompositeIdentificator SourceIdentificator { get; private set; }

	public override bool IsActive()
	{
		return base.gameObject.activeInHierarchy;
	}

	public void SetResource(Sprite sprite, int? quantity, CurrencyType source, CompositeIdentificator identificator)
	{
		SourceCurrency = source;
		SourceIdentificator = identificator;
		SetQuantity(quantity);
		icon.sprite = sprite;
		icon.preserveAspect = true;
		background.SetActive(active: true);
		border.sprite = borderSprite;
	}

	protected void SetQuantity(int? quantity)
	{
		if (quantity.HasValue)
		{
			count.SetActive(active: true);
			count.text = quantity.Value.ToString();
		}
		else
		{
			count.SetActive(active: false);
			count.text = string.Empty;
		}
	}
}
