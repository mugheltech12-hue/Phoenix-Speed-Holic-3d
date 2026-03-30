using UnityEngine;

public class SoundManager : MonoBehaviour
{
	public static SoundManager Instance;

	private AudioSource mAudio;

	private void Awake()
	{
		Object.DontDestroyOnLoad(base.gameObject);
		mAudio = GetComponent<AudioSource>();
		Instance = this;
	}

	public void PlayMusic()
	{
		if (!mAudio.isPlaying)
		{
			mAudio.Play();
		}
	}

	public void PauseMusic()
	{
		if (mAudio.isPlaying)
		{
			mAudio.Pause();
		}
	}

	private void OnDestroy()
	{
		Instance = null;
	}
}
