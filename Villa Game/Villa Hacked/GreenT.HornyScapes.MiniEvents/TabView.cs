using StripClub.UI;
using UnityEngine;
using UnityEngine.UI;

namespace GreenT.HornyScapes.MiniEvents;

public abstract class TabView<TModel> : MonoView<TModel> where TModel : IIdentifiable
{
	[SerializeField]
	private Button _interactButton;

	protected MiniEventWindowView _miniEventWindowView;

	private void OnEnable()
	{
		_interactButton.onClick.AddListener(OnInteractButtonClick);
	}

	private void OnDisable()
	{
		_interactButton.onClick.RemoveListener(OnInteractButtonClick);
	}

	public void Init(MiniEventWindowView miniEventWindowView)
	{
		_miniEventWindowView = miniEventWindowView;
	}

	protected abstract void OnInteractButtonClick();
}
