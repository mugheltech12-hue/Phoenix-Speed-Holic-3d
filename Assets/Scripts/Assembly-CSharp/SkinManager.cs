using UnityEngine;

public class SkinManager : MonoBehaviour
{
	public static SkinManager Instance;

	private Transform mTransform;

	private Transform[] cTransform;

	public Material railMat;

	public Material roadMat;

	public Material ballMat;

	public Material trailMat;

	public Material skin_1_3;

	public Material skin_2;

	public Material skin_4;

	public Material skin_5;

	public Material skin_6;

	public Material skin_7;

	public Material skin_8;

	public Material skin_9;

	private const int startR = 16;

	private const int startG = 10;

	private const int startB = 0;

	private int chroma;

	private int skin;

	private int cR;

	private int cG;

	private int cB;

	private void Awake()
	{
		Instance = this;
		chroma = PlayerPrefs.GetInt("CHROMA");
		mTransform = base.transform;
		cTransform = new Transform[mTransform.childCount];
		
	}


    private void Start()
    {
        SetSkin(DataManager.Instance.GetIntDB(DataManager.DBNameTag.SKIN));
        SetRoad(DataManager.Instance.GetIntDB(DataManager.DBNameTag.ROAD));
        SetBall(DataManager.Instance.GetIntDB(DataManager.DBNameTag.BALL));
    }

    private void SetBrightnessLevel(int level)
	{
		chroma = level;
		PlayerPrefs.SetInt("CHROMA", chroma);
		switch (skin)
		{
		case 0:
			break;
		case 1:
			cR = 5 * level;
			cG = 16 * level;
			cB = 5 * level;
			skin_1_3.SetColor("_TintColor", new Color((float)cR / 255f, (float)cG / 255f, (float)cB / 255f));
			break;
		case 2:
			cR = 16 * level;
			cG = 0 * level;
			cB = 10 * level;
			skin_2.SetColor("_TintColor", new Color((float)cR / 255f, (float)cG / 255f, (float)cB / 255f));
			break;
		case 3:
			cR = 16 * level;
			cG = 10 * level;
			cB = 0 * level;
			skin_1_3.SetColor("_TintColor", new Color((float)cR / 255f, (float)cG / 255f, (float)cB / 255f));
			break;
		case 4:
			cR = 0 * level;
			cG = 10 * level;
			cB = 16 * level;
			skin_4.SetColor("_TintColor", new Color((float)cR / 255f, (float)cG / 255f, (float)cB / 255f));
			break;
		case 5:
			cR = 16 * level;
			cG = 5 * level;
			cB = 5 * level;
			skin_5.SetColor("_TintColor", new Color((float)cR / 255f, (float)cG / 255f, (float)cB / 255f));
			break;
		case 6:
			cR = 5 * level;
			cG = 5 * level;
			cB = 16 * level;
			skin_6.SetColor("_TintColor", new Color((float)cR / 255f, (float)cG / 255f, (float)cB / 255f));
			break;
		case 7:
			cR = 5 * level;
			cG = 16 * level;
			cB = 5 * level;
			skin_7.SetColor("_TintColor", new Color((float)cR / 255f, (float)cG / 255f, (float)cB / 255f));
			break;
		case 8:
			cR = 16 * level;
			cG = 8 * level;
			cB = 0 * level;
			skin_8.SetColor("_TintColor", new Color((float)cR / 255f, (float)cG / 255f, (float)cB / 255f));
			break;
		case 9:
			cR = 0 * level;
			cG = 8 * level;
			cB = 16 * level;
			skin_9.SetColor("_TintColor", new Color((float)cR / 255f, (float)cG / 255f, (float)cB / 255f));
			break;
		}
	}

