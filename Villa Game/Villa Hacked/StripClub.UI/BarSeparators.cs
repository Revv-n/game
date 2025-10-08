using System.Collections.Generic;
using UnityEngine;

namespace StripClub.UI;

public class BarSeparators : MonoBehaviour
{
	[SerializeField]
	private GameObject separatorPrefab;

	[SerializeField]
	private Transform separatorContainer;

	private List<GameObject> separators = new List<GameObject>();

	public void Init(int count)
	{
		int i;
		for (i = 0; i != count; i++)
		{
			if (i < separators.Count)
			{
				separators[i].SetActive(value: true);
				continue;
			}
			GameObject item = Object.Instantiate(separatorPrefab, separatorContainer);
			separators.Add(item);
		}
		for (; i < separators.Count; i++)
		{
			separators[i].SetActive(value: false);
		}
	}
}
