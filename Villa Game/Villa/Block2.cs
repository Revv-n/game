using DG.Tweening;
using UnityEngine;

public class Block2 : MonoBehaviour
{
	public Transform tr;

	public float time;

	private void Start()
	{
		Sequence s = DOTween.Sequence();
		s.Join(tr.transform.DORotate(new Vector3(0f, 0f, -15f), time));
		s.Append(tr.transform.DORotate(new Vector3(0f, 0f, 15f), time));
		s.Append(tr.transform.DORotate(new Vector3(0f, 0f, -15f), time));
		s.Append(tr.transform.DORotate(new Vector3(0f, 0f, 0f), time));
	}

	private void Update()
	{
	}
}
