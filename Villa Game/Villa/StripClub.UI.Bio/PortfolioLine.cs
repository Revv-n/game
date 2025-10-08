using UnityEngine;
using UnityEngine.UI;

namespace StripClub.UI.Bio;

public class PortfolioLine : MonoBehaviour
{
	[SerializeField]
	private Text content;

	public void Set(string line)
	{
		content.text = line;
	}
}