	public Material SetSkin(int _skin)
	{
		skin = _skin;
		switch (_skin)
		{
		case 0:
		{
			SetBrightnessLevel(0);
			cTransform = new Transform[mTransform.childCount];
			for (int k = 0; k < mTransform.childCount; k++)
			{
				cTransform[k] = mTransform.GetChild(k);
				for (int l = 0; l < cTransform[k].childCount; l++)
				{
					if (l > 0 && l < cTransform[k].childCount - 2)
					{
						cTransform[k].GetChild(l).gameObject.SetActive(false);
					}
				}
			}
			return skin_1_3;
		}
		case 1:
		{
			SetBrightnessLevel(1);
			cTransform = new Transform[mTransform.childCount];
			for (int num9 = 0; num9 < mTransform.childCount; num9++)
			{
				cTransform[num9] = mTransform.GetChild(num9);
				for (int num10 = 0; num10 < cTransform[num9].childCount; num10++)
				{
					if (num10 > 0 && num10 < cTransform[num9].childCount - 2)
					{
						cTransform[num9].GetChild(num10).gameObject.SetActive(true);
						cTransform[num9].GetChild(num10).GetComponent<Renderer>().sharedMaterial = skin_1_3;
						cTransform[num9].GetChild(num10).localScale = new Vector3(1.5f, 1.5f, 1f);
						cTransform[num9].GetChild(num10).localRotation = Quaternion.identity;
						cTransform[num9].GetChild(num10).Rotate(new Vector3(0f, 0f, -num10 * 30));
						cTransform[num9].GetChild(num10).Rotate(new Vector3(-30f, 0f, 0f));
					}
				}
			}
			return skin_1_3;
		}
		case 2:
		{
			SetBrightnessLevel(1);
			cTransform = new Transform[mTransform.childCount];
			for (int m = 0; m < mTransform.childCount; m++)
			{
				cTransform[m] = mTransform.GetChild(m);
				for (int n = 0; n < cTransform[m].childCount; n++)
				{
					if (n > 0 && n < cTransform[m].childCount - 2)
					{
						cTransform[m].GetChild(n).gameObject.SetActive(true);
						cTransform[m].GetChild(n).GetComponent<Renderer>().sharedMaterial = skin_2;
						cTransform[m].GetChild(n).localScale = new Vector3(1.5f, 1.5f, 1f);
						cTransform[m].GetChild(n).localRotation = Quaternion.identity;
						cTransform[m].GetChild(n).Rotate(new Vector3(0f, 0f, -n * 30));
						cTransform[m].GetChild(n).Rotate(new Vector3(0f, 0f, 0f));
					}
				}
			}
			return skin_2;
		}
		case 3:
		{
			SetBrightnessLevel(2);
			cTransform = new Transform[mTransform.childCount];
			for (int num5 = 0; num5 < mTransform.childCount; num5++)
			{
				cTransform[num5] = mTransform.GetChild(num5);
				for (int num6 = 0; num6 < cTransform[num5].childCount; num6++)
				{
					if (num6 > 0 && num6 < cTransform[num5].childCount - 2)
					{
						cTransform[num5].GetChild(num6).gameObject.SetActive(true);
						cTransform[num5].GetChild(num6).GetComponent<Renderer>().sharedMaterial = skin_1_3;
						cTransform[num5].GetChild(num6).localScale = new Vector3(1.7f, 2.7f, 1f);
						cTransform[num5].GetChild(num6).localRotation = Quaternion.identity;
						cTransform[num5].GetChild(num6).Rotate(new Vector3(0f, 0f, -num6 * 30));
						cTransform[num5].GetChild(num6).Rotate(new Vector3(-80f, 0f, 0f));
					}
				}
			}
			return skin_1_3;
		}
		case 4:
		{
			SetBrightnessLevel(2);
			cTransform = new Transform[mTransform.childCount];
			for (int num11 = 0; num11 < mTransform.childCount; num11++)
			{
				cTransform[num11] = mTransform.GetChild(num11);
				for (int num12 = 0; num12 < cTransform[num11].childCount; num12++)
				{
					if (num12 > 0 && num12 < cTransform[num11].childCount - 2)
					{
						cTransform[num11].GetChild(num12).gameObject.SetActive(true);
						cTransform[num11].GetChild(num12).GetComponent<Renderer>().sharedMaterial = skin_4;
						cTransform[num11].GetChild(num12).localScale = new Vector3(2.5f, 2.5f, 1f);
						cTransform[num11].GetChild(num12).localRotation = Quaternion.identity;
						cTransform[num11].GetChild(num12).Rotate(new Vector3(0f, 0f, -num12 * 30));
						cTransform[num11].GetChild(num12).Rotate(new Vector3(-80f, 0f, 0f));
					}
				}
			}
			return skin_4;
		}
		case 5:
		{
			SetBrightnessLevel(3);
			cTransform = new Transform[mTransform.childCount];
			for (int num3 = 0; num3 < mTransform.childCount; num3++)
			{
				cTransform[num3] = mTransform.GetChild(num3);
				for (int num4 = 0; num4 < cTransform[num3].childCount; num4++)
				{
					if (num4 > 0 && num4 < cTransform[num3].childCount - 2)
					{
						cTransform[num3].GetChild(num4).gameObject.SetActive(true);
						cTransform[num3].GetChild(num4).GetComponent<Renderer>().sharedMaterial = skin_5;
						cTransform[num3].GetChild(num4).localScale = new Vector3(0.3f, 2.5f, 1f);
						cTransform[num3].GetChild(num4).localRotation = Quaternion.identity;
						cTransform[num3].GetChild(num4).Rotate(new Vector3(0f, 0f, -num4 * 30));
						cTransform[num3].GetChild(num4).Rotate(new Vector3(-80f, 0f, 0f));
					}
				}
			}
			return skin_5;
		}
		case 6:
		{
			SetBrightnessLevel(2);
			cTransform = new Transform[mTransform.childCount];
			for (int num13 = 0; num13 < mTransform.childCount; num13++)
			{
				cTransform[num13] = mTransform.GetChild(num13);
				for (int num14 = 0; num14 < cTransform[num13].childCount; num14++)
				{
					if (num14 > 0 && num14 < cTransform[num13].childCount - 2)
					{
						cTransform[num13].GetChild(num14).gameObject.SetActive(true);
						cTransform[num13].GetChild(num14).GetComponent<Renderer>().sharedMaterial = skin_6;
						cTransform[num13].GetChild(num14).localScale = new Vector3(1f, 5f, 1f);
						cTransform[num13].GetChild(num14).localRotation = Quaternion.identity;
						cTransform[num13].GetChild(num14).Rotate(new Vector3(0f, 0f, -num14 * 30));
						cTransform[num13].GetChild(num14).Rotate(new Vector3(-80f, 0f, 0f));
					}
				}
			}
			return skin_6;
		}
		case 7:
		{
			SetBrightnessLevel(1);
			cTransform = new Transform[mTransform.childCount];
			for (int num7 = 0; num7 < mTransform.childCount; num7++)
			{
				cTransform[num7] = mTransform.GetChild(num7);
				for (int num8 = 0; num8 < cTransform[num7].childCount; num8++)
				{
					if (num8 > 0 && num8 < cTransform[num7].childCount - 2)
					{
						cTransform[num7].GetChild(num8).gameObject.SetActive(true);
						cTransform[num7].GetChild(num8).GetComponent<Renderer>().sharedMaterial = skin_7;
						cTransform[num7].GetChild(num8).localScale = new Vector3(1.8f, 3f, 1f);
						cTransform[num7].GetChild(num8).localRotation = Quaternion.identity;
						cTransform[num7].GetChild(num8).Rotate(new Vector3(0f, 0f, -num8 * 30));
						cTransform[num7].GetChild(num8).Rotate(new Vector3(-80f, 0f, 0f));
					}
				}
			}
			return skin_7;
		}
		case 8:
		{
			SetBrightnessLevel(3);
			cTransform = new Transform[mTransform.childCount];
			for (int num = 0; num < mTransform.childCount; num++)
			{
				cTransform[num] = mTransform.GetChild(num);
				for (int num2 = 0; num2 < cTransform[num].childCount; num2++)
				{
					if (num2 > 0 && num2 < cTransform[num].childCount - 2)
					{
						cTransform[num].GetChild(num2).gameObject.SetActive(true);
						cTransform[num].GetChild(num2).GetComponent<Renderer>().sharedMaterial = skin_8;
						cTransform[num].GetChild(num2).localScale = new Vector3(1.8f, 1f, 1f);
						cTransform[num].GetChild(num2).localRotation = Quaternion.identity;
						cTransform[num].GetChild(num2).Rotate(new Vector3(0f, 0f, -num2 * 30));
						cTransform[num].GetChild(num2).Rotate(new Vector3(-80f, 0f, 0f));
					}
				}
			}
			return skin_8;
		}
		case 9:
		{
			SetBrightnessLevel(2);
			cTransform = new Transform[mTransform.childCount];
			for (int i = 0; i < mTransform.childCount; i++)
			{
				cTransform[i] = mTransform.GetChild(i);
				for (int j = 0; j < cTransform[i].childCount; j++)
				{
					if (j > 0 && j < cTransform[i].childCount - 2)
					{
						cTransform[i].GetChild(j).gameObject.SetActive(true);
						cTransform[i].GetChild(j).GetComponent<Renderer>().sharedMaterial = skin_9;
						cTransform[i].GetChild(j).localScale = new Vector3(1.8f, 3f, 1f);
						cTransform[i].GetChild(j).localRotation = Quaternion.identity;
						cTransform[i].GetChild(j).Rotate(new Vector3(0f, 0f, -j * 30));
						cTransform[i].GetChild(j).Rotate(new Vector3(-80f, 0f, 0f));
					}
				}
			}
			return skin_9;
		}
		default:
			return skin_1_3;
		}
	}

