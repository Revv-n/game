using System;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Merge.MotionDesign.TweenPanels.Utils;

public class ReflectionEaseDropdown : ReflectionViewElement
{
	private Dropdown dropdown;

	protected override void OnInit()
	{
		dropdown = GetComponentInChildren<Dropdown>();
		Array values = Enum.GetValues(SafeReflections.GetFieldType(target, property));
		List<string> list = new List<string>();
		dropdown.options.Clear();
		foreach (object item in values)
		{
			list.Add(item.ToString());
		}
		dropdown.AddOptions(list);
		dropdown.value = SafeReflections.GetFieldValue<int>(target, property);
		dropdown.onValueChanged.AddListener(SetValueInTarget);
	}

	private void SetValueInTarget(int value)
	{
		SafeReflections.SetFieldValue(target, property, value);
	}
}
