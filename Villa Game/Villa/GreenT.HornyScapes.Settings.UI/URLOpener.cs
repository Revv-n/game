using GreenT.HornyScapes.Constants;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.Settings.UI;

public class URLOpener : MonoBehaviour
{
	[SerializeField]
	private Button button;

	[Tooltip("Ключ из конфига констант")]
	[SerializeField]
	private string constantKey = string.Empty;

	private IConstants<string> constants;

	[Inject]
	public void Init(IConstants<string> constants)
	{
		this.constants = constants;
	}

	public void Set(string constantKey)
	{
		this.constantKey = constantKey;
	}

	public void OpenUrl()
	{
		Application.OpenURL(constants[constantKey]);
	}
}
