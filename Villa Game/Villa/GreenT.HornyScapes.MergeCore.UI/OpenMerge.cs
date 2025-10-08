using GreenT.Types;
using UnityEngine;

namespace GreenT.HornyScapes.MergeCore.UI;

public class OpenMerge : MonoBehaviour
{
	public ContentType OpenFieldType;

	public void OpenField()
	{
		Controller<GameItemController>.Instance.OpenField(OpenFieldType);
	}
}
