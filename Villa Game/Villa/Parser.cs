using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Parser : MonoBehaviour
{
	public const string FALSE = "FALSE";

	public const string TRUE = "TRUE";

	public static List<List<string>> Split(string csv)
	{
		List<List<string>> list = new List<List<string>>();
		string[] array = csv.Split(new string[1] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
		for (int i = 0; i < array.Length; i++)
		{
			List<string> item = SplitLine(array[i]);
			list.Add(item);
		}
		return list;
	}

	public static string ClearExpression(string expression)
	{
		return expression.Trim('"', '"', '\n');
	}

	public static bool IsEmpty(string expression)
	{
		if (!string.IsNullOrEmpty(expression) && !(expression == " "))
		{
			return expression == "\r";
		}
		return true;
	}

	public static RemoteVersion ParseVersion(string firstCell)
	{
		string[] array = firstCell.Split('|');
		string block = array[0].Split(':')[1];
		string version = array[1].Split(':')[1];
		return new RemoteVersion(block, version);
	}

	public static bool ParseBool(string expression)
	{
		if (IsEmpty(expression))
		{
			return false;
		}
		return expression == "TRUE";
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
		if (!float.TryParse(expression, out var result2))
		{
			Debug.LogError("Parse Exception! " + expression + " is not a float");
			return defValue;
		}
		return result2;
	}

	public static int ParseInt(string expression, int defValue = 0)
	{
		if (IsEmpty(expression))
		{
			return defValue;
		}
		if (!int.TryParse(expression, out var result))
		{
			Debug.LogError("Parse Exception! " + expression + " is not a int");
			return defValue;
		}
		return result;
	}

	public static T ParseEnum<T>(string expression, T defValue) where T : struct
	{
		if (IsEmpty(expression))
		{
			return defValue;
		}
		if (!Enum.TryParse<T>(expression, out var result))
		{
			Debug.LogError($"Parse Exception! {expression} is not a Enum. Used {defValue}");
			return defValue;
		}
		return result;
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

	private static List<string> SplitLine(string line)
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

	private static string TakeNextExpression(string line, ref int position, ref StringBuilder sb)
	{
		sb.Clear();
		int arraysOpen = 0;
		int modulesOpen = 0;
		bool stringFlag = false;
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
			CheckStringSym(c);
			if (c != '"')
			{
				sb.Append(c);
			}
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
		void CheckStringSym(char sym)
		{
			if (sym == '"')
			{
				stringFlag = !stringFlag;
			}
		}
		bool IsEndOfExpression(char sym)
		{
			if (sym == ',' && arraysOpen == 0 && modulesOpen == 0)
			{
				return !stringFlag;
			}
			return false;
		}
	}
}
