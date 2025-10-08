using UnityEngine;

namespace GreenT.HornyScapes.Presents.Models;

[CreateAssetMenu(fileName = "PresentBundleData", menuName = "GreenT/HornyScapes/Present/PresentBundleData")]
public sealed class PresentBundleData : ScriptableObject
{
	private const string DefaultCurrencyKey = "ui.hint.present_";

	private const string BundleName = "presents";

	[SerializeField]
	private int _presentId;

	[SerializeField]
	private Sprite _sprite;

	[SerializeField]
	private string _currencyKey;

	public int PresentId => _presentId;

	public Sprite Sprite => _sprite;

	public string CurrencyKey => _currencyKey;
}
