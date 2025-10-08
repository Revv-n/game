using System;
using UnityEngine;

namespace StripClub.Model;

[Serializable]
public class CurrencySettings
{
	[SerializeField]
	private Sprite sprite;

	[SerializeField]
	private Sprite alternativeSprite;

	[SerializeField]
	private string localizationKey;

	[SerializeField]
	private int initialValue;

	[SerializeField]
	private AudioClip getSound;

	public Sprite Sprite => sprite;

	public Sprite AlternativeSprite => alternativeSprite;

	public string Key => localizationKey;

	public int InitialValue => initialValue;

	public AudioClip GetSound => getSound;

	public void SetSprite(Sprite sprite)
	{
		this.sprite = sprite;
	}

	public void SetAlternativeSprite(Sprite sprite)
	{
		alternativeSprite = sprite;
	}

	public void SetLocalization(string localizationKey)
	{
		this.localizationKey = localizationKey;
	}
}
