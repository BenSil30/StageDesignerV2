using UnityEngine;

[System.Serializable]
public class KeyframeClass
{
	public float KeyframeTime;
	public Light KeyframeLight;

	public Vector3 KeyframePosition;
	public Quaternion KeyframeRotation;

	public float KeyframeIntensity;
	public Color AlternativeKeyframeIntensity;
	public Color KeyframeColor;

	public float KeyframeRotationSpeed;
	public float KeyframePulseRate;

	public bool KeyframePulseOn;
	public bool KeyframeIsAnimating;

	// if prefab has a light to change and not a texture
	public KeyframeClass(float time, Light light, Vector3 position, Quaternion rotation, float intensity, Color color, float keyframeRotationSpeed, float keyframePulseRate, bool keyframePulseOn, bool keyframeIsAnimating)
	{
		KeyframeTime = time;
		KeyframeLight = light;

		KeyframePosition = position;
		KeyframeRotation = rotation;

		KeyframeIntensity = intensity;
		AlternativeKeyframeIntensity = Color.clear;
		KeyframeColor = color;

		KeyframeRotationSpeed = keyframeRotationSpeed;
		KeyframePulseRate = keyframePulseRate;

		KeyframePulseOn = keyframePulseOn;
		KeyframeIsAnimating = keyframeIsAnimating;
	}

	// if prefab doesn't have a light to change and instead uses a texture
	public KeyframeClass(float time, Vector3 position, Quaternion rotation, Color intensity, Color color, float keyframeRotationSpeed, float keyframePulseRate, bool keyframePulseOn, bool keyframeIsAnimating)
	{
		KeyframeTime = time;
		KeyframeLight = null;

		KeyframePosition = position;
		KeyframeRotation = rotation;

		KeyframeIntensity = -1f;
		AlternativeKeyframeIntensity = intensity;
		KeyframeColor = color;

		KeyframeRotationSpeed = keyframeRotationSpeed;
		KeyframePulseRate = keyframePulseRate;

		KeyframePulseOn = keyframePulseOn;
		KeyframeIsAnimating = keyframeIsAnimating;
	}
}