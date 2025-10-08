using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.Sounds;

[RequireComponent(typeof(Toggle))]
public abstract class BaseToggleMute : MonoBehaviour
{
	public SoundSettingsSO GlobalSettings;

	public Toggle Toggle;

	protected IAudioPlayer audioPlayer;

	private void OnValidate()
	{
		if (!Toggle)
		{
			Toggle = GetComponent<Toggle>();
		}
	}

	[Inject]
	protected virtual void InnerInit(IAudioPlayer audioPlayer)
	{
		this.audioPlayer = audioPlayer;
		Toggle.onValueChanged.AddListener(ChangeState);
	}

	protected virtual void ChangeState(bool state)
	{
	}

	private void OnDestroy()
	{
		Toggle.onValueChanged.RemoveAllListeners();
	}
}
