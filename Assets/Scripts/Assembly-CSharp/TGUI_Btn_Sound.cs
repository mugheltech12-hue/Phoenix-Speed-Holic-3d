using UnityEngine;

public class TGUI_Btn_Sound : TGUI_Btn_Basic
{
	[SerializeField]
	private TextMesh txt_Sound;

	private int sound;

	protected override void Awake()
	{
		base.Awake();
		sound = DataManager.Instance.GetIntDB(DataManager.DBNameTag.SOUND);
		switch (sound)
		{
		case 0:
			txt_Sound.text = "off";
			break;
		case 1:
			txt_Sound.text = "on";
			break;
		default:
			txt_Sound.text = "on";
			break;
		}
	}

	protected override void OnClicked()
	{
		base.OnClicked();
		switch (sound)
		{
		case 0:
			SoundManager.Instance.PlayMusic();
			txt_Sound.text = "on";
			sound = 1;
			break;
		case 1:
			SoundManager.Instance.PauseMusic();
			txt_Sound.text = "off";
			sound = 0;
			break;
		}
		DataManager.Instance.SetIntDB(DataManager.DBNameTag.SOUND, sound);
	}
}