	public void SetRoad(int _road)
	{
		switch (_road)
		{
		case 1:
			railMat.SetColor("_TintColor", new Color(0.47058824f, 2f / 51f, 1f / 51f));
			roadMat.SetColor("_TintColor", new Color(0.2509804f, 0.11764706f, 0f));
			break;
		case 2:
			railMat.SetColor("_TintColor", new Color(0.47058824f, 2f / 51f, 1f / 51f));
			roadMat.SetColor("_TintColor", new Color(0.11764706f, 0.2509804f, 0f));
			break;
		case 3:
			railMat.SetColor("_TintColor", new Color(0.47058824f, 2f / 51f, 1f / 51f));
			roadMat.SetColor("_TintColor", new Color(18f / 85f, 18f / 85f, 4f / 51f));
			break;
		case 4:
			railMat.SetColor("_TintColor", new Color(0.47058824f, 2f / 51f, 1f / 51f));
			roadMat.SetColor("_TintColor", new Color(0.20784314f, 0.2f, 27f / 85f));
			break;
		case 5:
			railMat.SetColor("_TintColor", new Color(0.47058824f, 2f / 51f, 1f / 51f));
			roadMat.SetColor("_TintColor", new Color(0.1764706f, 0.1764706f, 0.1764706f));
			break;
		}
	}

