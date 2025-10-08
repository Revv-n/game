using GreenT.HornyScapes.Events.Content;
using GreenT.HornyScapes.MergeCore;
using GreenT.Types;
using GreenT.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace GreenT.HornyScapes;

public class MergeFieldOpener : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
{
	[Inject]
	private ContentSelectorGroup contentSelectorGroup;

	[SerializeField]
	private ContentType OpenFieldType;

	[SerializeField]
	private WindowOpener WindowOpener;

	public void OnPointerClick(PointerEventData eventData)
	{
		OpenMergeField();
	}

	private void OpenMergeField()
	{
		contentSelectorGroup.Select(OpenFieldType);
		Controller<GameItemController>.Instance.OpenField(OpenFieldType);
		WindowOpener.OpenOnly();
	}
}
