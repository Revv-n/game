using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace StripClub.Extensions;

public static class SizeUtility
{
	private static Dictionary<float, float> womenFeetSizeCorrespondencesTable = new Dictionary<float, float>
	{
		{ 4f, 33f },
		{ 4.5f, 34f },
		{ 5f, 35f },
		{ 5.5f, 35.5f },
		{ 6f, 36f },
		{ 6.5f, 36.5f },
		{ 7f, 37.5f },
		{ 7.5f, 38f },
		{ 8f, 38.5f },
		{ 8.5f, 39f },
		{ 9f, 39.5f },
		{ 9.5f, 40f },
		{ 10f, 41f },
		{ 10.5f, 42f },
		{ 11f, 43f }
	};

	public static RelativeSize GetRelativeHeight(float heightInFeets)
	{
		if (heightInFeets < 5.25f)
		{
			return RelativeSize.Small;
		}
		if (heightInFeets > 5.6f)
		{
			return RelativeSize.Large;
		}
		return RelativeSize.Medium;
	}

	public static RelativeSize GetBreastSize(string breastSize)
	{
		Regex regex = new Regex("[AB]");
		Regex regex2 = new Regex("C");
		Regex regex3 = new Regex("[DEF]");
		if (regex.IsMatch(breastSize))
		{
			return RelativeSize.Small;
		}
		if (regex3.IsMatch(breastSize))
		{
			return RelativeSize.Large;
		}
		if (regex2.IsMatch(breastSize))
		{
			return RelativeSize.Medium;
		}
		return RelativeSize.Undefined;
	}

	public static RelativeSize GetFeetSize(float feetSize)
	{
		if (feetSize < 7f)
		{
			return RelativeSize.Small;
		}
		if (feetSize > 8f)
		{
			return RelativeSize.Large;
		}
		return RelativeSize.Medium;
	}

	public static float FeetsToCentimeters(float feets)
	{
		return feets * 30.48f;
	}

	public static string FeetsToString(float feets)
	{
		StringBuilder stringBuilder = new StringBuilder(((int)feets).ToString()).Append('’');
		float num = 12f * (feets % 1f);
		if (num != 0f)
		{
			stringBuilder.Append(num.ToString("F0")).Append('”');
		}
		return stringBuilder.ToString();
	}

	public static float FeetSizeWomenUStoEU(float feetSizeUS)
	{
		return womenFeetSizeCorrespondencesTable[feetSizeUS];
	}
}
