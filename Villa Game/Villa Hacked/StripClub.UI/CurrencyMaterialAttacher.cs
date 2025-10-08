using GreenT.HornyScapes;
using GreenT.Types;
using StripClub.Model;
using UnityEngine;
using Zenject;

namespace StripClub.UI;

public class CurrencyMaterialAttacher : MonoBehaviour
{
	[SerializeField]
	protected Material target;

	[SerializeField]
	protected CurrencyType currency;

	[Inject]
	protected GreenT.HornyScapes.GameSettings gameSettings;

	protected virtual void Start()
	{
		if (currency != CurrencyType.None)
		{
			SetView();
		}
	}

	public virtual void SetView()
	{
		target.mainTexture = gameSettings.CurrencySettings[currency, default(CompositeIdentificator)].Sprite.texture;
	}

	public void SetView(CurrencyType currency, CompositeIdentificator currencyIdentificator)
	{
		target.mainTexture = gameSettings.CurrencySettings[currency, currencyIdentificator].Sprite.texture;
	}
}
