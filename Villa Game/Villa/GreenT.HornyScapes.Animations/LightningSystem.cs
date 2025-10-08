using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace GreenT.HornyScapes.Animations;

public class LightningSystem : MonoBehaviour
{
	[SerializeField]
	protected float maxValue = 0.8f;

	[SerializeField]
	protected float minValue;

	[SerializeField]
	protected float duration = 1f;

	[SerializeField]
	protected string propertyName = "_GrayScale_Fade_1";

	public Material lightningMaterial;

	public bool Debug;

	protected int propertyID = -1;

	protected void SetID()
	{
		propertyID = Shader.PropertyToID(propertyName);
	}

	public Tween Apply(SpriteRenderer renderer, MaterialPropertyBlock propBlock)
	{
		CheckSettings("Apply " + propertyName);
		renderer.material = lightningMaterial;
		return StartLightning(renderer, propBlock, minValue, maxValue);
	}

	public Tween Apply(Image image)
	{
		CheckSettings("Apply " + propertyName);
		image.material = Object.Instantiate(new Material(lightningMaterial));
		return StartLightning(image.material, minValue, maxValue);
	}

	public Tween Undo(SpriteRenderer renderer, MaterialPropertyBlock propBlock)
	{
		CheckSettings("Undo " + propertyName);
		renderer.material = lightningMaterial;
		return StartLightning(renderer, propBlock, maxValue, minValue);
	}

	public Tween Undo(Image image, Tween launched = null)
	{
		CheckSettings("Undo " + propertyName);
		launched?.Kill();
		return StartLightning(image.material, maxValue, minValue);
	}

	private void CheckSettings(string consoleLog)
	{
		_ = Debug;
		if (propertyID == -1)
		{
			SetID();
		}
	}

	protected virtual Tween StartLightning(SpriteRenderer renderer, MaterialPropertyBlock propBlock, float from, float to)
	{
		renderer.GetPropertyBlock(propBlock);
		float currentValue = from;
		SetLightning(renderer, propBlock, currentValue);
		return DOTween.To(() => currentValue, delegate(float x)
		{
			currentValue = x;
			SetLightning(renderer, propBlock, x);
		}, to, duration);
	}

	protected virtual Tween StartLightning(Material imageMaterial, float from, float to)
	{
		SetLightning(imageMaterial, from);
		return DOTween.To(() => from, delegate(float x)
		{
			from = x;
			SetLightning(imageMaterial, x);
		}, to, duration);
	}

	protected void SetLightning(SpriteRenderer renderer, MaterialPropertyBlock propBlock, float value)
	{
		_ = Debug;
		propBlock.SetFloat(propertyID, value);
		renderer.SetPropertyBlock(propBlock);
	}

	protected void SetLightning(Material imageMaterial, float value)
	{
		_ = Debug;
		imageMaterial.SetFloat(propertyID, value);
	}
}
