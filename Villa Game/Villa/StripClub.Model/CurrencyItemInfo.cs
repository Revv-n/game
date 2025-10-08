using UnityEngine;

namespace StripClub.Model;

[CreateAssetMenu(fileName = "Currency", menuName = "StripClub/Items/Currency", order = 0)]
public class CurrencyItemInfo : NamedScriptableItemInfo
{
	private const string prefix = "content.items.currency.";

	[SerializeField]
	protected Sprite smallIcon;

	[SerializeField]
	protected CurrencyType currency;

	public Sprite SmallIcon => smallIcon;

	public CurrencyType CurrencyType => currency;

	protected override string GetKey()
	{
		return "content.items.currency." + key;
	}
}
