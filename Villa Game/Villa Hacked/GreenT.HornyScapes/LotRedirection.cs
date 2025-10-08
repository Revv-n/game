using GreenT.Types;
using UnityEngine;

namespace GreenT.HornyScapes;

public abstract class LotRedirection : MonoBehaviour
{
	public abstract bool TryStraightRedirect(int bankTabId);

	public abstract bool TryRedirect(CompositeIdentificator currencyIdentificator);
}
