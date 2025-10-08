using UnityEngine;

[ExecuteInEditMode]
public class LineRenderControl : MonoBehaviour
{
	public GameObject target;

	public Material mat;

	private LineRenderer lR;

	private Renderer rend;

	private void Start()
	{
		lR = GetComponent<LineRenderer>();
		rend = GetComponent<Renderer>();
	}

	private void Update()
	{
		float num = Vector3.Distance(base.transform.position, target.transform.position);
		lR.SetPosition(1, new Vector3(0f, 0f, num));
		mat.SetTextureScale("_DisTex", new Vector2(num, 1f));
		rend.material = mat;
		base.transform.LookAt(target.transform.position);
	}
}
