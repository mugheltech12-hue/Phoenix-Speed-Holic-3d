using System.Collections;
using UnityEngine;

public class GameManager_ModeG : MonoBehaviour
{
	public TextMesh best_score;

	public MainController_ModeG controller;

	public Transform bTransform;

	public TextMesh txt_score;

	public ParticleSystem ps1;

	public ParticleSystem ps2;

	public Transform bgTiles;

	public Transform ps2t;

	public SkinManager TM;

	public float offsetZ;

	public CSEffect cse;

	public float degree;

	public float speedZ;

	public float near;

	private const string ColorName = "_TintColor";

	public Material roadMat;

	private Material bgMat;

	private float delta_R;

	private float delta_G;

	private float delta_B;

	private float dest_R;

	private float dest_G;

	private float dest_B;

	private bool isAniTime;

	private int colorState;

	private float changeSpeed = 2f;

	private RingSetting[] ringSet;

	private Transform[] tile;

	private Transform mTransform;

	private int resetPosCount;

	private int stand_degree;

	private int rand_degree;

	private int length;

	private int phase;

	private bool isAcc;

	private bool over;

	private int score;

	private float zPoint;

	private int index;

	private int variation;

	private int term;

	private int min;

	private int max;

	private int chroma;

	private void Awake()
	{
		switch (PlayerPrefs.GetInt("SPEED"))
		{
		case 0:
			best_score.text = string.Format("{0}", PlayerPrefs.GetInt("BEST_SCORE_G_07x"));
			break;
		case 1:
			best_score.text = string.Format("{0}", PlayerPrefs.GetInt("BEST_SCORE_G_10x"));
			break;
		case 2:
			best_score.text = string.Format("{0}", PlayerPrefs.GetInt("BEST_SCORE_G_13x"));
			break;
		}
		chroma = PlayerPrefs.GetInt("CHROMA");
		bgMat = TM.SetSkin(PlayerPrefs.GetInt("SKIN"));
		colorState = Random.Range(0, 6);
		length = bgTiles.childCount;
		tile = new Transform[length];
		ringSet = new RingSetting[length];
		for (int i = 0; i < bgTiles.childCount; i++)
		{
			tile[i] = bgTiles.GetChild(i);
			ringSet[i] = tile[i].GetComponent<RingSetting>();
		}
		Camera.main.farClipPlane = offsetZ + offsetZ * (float)length;
		mTransform = base.transform;
		stand_degree = 13;
		rand_degree = 1;
		variation = 2;
		term = 15;
		min = 13;
		max = 18;
		phase = 0;
		controller.degree = 150f * (speedZ / 45f);
		StartColorAnimation();
	}

	public void SetIndex(int _idx)
	{
		index = _idx;
	}

	private void LateUpdate()
	{
		mTransform.position += new Vector3(0f, 0f, speedZ * Time.deltaTime);
		ps2t.position += new Vector3(0f, 0f, speedZ * Time.deltaTime);
	}

	private void Update()
	{
		CheckSpeed();
		CheckPhase();
		//CheckColor();
	}

	private void CheckSpeed()
	{
		if (!isAcc)
		{
			return;
		}
		switch (phase)
		{
		case 1:
			if (speedZ < 25f)
			{
				speedZ += 3f * Time.deltaTime;
				if (speedZ > 25f)
				{
					speedZ = 25f;
					isAcc = false;
				}
			}
			break;
		case 2:
			if (speedZ < 35f)
			{
				speedZ += 3f * Time.deltaTime;
				if (speedZ > 35f)
				{
					isAcc = false;
					speedZ = 35f;
					min = 12;
					max = 17;
				}
			}
			break;
		case 3:
			if (speedZ < 45f)
			{
				speedZ += 3f * Time.deltaTime;
				if (speedZ > 45f)
				{
					isAcc = false;
					ps1.Play();
					speedZ = 45f;
					min = 11;
					max = 16;
				}
			}
			break;
		case 4:
			if (speedZ < 55f)
			{
				speedZ += 3f * Time.deltaTime;
				if (speedZ > 55f)
				{
					isAcc = false;
					changeSpeed = 1f;
					ps2.Play();
					speedZ = 55f;
					min = 10;
					max = 15;
				}
			}
			break;
		case 5:
			if (speedZ < 65f)
			{
				speedZ += 3f * Time.deltaTime;
				if (speedZ > 65f)
				{
					isAcc = false;
					StartCoroutine(SpeicalPhaseManager());
					speedZ = 65f;
					min = 9;
					max = 14;
				}
			}
			break;
		}
		controller.degree = 150f * (speedZ / 45f);
	}

