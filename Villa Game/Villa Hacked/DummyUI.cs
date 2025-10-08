using UnityEngine;
using UnityEngine.UI;

public class DummyUI : MonoBehaviour
{
	public bool isSet;

	private void OnValidate()
	{
		if (!isSet)
		{
			RenameObject();
			AddImage();
			base.transform.SetAsFirstSibling();
			isSet = true;
		}
	}

	private void AddImage()
	{
		if (base.gameObject.GetComponent<Image>() == null)
		{
			base.gameObject.AddComponent<Image>();
		}
	}

	private void RenameObject()
	{
		base.gameObject.name = "DummyUI";
	}

	private void Start()
	{
		Object.Destroy(base.gameObject);
	}
}
