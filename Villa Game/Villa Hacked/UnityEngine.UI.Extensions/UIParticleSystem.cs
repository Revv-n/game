using System;

namespace UnityEngine.UI.Extensions;

[ExecuteInEditMode]
[RequireComponent(typeof(CanvasRenderer), typeof(ParticleSystem))]
[AddComponentMenu("UI/Effects/Extensions/UIParticleSystem")]
public class UIParticleSystem : MaskableGraphic
{
	[Tooltip("Having this enabled run the system in LateUpdate rather than in Update making it faster but less precise (more clunky)")]
	public bool fixedTime = true;

	private Transform _transform;

	private ParticleSystem pSystem;

	private Particle[] particles;

	private UIVertex[] _quad = new UIVertex[4];

	private Vector4 imageUV = Vector4.zero;

	private TextureSheetAnimationModule textureSheetAnimation;

	private int textureSheetAnimationFrames;

	private Vector2 textureSheetAnimationFrameSize;

	private ParticleSystemRenderer pRenderer;

	private Material currentMaterial;

	private Texture currentTexture;

	private MainModule mainModule;

	public override Texture mainTexture => currentTexture;

	protected bool Initialize()
	{
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		if (_transform == null)
		{
			_transform = base.transform;
		}
		MainModule main;
		if ((Object)(object)pSystem == null)
		{
			pSystem = GetComponent<ParticleSystem>();
			if ((Object)(object)pSystem == null)
			{
				return false;
			}
			mainModule = pSystem.main;
			main = pSystem.main;
			if (((MainModule)(ref main)).maxParticles > 14000)
			{
				((MainModule)(ref mainModule)).maxParticles = 14000;
			}
			pRenderer = ((Component)(object)pSystem).GetComponent<ParticleSystemRenderer>();
			if ((Object)(object)pRenderer != null)
			{
				((Renderer)(object)pRenderer).enabled = false;
			}
			Material material = new Material(Shader.Find("UI Extensions/Particles/Additive"));
			if (this.material == null)
			{
				this.material = material;
			}
			currentMaterial = this.material;
			if ((bool)currentMaterial && currentMaterial.HasProperty("_MainTex"))
			{
				currentTexture = currentMaterial.mainTexture;
				if (currentTexture == null)
				{
					currentTexture = Texture2D.whiteTexture;
				}
			}
			this.material = currentMaterial;
			((MainModule)(ref mainModule)).scalingMode = (ParticleSystemScalingMode)0;
			particles = null;
		}
		if (particles == null)
		{
			main = pSystem.main;
			particles = (Particle[])(object)new Particle[((MainModule)(ref main)).maxParticles];
		}
		imageUV = new Vector4(0f, 0f, 1f, 1f);
		textureSheetAnimation = pSystem.textureSheetAnimation;
		textureSheetAnimationFrames = 0;
		textureSheetAnimationFrameSize = Vector2.zero;
		if (((TextureSheetAnimationModule)(ref textureSheetAnimation)).enabled)
		{
			textureSheetAnimationFrames = ((TextureSheetAnimationModule)(ref textureSheetAnimation)).numTilesX * ((TextureSheetAnimationModule)(ref textureSheetAnimation)).numTilesY;
			textureSheetAnimationFrameSize = new Vector2(1f / (float)((TextureSheetAnimationModule)(ref textureSheetAnimation)).numTilesX, 1f / (float)((TextureSheetAnimationModule)(ref textureSheetAnimation)).numTilesY);
		}
		return true;
	}

	protected override void Awake()
	{
		base.Awake();
		if (!Initialize())
		{
			base.enabled = false;
		}
	}

