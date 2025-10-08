using GreenT.Types;
using StripClub.Model;
using StripClub.UI;
using UnityEngine;
using UnityEngine.UI;

namespace GreenT.HornyScapes;

public sealed class MiniEventFlyingCurrencyView : MonoBehaviour
{
	[SerializeField]
	private CurrencyMaterialAttacher _currencyMaterialAttacher;

	[SerializeField]
	private Image _image;

	public Image Image => _image;

	public void Setup(CurrencyType currencyType, CompositeIdentificator currencyIdentificator)
	{
		_currencyMaterialAttacher.SetView(currencyType, currencyIdentificator);
	}
}
