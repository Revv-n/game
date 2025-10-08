using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GreenT.HornyScapes.Lootboxes;
using StripClub.Model;

namespace Merge.Core.Balance;

public static class ParsingUtils
{
	public static class MergePointsParser
	{
		public static ModuleConfigs.MergePoints.MergePointsCreateData[] ParseMergePointsCreateData(string currencyTypeStrings, string unlockTypeStrings, string unlockValueStrings, string chanceStrings, string quantityStrings)
		{
			string[] currencyTypeArray = currencyTypeStrings.Split(',', StringSplitOptions.None);
			string[] unlockTypeArrays = unlockTypeStrings.Split('|', StringSplitOptions.None);
			string[] unlockValueArrays = unlockValueStrings.Split('|', StringSplitOptions.None);
			string[] chanceArray = chanceStrings.Split(',', StringSplitOptions.None);
			string[] quantityArray = quantityStrings.Split(',', StringSplitOptions.None);
			ValidateArrayLengths(currencyTypeArray, unlockTypeArrays, unlockValueArrays, chanceArray, quantityArray);
			return CreateMergePointsDataArray(currencyTypeArray, unlockTypeArrays, unlockValueArrays, chanceArray, quantityArray);
		}

		private static void ValidateArrayLengths(string[] currencyTypeArray, string[] unlockTypeArrays, string[] unlockValueArrays, string[] chanceArray, string[] quantityArray)
		{
			int num = currencyTypeArray.Length;
			if (unlockTypeArrays.Length != num || unlockValueArrays.Length != num || chanceArray.Length != num || quantityArray.Length != num)
			{
				throw new ArgumentException("Количество элементов в массивах не совпадает");
			}
		}

		private static ModuleConfigs.MergePoints.MergePointsCreateData[] CreateMergePointsDataArray(string[] currencyTypeArray, string[] unlockTypeArrays, string[] unlockValueArrays, string[] chanceArray, string[] quantityArray)
		{
			int num = currencyTypeArray.Length;
			ModuleConfigs.MergePoints.MergePointsCreateData[] array = new ModuleConfigs.MergePoints.MergePointsCreateData[num];
			for (int i = 0; i < num; i++)
			{
				CurrencySelector currencySelector = ParseCurrencySelector(currencyTypeArray[i].Trim());
				int pointsChance = int.Parse(chanceArray[i].Trim());
				int pointsQty = int.Parse(quantityArray[i].Trim());
				ParseLockers(unlockTypeArrays[i], unlockValueArrays[i], out var lockerTypes, out var lockerValues);
				array[i] = new ModuleConfigs.MergePoints.MergePointsCreateData(currencySelector, pointsChance, pointsQty, lockerTypes, lockerValues);
			}
			return array;
		}

		private static CurrencySelector ParseCurrencySelector(string currencyTypeString)
		{
			return (SelectorTools.CreateSelector(currencyTypeString) as CurrencySelector) ?? throw new InvalidOperationException("Критическая ошибка: тип " + currencyTypeString + " не является CurrencySelector");
		}

		private static void ParseLockers(string unlockTypeString, string unlockValueString, out UnlockType[] lockerTypes, out string[] lockerValues)
		{
			string[] array = unlockTypeString.Split(',', StringSplitOptions.None);
			string[] array2 = unlockValueString.Split(',', StringSplitOptions.None);
			if (array.Length != array2.Length)
			{
				throw new ArgumentException("Количество элементов в типах и значениях блокировок не совпадает");
			}
			lockerTypes = new UnlockType[array.Length];
			lockerValues = new string[array2.Length];
			for (int i = 0; i < array.Length; i++)
			{
				string key = array[i].Trim();
				lockerTypes[i] = SelectorTools.ParseUnlockType(key);
				lockerValues[i] = array2[i].Trim();
			}
		}
	}

	public const string FALSE = "false";

	public const string TRUE = "true";

	private static bool parseFloatsWithDots = true;

	public static List<string> SplitLine(string line)
	{
		int position = 0;
		StringBuilder sb = new StringBuilder();
		List<string> list = new List<string>();
		while (position < line.Length)
		{
			list.Add(TakeNextExpression(line, ref position, ref sb));
		}
		if (line[line.Length - 1] == ',')
		{
			list.Add("");
		}
		return list;
	}

