using UnityEngine;

public class IntAttribute : PropertyAttribute
{
	private string path = "";

	public IntAttribute(string path)
	{
		this.path = path;
	}
}
