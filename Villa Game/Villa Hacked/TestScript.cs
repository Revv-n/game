using System.Collections;
using System.Globalization;
using UnityEngine;

public class TestScript : MonoBehaviour
{
	public Color TestColor;

	[EditorButton]
	public void SetScale(float scale)
	{
		base.transform.localScale = new Vector3(scale, scale, scale);
	}

	[EditorButton]
	public void SetScaleX(float scale = 5f)
	{
		base.transform.localScale = new Vector3(scale, base.transform.localScale.y, base.transform.localScale.z);
	}

	[EditorButton]
	public void FloatIntDefault(float floatAsInt = 5f)
	{
		Debug.Log("floatAsInt " + floatAsInt);
	}

	[EditorButton("Пустой метод", true)]
	public void EmptyMethodNamed()
	{
	}

	[EditorButton("Пустой метод без названия", false)]
	public void EmptyMethod()
	{
	}

	[EditorButton]
	public void PrintStuff(float floatVal, int intVal, string stringVal, bool boolVal)
	{
		Debug.Log("floatVal " + floatVal);
		Debug.Log("intVal " + intVal);
		Debug.Log("stringVal " + stringVal);
		Debug.Log("boolVal " + boolVal);
	}

	[EditorButton]
	public void SetMaterialColor(Color color)
	{
		GetComponent<MeshRenderer>().sharedMaterial.color = color;
	}

	[EditorButton]
	public IEnumerator CountTo(int max = 6)
	{
		int current = 0;
		while (current < max)
		{
			Debug.Log(current++);
			yield return new WaitForSeconds(1f);
		}
	}

	[EditorButton]
	public string ConvertToHex(Color color)
	{
		return "#" + ColorToHex(color);
	}

	[EditorButton]
	public void PrintNameOf(GameObject go)
	{
		Debug.Log(go.name);
	}

	private string ColorToHex(Color32 color)
	{
		return color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
	}

	private Color HexToColor(string hex)
	{
		byte r = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
		byte g = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
		byte b = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
		return new Color32(r, g, b, byte.MaxValue);
	}
}