	public static string TakeNextExpression(string line, ref int position, ref StringBuilder sb)
	{
		sb.Clear();
		int arraysOpen = 0;
		int modulesOpen = 0;
		for (int i = position; i < line.Length; i++)
		{
			char c = line[i];
			if (IsEndOfExpression(c))
			{
				position = i + 1;
				return sb.ToString();
			}
			CheckArraySym(c);
			CheckModuleSym(c);
			sb.Append(c);
		}
		position = line.Length + 1;
		return sb.ToString();
		void CheckArraySym(char sym)
		{
			if (sym == '[')
			{
				arraysOpen++;
			}
			if (sym == ']')
			{
				arraysOpen--;
			}
		}
		void CheckModuleSym(char sym)
		{
			if (sym == '{')
			{
				modulesOpen++;
			}
			if (sym == '}')
			{
				modulesOpen--;
			}
		}
		bool IsEndOfExpression(char sym)
		{
			if (sym == ',' && arraysOpen == 0)
			{
				return modulesOpen == 0;
			}
			return false;
		}
	}

	public static int[] ParseIntWeightArray(string expression)
	{
		List<int> list = new List<int>();
		if (IsEmpty(expression))
		{
			return Array.Empty<int>();
		}
		expression = expression.Replace("\"", "");
		list.AddRange(expression.Split(";", StringSplitOptions.None).Select(int.Parse));
		return list.ToArray();
	}

	public static RemoteVersion ParseVersion(string firstLine)
	{
		string version = "1";
		return new RemoteVersion("1", version);
	}

	public static bool IsEmpty(string expression)
	{
		switch (expression)
		{
		default:
			return expression == "\r";
		case null:
		case "":
		case " ":
			return true;
		}
	}

	public static string ClearString(string expression)
	{
		return expression.Trim('[', ']');
	}

	public static bool ParseBool(string expression)
	{
		if (IsEmpty(expression))
		{
			return false;
		}
		return expression == "true";
	}

	public static float ParseFloat(string expression, float defValue = 0f)
	{
		if (IsEmpty(expression))
		{
			return defValue;
		}
		if (float.TryParse(expression, out var result))
		{
			return result;
		}
		expression = expression.Replace('.', ',');
		return float.Parse(expression);
	}

	public static int ParseInt(string expression, int defValue = 0)
	{
		if (IsEmpty(expression))
		{
			return defValue;
		}
		return int.Parse(expression);
	}

	public static T ParseEnum<T>(string expression, T defValue)
	{
		if (IsEmpty(expression))
		{
			return defValue;
		}
		return (T)Enum.Parse(typeof(T), expression);
	}

	public static TimeSpan ParseTime(string expression, TimeSpan defValue)
	{
		if (IsEmpty(expression))
		{
			return defValue;
		}
		TimeSpan result = default(TimeSpan);
		string text = "";
		for (int i = 0; i < expression.Length; i++)
		{
			if (char.IsDigit(expression[i]))
			{
				text += expression[i];
				continue;
			}
			if (expression[i] == 'd')
			{
				result += TimeSpan.FromDays(int.Parse(text));
			}
			else if (expression[i] == 'h')
			{
				result += TimeSpan.FromHours(int.Parse(text));
			}
			else if (expression[i] == 'm')
			{
				result += TimeSpan.FromMinutes(int.Parse(text));
			}
			else
			{
				if (expression[i] != 's')
				{
					throw new Exception("Unexcepted time sym");
				}
				result += TimeSpan.FromSeconds(int.Parse(text));
			}
			text = "";
		}
		return result;
	}

