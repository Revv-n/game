using UnityEngine;

namespace GreenT.HornyScapes;

public abstract class BaseRouletteLotViewFactory<T, V> : AbstractLotViewFactory<T, V> where T : RouletteSummonLot where V : RouletteLotBackgroundView
{
	public override V Create(T lot)
	{
		V val = TryGetView<V>(lot.Source, lot.ViewType);
		V val2 = container.InstantiatePrefabForComponent<V>((Object)val, viewContainer);
		val2.Set(lot);
		return val2;
	}
}
