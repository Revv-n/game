using GreenT.HornyScapes.Events.Content;
using GreenT.Types;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Events;

public class MonoContentSetter : MonoBehaviour
{
	[SerializeField]
	private ContentType setType;

	[Inject]
	private ContentSelectorGroup contentSelectorGroup;

	public void Set()
	{
		contentSelectorGroup.Select(setType);
	}
}
