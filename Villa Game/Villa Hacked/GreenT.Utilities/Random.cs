using UnityEngine;

namespace GreenT.Utilities;

public static class Random
{
	public static int GetRandomElementByLevel(int[] levels)
	{
		float num = CalcWeightConstant(levels);
		float[] array = new float[levels.Length];
		float num2 = 0f;
		int i;
		for (i = 0; i != levels.Length; i++)
		{
			array[i] = num / (float)levels[i];
			num2 += array[i];
		}
		float num3 = UnityEngine.Random.Range(0f, num2);
		i = 0;
		float num4 = array[0];
		while (i < array.Length - 1 && num4 < num3)
		{
			i++;
			num4 += array[i];
		}
		return i;
	}

	private static float CalcWeightConstant(int[] levels)
	{
		float num = 0f;
		if (levels.Length == 0)
		{
			return 0f;
		}
		for (int i = 0; i != levels.Length; i++)
		{
			num += 1f / (float)levels[i];
		}
		return 1f / num;
	}
}
