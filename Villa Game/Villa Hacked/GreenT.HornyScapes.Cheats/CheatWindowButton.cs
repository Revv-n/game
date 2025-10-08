using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.Cheats;

public class CheatWindowButton : MonoBehaviour
{
	[SerializeField]
	private Button _windowButton;

	[SerializeField]
	private Image _windowButtonImage;

	[SerializeField]
	private Sprite _close;

	[SerializeField]
	private Sprite _open;

	[SerializeField]
	private CheatWindow _cheatWindow;

	[Inject]
	private UIClickSuppressor _cickSuppressor;

	private void Start()
	{
		_windowButton.onClick.AddListener(delegate
		{
			ShowWindow(!_cheatWindow.IsOpened);
		});
		_windowButton.onClick.AddListener(delegate
		{
			_cickSuppressor.SuppressClick();
		});
	}

	public void SetSprite(bool show)
	{
		_windowButtonImage.sprite = (show ? _close : _open);
	}

	private void ShowWindow(bool state)
	{
		SetSprite(state);
		_cheatWindow.ChangeVisibility(state);
	}

	private void OnDestroy()
	{
		_windowButton.onClick.RemoveAllListeners();
	}
}
