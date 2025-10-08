using System;
using TMPro;
using UnityEngine;

namespace ntw.CurvedTextMeshPro;

public class CurvedText : TextProOnACurve
{
	protected override bool ParametersHaveChanged()
	{
		throw new NotImplementedException();
	}

	protected override Matrix4x4 ComputeTransformationMatrix(Vector3 charMidBaselinePos, float zeroToOnePos, TMP_TextInfo textInfo, int charIdx)
	{
		throw new NotImplementedException();
	}
}
