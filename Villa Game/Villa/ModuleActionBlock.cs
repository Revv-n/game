using StripClub.UI.Shop;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ModuleActionBlock : MonoBehaviour
{
	public TextMeshProUGUI MainLabel;

	public TextMeshProUGUI ButtonLabel;

	public Image CurrencyIcon;

	public Button Button;

	public PriceWithFreeView StateView;

	public ModuleActionBlock SetMainLabelText(string text)
	{
		MainLabel.text = text;
		return this;
	}

	public ModuleActionBlock SetButtonLabelText(string text)
	{
		ButtonLabel.text = text;
		return this;
	}

	public ModuleActionBlock SetCurrencyIcon(Sprite icon)
	{
		CurrencyIcon.sprite = icon;
		return this;
	}
}