	protected override void OnPopulateMesh(VertexHelper vh)
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Invalid comparison between Unknown and I4
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Invalid comparison between Unknown and I4
		vh.Clear();
		if (!base.gameObject.activeInHierarchy)
		{
			return;
		}
		Vector2 zero = Vector2.zero;
		Vector2 zero2 = Vector2.zero;
		Vector2 zero3 = Vector2.zero;
		int num = pSystem.GetParticles(particles);
		for (int i = 0; i < num; i++)
		{
			Particle val = particles[i];
			Vector2 vector = (((int)((MainModule)(ref mainModule)).simulationSpace == 0) ? ((Particle)(ref val)).position : _transform.InverseTransformPoint(((Particle)(ref val)).position));
			float num2 = (0f - ((Particle)(ref val)).rotation) * ((float)Math.PI / 180f);
			float f = num2 + (float)Math.PI / 2f;
			Color32 currentColor = ((Particle)(ref val)).GetCurrentColor(pSystem);
			float num3 = ((Particle)(ref val)).GetCurrentSize(pSystem) * 0.5f;
			if ((int)((MainModule)(ref mainModule)).scalingMode == 2)
			{
				vector /= base.canvas.scaleFactor;
			}
			Vector4 vector2 = imageUV;
			if (((TextureSheetAnimationModule)(ref textureSheetAnimation)).enabled)
			{
				float num4 = 1f - ((Particle)(ref val)).remainingLifetime / ((Particle)(ref val)).startLifetime;
				MinMaxCurve frameOverTime = ((TextureSheetAnimationModule)(ref textureSheetAnimation)).frameOverTime;
				if (((MinMaxCurve)(ref frameOverTime)).curveMin != null)
				{
					frameOverTime = ((TextureSheetAnimationModule)(ref textureSheetAnimation)).frameOverTime;
					num4 = ((MinMaxCurve)(ref frameOverTime)).curveMin.Evaluate(1f - ((Particle)(ref val)).remainingLifetime / ((Particle)(ref val)).startLifetime);
				}
				else
				{
					frameOverTime = ((TextureSheetAnimationModule)(ref textureSheetAnimation)).frameOverTime;
					if (((MinMaxCurve)(ref frameOverTime)).curve != null)
					{
						frameOverTime = ((TextureSheetAnimationModule)(ref textureSheetAnimation)).frameOverTime;
						num4 = ((MinMaxCurve)(ref frameOverTime)).curve.Evaluate(1f - ((Particle)(ref val)).remainingLifetime / ((Particle)(ref val)).startLifetime);
					}
					else
					{
						frameOverTime = ((TextureSheetAnimationModule)(ref textureSheetAnimation)).frameOverTime;
						if (((MinMaxCurve)(ref frameOverTime)).constant > 0f)
						{
							frameOverTime = ((TextureSheetAnimationModule)(ref textureSheetAnimation)).frameOverTime;
							num4 = ((MinMaxCurve)(ref frameOverTime)).constant - ((Particle)(ref val)).remainingLifetime / ((Particle)(ref val)).startLifetime;
						}
					}
				}
				num4 = Mathf.Repeat(num4 * (float)((TextureSheetAnimationModule)(ref textureSheetAnimation)).cycleCount, 1f);
				int num5 = 0;
				ParticleSystemAnimationType animation = ((TextureSheetAnimationModule)(ref textureSheetAnimation)).animation;
				if ((int)animation != 0)
				{
					if ((int)animation == 1)
					{
						num5 = Mathf.FloorToInt(num4 * (float)((TextureSheetAnimationModule)(ref textureSheetAnimation)).numTilesX);
						int rowIndex = ((TextureSheetAnimationModule)(ref textureSheetAnimation)).rowIndex;
						num5 += rowIndex * ((TextureSheetAnimationModule)(ref textureSheetAnimation)).numTilesX;
					}
				}
				else
				{
					num5 = Mathf.FloorToInt(num4 * (float)textureSheetAnimationFrames);
				}
				num5 %= textureSheetAnimationFrames;
				vector2.x = (float)(num5 % ((TextureSheetAnimationModule)(ref textureSheetAnimation)).numTilesX) * textureSheetAnimationFrameSize.x;
				vector2.y = (float)Mathf.FloorToInt(num5 / ((TextureSheetAnimationModule)(ref textureSheetAnimation)).numTilesX) * textureSheetAnimationFrameSize.y;
				vector2.z = vector2.x + textureSheetAnimationFrameSize.x;
				vector2.w = vector2.y + textureSheetAnimationFrameSize.y;
			}
			zero.x = vector2.x;
			zero.y = vector2.y;
			_quad[0] = UIVertex.simpleVert;
			_quad[0].color = currentColor;
			_quad[0].uv0 = zero;
			zero.x = vector2.x;
			zero.y = vector2.w;
			_quad[1] = UIVertex.simpleVert;
			_quad[1].color = currentColor;
			_quad[1].uv0 = zero;
			zero.x = vector2.z;
			zero.y = vector2.w;
			_quad[2] = UIVertex.simpleVert;
			_quad[2].color = currentColor;
			_quad[2].uv0 = zero;
			zero.x = vector2.z;
			zero.y = vector2.y;
			_quad[3] = UIVertex.simpleVert;
			_quad[3].color = currentColor;
			_quad[3].uv0 = zero;
			if (num2 == 0f)
			{
				zero2.x = vector.x - num3;
				zero2.y = vector.y - num3;
				zero3.x = vector.x + num3;
				zero3.y = vector.y + num3;
				zero.x = zero2.x;
				zero.y = zero2.y;
				_quad[0].position = zero;
				zero.x = zero2.x;
				zero.y = zero3.y;
				_quad[1].position = zero;
				zero.x = zero3.x;
				zero.y = zero3.y;
				_quad[2].position = zero;
				zero.x = zero3.x;
				zero.y = zero2.y;
				_quad[3].position = zero;
			}
			else
			{
				Vector2 vector3 = new Vector2(Mathf.Cos(num2), Mathf.Sin(num2)) * num3;
				Vector2 vector4 = new Vector2(Mathf.Cos(f), Mathf.Sin(f)) * num3;
				_quad[0].position = vector - vector3 - vector4;
				_quad[1].position = vector - vector3 + vector4;
				_quad[2].position = vector + vector3 + vector4;
				_quad[3].position = vector + vector3 - vector4;
			}
			vh.AddUIVertexQuad(_quad);
		}
	}

	private void Update()
	{
		if (!fixedTime && Application.isPlaying)
		{
			pSystem.Simulate(Time.unscaledDeltaTime, false, false, true);
			SetAllDirty();
			if ((currentMaterial != null && currentTexture != currentMaterial.mainTexture) || (material != null && currentMaterial != null && material.shader != currentMaterial.shader))
			{
				pSystem = null;
				Initialize();
			}
		}
	}

	private void LateUpdate()
	{
		if (!Application.isPlaying)
		{
			SetAllDirty();
		}
		else if (fixedTime)
		{
			pSystem.Simulate(Time.unscaledDeltaTime, false, false, true);
			SetAllDirty();
			if ((currentMaterial != null && currentTexture != currentMaterial.mainTexture) || (material != null && currentMaterial != null && material.shader != currentMaterial.shader))
			{
				pSystem = null;
				Initialize();
			}
		}
		if (!(material == currentMaterial))
		{
			pSystem = null;
			Initialize();
		}
	}
}
