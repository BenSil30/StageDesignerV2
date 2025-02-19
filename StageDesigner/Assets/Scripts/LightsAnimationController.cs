using UnityEngine;

public class LightsAnimationController : MonoBehaviour
{
	public UIManager UIManager;
	public ItemManager ItemManager;
	public AudioSource AudioSource;

	public float CurrentSongTime;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	private void Start()
	{
	}

	// Update is called once per frame
	private void Update()
	{
		if (AudioSource.clip != null) CurrentSongTime = AudioSource.time;
	}
}