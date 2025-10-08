using System;

namespace GreenT.HornyScapes.Extensions;

public static class EnumExtension
{
	private static Array GetValue<TEnum>() where TEnum : Enum
	{
		return Enum.GetValues(typeof(TEnum));
	}

	public static TEnum[] GetTargetValue<TEnum>() where TEnum : Enum
	{
		return Enum.GetValues(typeof(TEnum)) as TEnum[];
	}

	public static void ForeachEnum<TEnum>(Action<TEnum> action) where TEnum : Enum
	{
		foreach (object item in GetValue<TEnum>())
		{
			action((TEnum)item);
		}
	}
}
