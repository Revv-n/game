using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StripClub.Gallery.UI;

public class MediaToggle : Toggle
{
	[SerializeField]
	private Image iconImage;

	[SerializeField]
	private Sprite lockIcon;

	[SerializeField]
	private TextMeshProUGUI title;

	private Sprite defaultIcon;

	public Type ContentType { get; private set; }

	public void Init(Type type, Sprite icon, string text, Action<Toggle> clickAction)
	{
		title.text = text;
		defaultIcon = icon;
		iconImage.sprite = icon;
		ContentType = type;
		base.Init(text, clickAction);
	}
}
