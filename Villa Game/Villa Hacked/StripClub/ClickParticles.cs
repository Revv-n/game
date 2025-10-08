using System;
using UniRx;
using UnityEngine;

namespace StripClub;

public class ClickParticles : MonoBehaviour
{
	[SerializeField]
	private ParticleSystem particles;

	[SerializeField]
	private Camera camera;

	private void Awake()
	{
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<long>(Observable.Where<long>(Observable.EveryUpdate(), (Func<long, bool>)((long _) => Input.GetMouseButtonDown(0))), (Action<long>)delegate
		{
			PlayParticles();
		}), (Component)this);
	}

	private void PlayParticles()
	{
		if (camera.enabled)
		{
			Vector3 mousePosition = Input.mousePosition;
			mousePosition.z = Camera.main.nearClipPlane;
			mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
			base.transform.InverseTransformPoint(mousePosition);
			((Component)(object)particles).transform.position = Input.mousePosition;
			particles.Stop();
			particles.Play();
		}
	}
}