	public void SetBall(int _ball)
	{
		switch (_ball)
		{
		case 1:
			ballMat.SetColor("_TintColor", new Color(57f / 85f, 44f / 85f, 6f / 85f));
			trailMat.SetColor("_TintColor", new Color(1f / 3f, 22f / 85f, 3f / 85f));
			break;
		case 2:
			ballMat.SetColor("_TintColor", new Color(44f / 85f, 57f / 85f, 6f / 85f));
			trailMat.SetColor("_TintColor", new Color(22f / 85f, 1f / 3f, 3f / 85f));
			break;
		case 3:
			ballMat.SetColor("_TintColor", new Color(76f / 85f, 0.38431373f, 0.11764706f));
			trailMat.SetColor("_TintColor", new Color(38f / 85f, 0.19215687f, 1f / 17f));
			break;
		case 4:
			ballMat.SetColor("_TintColor", new Color(29f / 51f, 29f / 51f, 1f));
			trailMat.SetColor("_TintColor", new Color(24f / 85f, 24f / 85f, 25f / 51f));
			break;
		case 5:
			ballMat.SetColor("_TintColor", new Color(0.61960787f, 0.61960787f, 0.61960787f));
			trailMat.SetColor("_TintColor", new Color(0.30980393f, 0.30980393f, 0.30980393f));
			break;
		}
	}

	private void OnDestroy()
	{
		Instance = null;
		railMat = null;
		roadMat = null;
		ballMat = null;
		trailMat = null;
		skin_1_3 = null;
		skin_2 = null;
		skin_4 = null;
		skin_5 = null;
		skin_6 = null;
		skin_7 = null;
		skin_8 = null;
		skin_9 = null;
	}
}
