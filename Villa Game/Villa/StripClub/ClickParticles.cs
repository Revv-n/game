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
		(from _ in Observable.EveryUpdate()
			where Input.GetMouseButtonDown(0)
			select _).Subscribe(delegate
		{
			PlayParticles();
		}).AddTo(this);
	}

	private void PlayParticles()
	{
		if (camera.enabled)
		{
			Vector3 mousePosition = Input.mousePosition;
			mousePosition.z = Camera.main.nearClipPlane;
			mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
			base.transform.InverseTransformPoint(mousePosition);
			particles.transform.position = Input.mousePosition;
			particles.Stop();
			particles.Play();
		}
	}
}
