using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.UI;

public class IndependentSupportButton : MonoBehaviour
{
	[SerializeField]
	private Button button;

	private string redirectUrl;

	[Inject]
	public void Init([Inject(Id = "SupportUrl")] string redirectUrl)
	{
		this.redirectUrl = redirectUrl;
	}

	private void Awake()
	{
		button.onClick.AddListener(OpenUrl);
	}

	public void OpenUrl()
	{
		Application.OpenURL(redirectUrl);
	}
}
