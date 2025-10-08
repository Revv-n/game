using TMPro;
using UnityEngine;

namespace GreenT.HornyScapes.MergeStore;

public class SaleView : MonoBehaviour
{
	[SerializeField]
	private TMP_Text _text;

	public void Set(int value)
	{
		_text.text = $"-{value}%";
	}
}
