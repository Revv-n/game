using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StripClub.Registration;

[RequireComponent(typeof(Image))]
public class PasswordHider : MonoBehaviour
{
	[SerializeField]
	private Image _image;

	[Space]
	[SerializeField]
	private TMP_InputField _passwordField;

	[SerializeField]
	private Sprite _hidenImage;

	[SerializeField]
	private Sprite _showedImage;

	private bool _hiden = true;

	private void OnValidate()
	{
		_image = GetComponent<Image>();
	}

	private void Start()
	{
		SetFieldType(_hiden);
	}

	public void ChangeVisibility(bool value)
	{
		_hiden = value;
		SetFieldType(_hiden);
	}

	public void ChangeVisibility()
	{
		_hiden = !_hiden;
		SetFieldType(_hiden);
	}

	private void SetFieldType(bool value)
	{
		if (value)
		{
			_image.sprite = _hidenImage;
			_passwordField.contentType = TMP_InputField.ContentType.Password;
		}
		else
		{
			_image.sprite = _showedImage;
			_passwordField.contentType = TMP_InputField.ContentType.Standard;
		}
		_passwordField.ForceLabelUpdate();
	}
}