	private void CheckPhase()
	{
		if (!(tile[index].position.z < mTransform.position.z + near))
		{
			return;
		}
		zPoint = mTransform.position.z + (float)length * offsetZ + (tile[index].position.z - mTransform.position.z);
		tile[index].position = new Vector3(tile[index].position.x, tile[index].position.y, zPoint);
		stand_degree += rand_degree;
		ringSet[index].Init(stand_degree);
		resetPosCount++;
		index++;
		if (index == length)
		{
			index = 0;
		}
		if (!over)
		{
			score++;
			txt_score.text = string.Format("{0}", score);
		}
		if (isAcc || resetPosCount % term != 0)
		{
			return;
		}
		switch (phase)
		{
		case 0:
			if (score > 80)
			{
				StartColorAnimation();
				rand_degree *= -1;
				isAcc = true;
				phase = 1;
				resetPosCount = 0;
			}
			else
			{
				resetNextPhase();
			}
			break;
		case 1:
			if (score > 300)
			{
				StartColorAnimation();
				rand_degree *= -1;
				isAcc = true;
				phase = 2;
				resetPosCount = 0;
			}
			else
			{
				resetNextPhase();
			}
			break;
		case 2:
			if (score > 500)
			{
				StartColorAnimation();
				rand_degree *= -1;
				isAcc = true;
				phase = 3;
				resetPosCount = 0;
			}
			else
			{
				resetNextPhase();
			}
			break;
		case 3:
			if (score > 850)
			{
				rand_degree *= -1;
				isAcc = true;
				phase = 4;
				changeSpeed = 1f;
				resetPosCount = 0;
			}
			else
			{
				resetNextPhase();
			}
			break;
		case 4:
			if (score > 1250)
			{
				rand_degree *= -1;
				isAcc = true;
				phase = 5;
				resetPosCount = 0;
			}
			else
			{
				resetNextPhase();
			}
			break;
		case 5:
			resetNextPhase();
			break;
		case 6:
			resetNextPhase();
			break;
		}
	}

	private void resetNextPhase()
	{
		term = Random.Range(min, max);
		resetPosCount = 0;
		switch (Random.Range(0, variation))
		{
		case 0:
			rand_degree = 1;
			break;
		case 1:
			rand_degree = -1;
			break;
		}
	}

	private void CheckColor()
	{
		if (isAniTime)
		{
			bgMat.SetColor("_TintColor", new Color(bgMat.GetColor("_TintColor").r + delta_R * Time.deltaTime / changeSpeed, bgMat.GetColor("_TintColor").g + delta_G * Time.deltaTime / changeSpeed, bgMat.GetColor("_TintColor").b + delta_B * Time.deltaTime / changeSpeed));
			if (delta_R > 0f)
			{
				if (bgMat.GetColor("_TintColor").r > dest_R)
				{
					bgMat.SetColor("_TintColor", new Color(dest_R, bgMat.GetColor("_TintColor").g, bgMat.GetColor("_TintColor").b));
				}
			}
			else if (bgMat.GetColor("_TintColor").r < dest_R)
			{
				bgMat.SetColor("_TintColor", new Color(dest_R, bgMat.GetColor("_TintColor").g, bgMat.GetColor("_TintColor").b));
			}
			if (delta_G > 0f)
			{
				if (bgMat.GetColor("_TintColor").g > dest_G)
				{
					bgMat.SetColor("_TintColor", new Color(bgMat.GetColor("_TintColor").r, dest_G, bgMat.GetColor("_TintColor").b));
				}
			}
			else if (bgMat.GetColor("_TintColor").g < dest_G)
			{
				bgMat.SetColor("_TintColor", new Color(bgMat.GetColor("_TintColor").r, dest_G, bgMat.GetColor("_TintColor").b));
			}
			if (delta_B > 0f)
			{
				if (bgMat.GetColor("_TintColor").b > dest_B)
				{
					bgMat.SetColor("_TintColor", new Color(bgMat.GetColor("_TintColor").r, bgMat.GetColor("_TintColor").g, dest_B));
				}
			}
			else if (bgMat.GetColor("_TintColor").b < dest_B)
			{
				bgMat.SetColor("_TintColor", new Color(bgMat.GetColor("_TintColor").r, bgMat.GetColor("_TintColor").g, dest_B));
			}
			if (bgMat.GetColor("_TintColor").r == dest_R && bgMat.GetColor("_TintColor").g == dest_G && bgMat.GetColor("_TintColor").b == dest_B)
			{
				isAniTime = false;
			}
		}
		else if (score % 30 == 0)
		{
			StartColorAnimation();
		}
	}

	private void SetColorAni(int R, int G, int B)
	{
		//isAniTime = true;
		//dest_R = (float)R / 255f;
		//dest_G = (float)G / 255f;
		//dest_B = (float)B / 255f;
		//dest_R *= chroma;
		//dest_G *= chroma;
		//dest_B *= chroma;
		//delta_R = dest_R - bgMat.GetColor("_TintColor").r;
		//delta_G = dest_G - bgMat.GetColor("_TintColor").g;
		//delta_B = dest_B - bgMat.GetColor("_TintColor").b;
	}

	private void StartColorAnimation()
	{
		switch (colorState)
		{
		case 0:
			SetColorAni(5, 5, 16);
			colorState = 1;
			break;
		case 1:
			SetColorAni(0, 13, 13);
			colorState = 2;
			break;
		case 2:
			SetColorAni(16, 5, 5);
			colorState = 3;
			break;
		case 3:
			SetColorAni(13, 13, 0);
			colorState = 4;
			break;
		case 4:
			SetColorAni(0, 10, 16);
			colorState = 5;
			break;
		case 5:
			SetColorAni(13, 0, 13);
			colorState = 0;
			break;
		}
	}

	public void SetOver()
	{
		if (!over)
		{
			//SocialManager.Instance.ReservateReport(score);
			PauseManager.Instance.isGameOver = true;
			bTransform.gameObject.layer = 0;
			bTransform.parent = null;
			StartCoroutine(DelayEnding());
			controller.enabled = false;
			over = true;
		}
	}

	private IEnumerator SpeicalPhaseManager()
	{
		min = 8;
		max = 13;
		yield return new WaitForSeconds(10f);
		min = 8;
		max = 12;
		yield return new WaitForSeconds(10f);
		min = 8;
		max = 11;
	}

	private IEnumerator DelayEnding()
	{
		yield return new WaitForSeconds(0.5f);
		cse.SetAni("IntroScene");
	}
}
