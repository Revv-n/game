namespace GreenT.HornyScapes;

public abstract class EnumConverterM<TFrom, TTo>
{
	public abstract TTo Convert(TFrom from);
}
