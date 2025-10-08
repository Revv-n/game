using System;
using UnityEngine;
using UnityEngine.Sprites;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(CanvasRenderer))]
[RequireComponent(typeof(ParticleSystem))]
public class UIParticleSystem : MaskableGraphic
{
	public Texture particleTexture;

	public Sprite particleSprite;

	[SerializeField]
	private ParticleSystemRenderMode _particleSystemRenderMode;

	private Transform _transform;

	private ParticleSystem _particleSystem;

	private Particle[] _particles;

	private UIVertex[] _quad = new UIVertex[4];

	private Vector4 _uv = Vector4.zero;

	private TextureSheetAnimationModule _textureSheetAnimation;

	private int _textureSheetAnimationFrames;

	private Vector2 _textureSheedAnimationFrameSize;

	public override Texture mainTexture
	{
		get
		{
			if ((bool)particleTexture)
			{
				return particleTexture;
			}
			if ((bool)particleSprite)
			{
				return particleSprite.texture;
			}
			return null;
		}
	}

	public Particle[] particles => _particles;

	protected bool Initialize()
	{
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		if (_transform == null)
		{
			_transform = base.transform;
		}
		if ((UnityEngine.Object)(object)_particleSystem == null)
		{
			_particleSystem = GetComponent<ParticleSystem>();
			if ((UnityEngine.Object)(object)_particleSystem == null)
			{
				return false;
			}
			ParticleSystemRenderer val = ((Component)(object)_particleSystem).GetComponent<ParticleSystemRenderer>();
			if ((UnityEngine.Object)(object)val == null)
			{
				val = ((Component)(object)_particleSystem).gameObject.AddComponent<ParticleSystemRenderer>();
			}
			Material sharedMaterial = ((Renderer)(object)val).sharedMaterial;
			if ((bool)sharedMaterial && sharedMaterial.HasProperty("_MainTex"))
			{
				particleTexture = sharedMaterial.mainTexture;
			}
			Material material = new Material(Shader.Find("UI/Particles/Hidden"));
			if (Application.isPlaying)
			{
				((Renderer)(object)val).material = material;
			}
			_particleSystem.scalingMode = (ParticleSystemScalingMode)0;
			_particles = null;
		}
		if (_particles == null)
		{
			_particles = (Particle[])(object)new Particle[_particleSystem.maxParticles];
		}
		if ((bool)particleTexture)
		{
			_uv = new Vector4(0f, 0f, 1f, 1f);
		}
		else if ((bool)particleSprite)
		{
			_uv = DataUtility.GetOuterUV(particleSprite);
		}
		_textureSheetAnimation = _particleSystem.textureSheetAnimation;
		_textureSheetAnimationFrames = 0;
		_textureSheedAnimationFrameSize = Vector2.zero;
		if (((TextureSheetAnimationModule)(ref _textureSheetAnimation)).enabled)
		{
			_textureSheetAnimationFrames = ((TextureSheetAnimationModule)(ref _textureSheetAnimation)).numTilesX * ((TextureSheetAnimationModule)(ref _textureSheetAnimation)).numTilesY;
			_textureSheedAnimationFrameSize = new Vector2(1f / (float)((TextureSheetAnimationModule)(ref _textureSheetAnimation)).numTilesX, 1f / (float)((TextureSheetAnimationModule)(ref _textureSheetAnimation)).numTilesY);
		}
		((Component)(object)_particleSystem).GetComponent<ParticleSystemRenderer>().renderMode = _particleSystemRenderMode;
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
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Invalid comparison between Unknown and I4
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Invalid comparison between Unknown and I4
		vh.Clear();
		if (!base.gameObject.activeInHierarchy)
		{
			return;
		}
		int num = _particleSystem.GetParticles(_particles);
		for (int i = 0; i < num; i++)
		{
			Particle val = _particles[i];
			Vector2 vector = (((int)_particleSystem.simulationSpace == 0) ? ((Particle)(ref val)).position : _transform.InverseTransformPoint(((Particle)(ref val)).position));
			float num2 = (0f - ((Particle)(ref val)).rotation) * ((float)Math.PI / 180f);
			float f = num2 + (float)Math.PI / 2f;
			Color32 currentColor = ((Particle)(ref val)).GetCurrentColor(_particleSystem);
			float num3 = ((Particle)(ref val)).GetCurrentSize(_particleSystem) * 0.5f;
			if ((int)_particleSystem.scalingMode == 2)
			{
				vector /= base.canvas.scaleFactor;
			}
			Vector4 uv = _uv;
			if (((TextureSheetAnimationModule)(ref _textureSheetAnimation)).enabled)
			{
				float num4 = 1f - ((Particle)(ref val)).remainingLifetime / ((Particle)(ref val)).startLifetime;
				num4 = Mathf.Repeat(num4 * (float)((TextureSheetAnimationModule)(ref _textureSheetAnimation)).cycleCount, 1f);
				int num5 = 0;
				ParticleSystemAnimationType animation = ((TextureSheetAnimationModule)(ref _textureSheetAnimation)).animation;
				if ((int)animation != 0)
				{
					if ((int)animation == 1)
					{
						num5 = Mathf.FloorToInt(num4 * (float)((TextureSheetAnimationModule)(ref _textureSheetAnimation)).numTilesX);
						int rowIndex = ((TextureSheetAnimationModule)(ref _textureSheetAnimation)).rowIndex;
						num5 += rowIndex * ((TextureSheetAnimationModule)(ref _textureSheetAnimation)).numTilesX;
					}
				}
				else
				{
					num5 = Mathf.FloorToInt(num4 * (float)_textureSheetAnimationFrames);
				}
				num5 %= _textureSheetAnimationFrames;
				uv.x = (float)(num5 % ((TextureSheetAnimationModule)(ref _textureSheetAnimation)).numTilesX) * _textureSheedAnimationFrameSize.x;
				uv.y = (float)Mathf.FloorToInt(num5 / ((TextureSheetAnimationModule)(ref _textureSheetAnimation)).numTilesX) * _textureSheedAnimationFrameSize.y;
				uv.z = uv.x + _textureSheedAnimationFrameSize.x;
				uv.w = uv.y + _textureSheedAnimationFrameSize.y;
			}
			_quad[0] = UIVertex.simpleVert;
			_quad[0].color = currentColor;
			_quad[0].uv0 = new Vector2(uv.x, uv.y);
			_quad[1] = UIVertex.simpleVert;
			_quad[1].color = currentColor;
			_quad[1].uv0 = new Vector2(uv.x, uv.w);
			_quad[2] = UIVertex.simpleVert;
			_quad[2].color = currentColor;
			_quad[2].uv0 = new Vector2(uv.z, uv.w);
			_quad[3] = UIVertex.simpleVert;
			_quad[3].color = currentColor;
			_quad[3].uv0 = new Vector2(uv.z, uv.y);
			if (num2 == 0f)
			{
				Vector2 vector2 = new Vector2(vector.x - num3, vector.y - num3);
				Vector2 vector3 = new Vector2(vector.x + num3, vector.y + num3);
				_quad[0].position = new Vector2(vector2.x, vector2.y);
				_quad[1].position = new Vector2(vector2.x, vector3.y);
				_quad[2].position = new Vector2(vector3.x, vector3.y);
				_quad[3].position = new Vector2(vector3.x, vector2.y);
			}
			else
			{
				Vector2 vector4 = new Vector2(Mathf.Cos(num2), Mathf.Sin(num2)) * num3;
				Vector2 vector5 = new Vector2(Mathf.Cos(f), Mathf.Sin(f)) * num3;
				_quad[0].position = vector - vector4 - vector5;
				_quad[1].position = vector - vector4 + vector5;
				_quad[2].position = vector + vector4 + vector5;
				_quad[3].position = vector + vector4 - vector5;
			}
			vh.AddUIVertexQuad(_quad);
		}
	}

	private void Update()
	{
		if (Application.isPlaying)
		{
			_particleSystem.Simulate(Time.unscaledDeltaTime, false, false);
			SetAllDirty();
		}
	}
}
