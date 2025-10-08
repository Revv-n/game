using UnityEngine;

namespace StripClub.Registration;

public class ReceiveCheckBox : MonoBehaviour
{
	[SerializeField]
	private GameObject _checkIcon;

	[SerializeField]
	private bool _receive;

	public bool Receive => _receive;

	private void Start()
	{
		_checkIcon.SetActive(_receive);
	}

	public void SetReceiving(bool value)
	{
		_receive = value;
		_checkIcon.SetActive(value);
	}

	public void SetReceiving()
	{
		_receive = !_receive;
		_checkIcon.SetActive(_receive);
	}
}
