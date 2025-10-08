using System.Numerics;
using GreenT.Types;
using StripClub.Model;
using UnityEngine;

namespace StripClub.UI;

public abstract class ResourceView : MonoBehaviour, IResourceView
{
	public CurrencyType Resource { get; private set; }

	public BigInteger Count { get; protected set; }

	public CompositeIdentificator Identificator { get; protected set; }

	public virtual void Set(CurrencyType currency, BigInteger count, CompositeIdentificator compositeIdentificator)
	{
		Resource = currency;
		Count = count;
		Identificator = compositeIdentificator;
	}
}
