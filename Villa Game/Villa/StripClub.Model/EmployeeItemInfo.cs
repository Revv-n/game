using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace StripClub.Model;

[CreateAssetMenu(fileName = "Employee", menuName = "StripClub/Items/Employee item", order = 0)]
public class EmployeeItemInfo : NamedScriptableItemInfo
{
	[Serializable]
	public class ItemProperties
	{
		[SerializeField]
		private int multiplierValue = 2;

		[field: SerializeField]
		public int PlaceNumber { get; private set; }

		[field: SerializeField]
		public int EmployeeLevel { get; private set; }

		public int MultiplierValue => multiplierValue;

		public void SetPlaceNumber(int number)
		{
			if (PlaceNumber != number)
			{
				PlaceNumber = number;
			}
		}
	}

	private const string prefix = "content.items.employee.";

	[field: SerializeField]
	public List<ItemProperties> Properties { get; private set; }

	protected override string GetKey()
	{
		return "content.items.employee." + key;
	}

	public ItemProperties GetPropertyByPlaceNumber(int number)
	{
		int i = 0;
		try
		{
			for (; Properties[i].PlaceNumber != number; i++)
			{
			}
		}
		catch
		{
			throw new ArgumentNullException("[ScriptableObject] " + base.name + ":There is no properties for place with number" + number);
		}
		return Properties[i];
	}

	public override void OnValidate()
	{
		base.OnValidate();
	}

	[Conditional("UNITY_EDITOR")]
	private void AutoFill()
	{
	}

	[Conditional("UNITY_EDITOR")]
	private void FillPlaceNumber()
	{
		for (int i = 0; i < Properties.Count; i++)
		{
			int placeNumber = i + 1;
			Properties[i].SetPlaceNumber(placeNumber);
		}
	}
}
