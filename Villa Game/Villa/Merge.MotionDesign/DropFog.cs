using System;
using System.Collections;
using UnityEngine;

namespace Merge.MotionDesign;

public class DropFog : MonoBehaviour, IPoolReturner
{
	[SerializeField]
	private Transform sizeFitter;

	[SerializeField]
	private float lifeTime;

	private Coroutine lifeCrt;

	public Action ReturnInPool { get; set; }

	private void OnEnable()
	{
		lifeCrt = StartCoroutine(CRT_AutoHide());
	}

	private IEnumerator CRT_AutoHide()
	{
		yield return new WaitForSeconds(lifeTime);
		ReturnInPool();
	}
}
