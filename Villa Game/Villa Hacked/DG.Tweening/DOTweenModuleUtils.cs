using System;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Core.PathCore;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.Scripting;

namespace DG.Tweening;

public static class DOTweenModuleUtils
{
	public static class Physics
	{
		public static void SetOrientationOnPath(PathOptions options, Tween t, Quaternion newRot, Transform trans)
		{
			trans.rotation = newRot;
		}

		public static bool HasRigidbody2D(Component target)
		{
			return target.GetComponent<Rigidbody2D>() != null;
		}

		[Preserve]
		public static bool HasRigidbody(Component target)
		{
			return false;
		}

		[Preserve]
		public static TweenerCore<Vector3, Path, PathOptions> CreateDOTweenPathTween(MonoBehaviour target, bool tweenRigidbody, bool isLocal, Path path, float duration, PathMode pathMode)
		{
			if (!isLocal)
			{
				return target.transform.DOPath(path, duration, pathMode);
			}
			return target.transform.DOLocalPath(path, duration, pathMode);
		}
	}

	private static bool _initialized;

	[Preserve]
	public static void Init()
	{
		if (!_initialized)
		{
			_initialized = true;
			DOTweenExternalCommand.SetOrientationOnPath += Physics.SetOrientationOnPath;
		}
	}

	[Preserve]
	private static void Preserver()
	{
		AppDomain.CurrentDomain.GetAssemblies();
		typeof(MonoBehaviour).GetMethod("Stub");
	}
}
