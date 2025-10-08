using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StripClub.UI.Shop;

public class SlotsHeaderView : MonoView
{
	[SerializeField]
	private Image header;

	[SerializeField]
	private TMP_Text title;

	[SerializeField]
	private TMP_Text shadowTitle;

	public void Set(Sprite header, string title, Color titleColor, TMP_ColorGradient titleGradient)
	{
		this.header.sprite = header;
		this.title.text = title;
		shadowTitle.text = title;
		this.title.color = titleColor;
		this.title.colorGradientPreset = titleGradient;
	}
}