	public static GIData ParseGIData(string expression)
	{
		GIData result = GIData.GetEmpty();
		if (IsEmpty(expression))
		{
			return result;
		}
		expression = expression.Replace("\r", "").Replace("\"", "");
		if (!expression.Contains('{') && !expression.Contains('}'))
		{
			result = new GIData(GIKey.Parse(expression));
			return result;
		}
		int num = expression.IndexOf('{');
		GIKey key = GIKey.Parse(expression.Substring(0, num));
		result = new GIData(key);
		int length = expression.Length - num - 2;
		string[] array = expression.Substring(num + 1, length).Split('|', StringSplitOptions.None);
		List<string> overrideDataExpressions = new List<string>();
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].Contains('#') && overrideDataExpressions.Count > 0)
			{
				WriteStackedDataExpressions();
			}
			overrideDataExpressions.Add(array[i]);
		}
		if (overrideDataExpressions.Count > 0)
		{
			WriteStackedDataExpressions();
		}
		return result;
		void WriteStackedDataExpressions()
		{
			result.Modules.Add(ParseOverrideModuleData(overrideDataExpressions));
			overrideDataExpressions.Clear();
		}
	}

	public static GIKey ParseGIKey(string expression)
	{
		GIKey result = default(GIKey);
		if (IsEmpty(expression))
		{
			return result;
		}
		expression = expression.Replace("\r", "").Replace("\"", "");
		return GIKey.Parse(expression);
	}

	public static ModuleDatas.Base ParseOverrideModuleData(List<string> list)
	{
		if (list[0].Contains("#Locked"))
		{
			return ParseLocked();
		}
		if (list[0].Contains("#Bubble"))
		{
			return ParseBubble();
		}
		if (list[0].Contains("#ClickSpawn"))
		{
			return ParseClickSpawn();
		}
		return null;
		List<GIData> GetGiList(string field)
		{
			string text = list.FirstOrDefault((string x) => x.Contains(field));
			if (text != null)
			{
				return ParseGIDataList(text.Substring(field.Length));
			}
			return null;
		}
		int GetIntValue(string field, int defValue)
		{
			string text3 = list.FirstOrDefault((string x) => x.Contains(field));
			if (text3 != null)
			{
				return int.Parse(text3.Substring(field.Length));
			}
			return defValue;
		}
		string GetStringValue(string field, string defValue)
		{
			string text2 = list.FirstOrDefault((string x) => x.Contains(field));
			if (text2 != null)
			{
				return text2.Substring(field.Length);
			}
			return defValue;
		}
		ModuleDatas.Bubble ParseBubble()
		{
			int intValue = GetIntValue("Price:", 5);
			int intValue2 = GetIntValue("Time:", 60);
			string stringValue = GetStringValue("InBubble:", "Coin3");
			return new ModuleDatas.Bubble
			{
				OpenPrice = intValue,
				MainTimer = new RefTimer(intValue2, TimeMaster.Default),
				Rest = new GIData(GIKey.Parse(stringValue))
			};
		}
		ModuleDatas.ClickSpawn ParseClickSpawn()
		{
			List<GIData> giList = GetGiList("DropQueue:");
			int amount = giList?.Count ?? GetIntValue("Amount:", 0);
			return new ModuleDatas.ClickSpawn
			{
				DropQueue = giList,
				Amount = amount
			};
		}
		ModuleDatas.Locked ParseLocked()
		{
			int intValue3 = GetIntValue("Strength:", 1);
			int intValue4 = GetIntValue("BlockVisible:", 0);
			int intValue5 = GetIntValue("Price:", 5);
			return new ModuleDatas.Locked
			{
				Strength = intValue3,
				BlocksMerge = (intValue4 > 0),
				UnlockPrice = intValue5
			};
		}
	}

	public static List<GIData> ParseGIDataList(string expression)
	{
		List<GIData> list = new List<GIData>();
		if (IsEmpty(expression))
		{
			return list;
		}
		expression = expression.Replace("\"", "").Replace("}", "");
		if (expression[0] != '[')
		{
			list.Add(ParseGIData(expression));
			return list;
		}
		string text = expression.Substring(1, expression.Length - 2);
		StringBuilder sb = new StringBuilder();
		int position = 0;
		while (position < text.Length)
		{
			string expression2 = TakeNextExpression(text, ref position, ref sb);
			list.Add(ParseGIData(expression2));
		}
		return list;
	}

	public static List<WeightNode<GIData>> ParseGIDataWeightList(string expression)
	{
		List<WeightNode<GIData>> list = new List<WeightNode<GIData>>();
		if (IsEmpty(expression))
		{
			return list;
		}
		expression = expression.Replace("\"", "");
		int position = 0;
		StringBuilder sb = new StringBuilder();
		if (expression[0] != '[')
		{
			list.Add(GetWeightNode(expression));
			return list;
		}
		string text = expression.Substring(1, expression.Length - 2);
		while (position < text.Length)
		{
			string nodeStr2 = TakeNextExpression(text, ref position, ref sb);
			list.Add(GetWeightNode(nodeStr2));
		}
		return list;
		static WeightNode<GIData> GetWeightNode(string nodeStr)
		{
			float weight = 1f;
			string text2 = "";
			bool flag = false;
			for (int num = nodeStr.Length - 1; num >= 0; num--)
			{
				if (!char.IsDigit(nodeStr[num]) && nodeStr[num] != '.')
				{
					if (nodeStr[num] == ':')
					{
						weight = float.Parse(text2);
						flag = true;
					}
					else
					{
						text2 = "";
					}
					break;
				}
				text2 = nodeStr[num] + text2;
			}
			return new WeightNode<GIData>(ParseGIData(flag ? nodeStr.Substring(0, nodeStr.Length - text2.Length - 1) : nodeStr), weight);
		}
	}
}
