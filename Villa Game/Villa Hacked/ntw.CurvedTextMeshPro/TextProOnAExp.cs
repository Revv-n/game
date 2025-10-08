using TMPro;
using UnityEngine;

namespace ntw.CurvedTextMeshPro;

[ExecuteInEditMode]
public class TextProOnAExp : TextProOnACurve
{
	[SerializeField]
	[Tooltip("The base of the exponential curve")]
	private float m_expBase = 1.3f;

	private float m_oldExpBase = float.MaxValue;

	protected override bool ParametersHaveChanged()
	{
		bool result = m_expBase != m_oldExpBase;
		m_oldExpBase = m_expBase;
		return result;
	}

	protected override Matrix4x4 ComputeTransformationMatrix(Vector3 charMidBaselinePos, float zeroToOnePos, TMP_TextInfo textInfo, int charIdx)
	{
		float x = charMidBaselinePos.x;
		float num = Mathf.Pow(m_expBase, x);
		Vector2 vector = new Vector2(x, num - textInfo.lineInfo[0].lineExtents.max.y * (float)textInfo.characterInfo[charIdx].lineNumber);
		return Matrix4x4.TRS(new Vector3(vector.x, vector.y, 0f), Quaternion.AngleAxis(Mathf.Atan(Mathf.Log(m_expBase) * num) * 57.29578f, Vector3.forward), Vector3.one);
	}
}
