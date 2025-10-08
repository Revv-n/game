using GreenT.HornyScapes.Events.Content;
using GreenT.Types;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.Events;

public class ContentTypeChanger : MonoBehaviour
{
	[SerializeField]
	private Button closeBtn;

	[SerializeField]
	private ContentType setType;

	[Inject]
	private ContentSelectorGroup contentSelectorGroup;

	protected virtual void OnValidate()
	{
		TryGetComponent<Button>(out closeBtn);
	}

	protected virtual void Awake()
	{
		closeBtn.onClick.AddListener(ChangeContentType);
	}

	private void ChangeContentType()
	{
		contentSelectorGroup.Select(setType);
	}

	protected virtual void OnDestroy()
	{
		closeBtn.onClick.RemoveListener(ChangeContentType);
	}
}
