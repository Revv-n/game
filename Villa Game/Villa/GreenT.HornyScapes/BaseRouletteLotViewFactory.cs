namespace GreenT.HornyScapes;

public abstract class BaseRouletteLotViewFactory<T, V> : AbstractLotViewFactory<T, V> where T : RouletteSummonLot where V : RouletteLotBackgroundView
{
	public override V Create(T lot)
	{
		V prefab = TryGetView<V>(lot.Source, lot.ViewType);
		V val = container.InstantiatePrefabForComponent<V>(prefab, viewContainer);
		val.Set(lot);
		return val;
	}
}
