using TMPro;
using UnityEngine;

namespace GreenT.Utilities;

[RequireComponent(typeof(TMP_Text))]
[DisallowMultipleComponent]
public class TMPShaderFinder : ShaderFinder
{
	[SerializeField]
	private TMP_Text target;

	public void Start()
	{
		Material fontSharedMaterial = target.fontSharedMaterial;
		target.fontSharedMaterial = null;
		ShaderFinder.UpdateMaterial(fontSharedMaterial);
		target.fontSharedMaterial = fontSharedMaterial;
	}

	private void OnValidate()
	{
		if (target == null)
		{
			target = GetComponent<TMP_Text>();
		}
	}
}
