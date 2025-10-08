using UnityEngine;
using UnityEngine.UI;

namespace GreenT;

[RequireComponent(typeof(Button))]
public abstract class BaseButton : MonoBehaviour
{
	[SerializeField]
	protected Button button;

	protected virtual void OnValidate()
	{
		button = GetComponent<Button>();
	}

	protected virtual void AddListener()
	{
		button.onClick.AddListener(OnClick);
	}

	protected abstract void OnClick();

	public virtual void Clear()
	{
		button.onClick.RemoveListener(OnClick);
	}

	protected virtual void OnDestroy()
	{
		Clear();
	}
}
