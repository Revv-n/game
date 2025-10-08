using UnityEngine;

public class ErolabsDemoApp : MonoBehaviour
{
	private void Start()
	{
		Object.Instantiate(Resources.Load<GameObject>("CoresdkDemo")).AddComponent<ErolabsDemoScript>();
	}
}
