using GreenT.HornyScapes;
using GreenT.Types;
using StripClub.Model;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace StripClub.UI;

[RequireComponent(typeof(Image))]
public class CurrencySpriteAttacher : MonoBehaviour
{
	[SerializeField]
	protected Image target;

	[SerializeField]
	protected CurrencyType currency;

	[Inject]
	protected GreenT.HornyScapes.GameSettings gameSettings;

	protected virtual void OnValidate()
	{
		if (!target)
		{
			target = GetComponent<Image>();
		}
	}

	protected virtual void Start()
	{
		SetView();
	}

	public virtual void SetView()
	{
		target.sprite = gameSettings.CurrencySettings[currency, default(CompositeIdentificator)].Sprite;
	}

	public virtual void SetView(CompositeIdentificator compositeIdentificator = default(CompositeIdentificator))
	{
		target.sprite = gameSettings.CurrencySettings[currency, compositeIdentificator].Sprite;
	}

	public void ChangeCurrency(CurrencyType changeCurrency)
	{
		currency = changeCurrency;
		SetView();
	}
}
