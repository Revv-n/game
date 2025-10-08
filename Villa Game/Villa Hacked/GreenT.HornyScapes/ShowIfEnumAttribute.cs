using UnityEngine;

namespace GreenT.HornyScapes;

public class ShowIfEnumAttribute : PropertyAttribute
{
	public string FieldName;

	public MultiDropType Value;

	public ShowIfEnumAttribute(string enumFieldName, MultiDropType enumValue)
	{
		FieldName = enumFieldName;
		Value = enumValue;
	}
}
