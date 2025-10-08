using System;
using System.Collections.Generic;
using UnityEngine;

namespace GreenT.HornyScapes;

[Serializable]
[CreateAssetMenu(menuName = "TextMeshPro/Asian Font Fallback Config", fileName = "AsianFontFallbackConfig")]
public class AsianFontFallbackConfig : ScriptableObject
{
	public List<FontFallbackConfig> FontFallbackConfigs;
}
