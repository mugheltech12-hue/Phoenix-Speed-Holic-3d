using UnityEngine;

public class DataManager : MonoBehaviour
{
	public enum DBNameTag
	{
		UPDATE1_2 = 0,
		BEST_SCORE_T_07x = 1,
		BEST_SCORE_T_10x = 2,
		BEST_SCORE_T_13x = 3,
		BEST_SCORE_G_07x = 4,
		BEST_SCORE_G_10x = 5,
		BEST_SCORE_G_13x = 6,
		TUTORIAL_T = 7,
		TUTORIAL_G = 8,
		CHROMA = 9,
		SOUND = 10,
		SPEED = 11,
		MODE = 12,
		SKIN = 13,
		BALL = 14,
		ROAD = 15
	}

	public static DataManager Instance;

	private void Awake()
	{
		Object.DontDestroyOnLoad(base.gameObject);
		Instance = this;
		if (PlayerPrefs.HasKey("SOCIAL"))
		{
			PlayerPrefs.DeleteKey("SOCIAL");
		}
		if (!PlayerPrefs.HasKey("BEST_SCORE_T_07x"))
		{
			PlayerPrefs.SetInt("BEST_SCORE_T_07x", 0);
		}
		if (!PlayerPrefs.HasKey("BEST_SCORE_T_10x"))
		{
			PlayerPrefs.SetInt("BEST_SCORE_T_10x", 0);
		}
		if (!PlayerPrefs.HasKey("BEST_SCORE_T_13x"))
		{
			PlayerPrefs.SetInt("BEST_SCORE_T_13x", 0);
		}
		if (!PlayerPrefs.HasKey("BEST_SCORE_G_07x"))
		{
			PlayerPrefs.SetInt("BEST_SCORE_G_07x", 0);
		}
		if (!PlayerPrefs.HasKey("BEST_SCORE_G_10x"))
		{
			PlayerPrefs.SetInt("BEST_SCORE_G_10x", 0);
		}
		if (!PlayerPrefs.HasKey("BEST_SCORE_G_13x"))
		{
			PlayerPrefs.SetInt("BEST_SCORE_G_13x", 0);
		}
		if (!PlayerPrefs.HasKey("TUTORIAL_T"))
		{
			PlayerPrefs.SetInt("TUTORIAL_T", 1);
		}
		if (!PlayerPrefs.HasKey("TUTORIAL_G"))
		{
			PlayerPrefs.SetInt("TUTORIAL_G", 1);
		}
		if (!PlayerPrefs.HasKey("CHROMA"))
		{
			PlayerPrefs.SetInt("CHROMA", 1);
		}
		if (!PlayerPrefs.HasKey("SOUND"))
		{
			PlayerPrefs.SetInt("SOUND", 1);
		}
		if (!PlayerPrefs.HasKey("SPEED"))
		{
			PlayerPrefs.SetInt("SPEED", 1);
		}
		if (!PlayerPrefs.HasKey("MODE"))
		{
			PlayerPrefs.SetInt("MODE", 1);
		}
		if (!PlayerPrefs.HasKey("SKIN"))
		{
			PlayerPrefs.SetInt("SKIN", 1);
		}
		if (!PlayerPrefs.HasKey("BALL"))
		{
			PlayerPrefs.SetInt("BALL", 1);
		}
		if (!PlayerPrefs.HasKey("ROAD"))
		{
			PlayerPrefs.SetInt("ROAD", 1);
		}
		PlayerPrefs.Save();
		ModifySoundDB();
	}

	private void ModifySoundDB()
	{
		int intDB = GetIntDB(DBNameTag.SOUND);
		if (intDB != 0 && intDB != 1)
		{
			SetIntDB(DBNameTag.SOUND, 1);
		}
	}

	public int GetIntDB(DBNameTag _tag)
	{
		if (PlayerPrefs.HasKey(string.Format("{0}", _tag)))
		{
			return PlayerPrefs.GetInt(string.Format("{0}", _tag));
		}
		Debug.LogWarning("The key does not exist");
		return -100;
	}

	public void SetIntDB(DBNameTag _tag, int _val)
	{
		PlayerPrefs.SetInt(string.Format("{0}", _tag), _val);
		PlayerPrefs.Save();
	}

	private void OnDestroy()
	{
		Instance = null;
	}
}
