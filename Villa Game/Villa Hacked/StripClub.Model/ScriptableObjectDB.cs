using UnityEngine;

namespace StripClub.Model;

public class ScriptableObjectDB : ScriptableObject
{
	[Header("Локализация")]
	[SerializeField]
	public LanguageDictionary localization;
}
