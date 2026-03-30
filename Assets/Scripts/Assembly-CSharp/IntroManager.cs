using System.Collections;
using UnityEngine;

public class IntroManager : MonoBehaviour
{
	public CSEffect cse;

	public Transform bgTiles;

	public Transform introBall;

	public float offsetZ;

	public float degree;

	public float speedZ;

	public float near;

	private RingSetting[] ringSet;

	private Transform[] tile;

	private int length;

	private Transform mTransform;

	private float zPoint;

	private int ballIndex;

	private int index;

	private void Awake()
	{
		//SocialManager.Instance.CheckReservation();
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
		index = 0;
		ballIndex = index + 4;
		//AdManager.Instance.ShowInter();
	}

	private void Start()
	{
		StartCoroutine(StartingEffect(0.5f));
	}

	private IEnumerator StartingEffect(float _delay)
	{
		cse.enabled = false;
		yield return new WaitForSeconds(_delay);
		cse.enabled = true;
		if (DataManager.Instance.GetIntDB(DataManager.DBNameTag.SOUND) == 0)
		{
			SoundManager.Instance.PauseMusic();
		}
		else
		{
			SoundManager.Instance.PlayMusic();
		}
		yield return new WaitForSeconds(_delay);
	}

	private void LateUpdate()
	{
		mTransform.position += new Vector3(0f, 0f, speedZ * Time.deltaTime);
	}

	private void Update()
	{
		CheckTile();
	}

	private void CheckTile()
	{
		if (tile[index].position.z < mTransform.position.z + near)
		{
			zPoint = mTransform.position.z + (float)length * offsetZ + (tile[index].position.z - mTransform.position.z);
			tile[index].position = new Vector3(tile[index].position.x, tile[index].position.y, zPoint);
			introBall.SendMessage("OnTilePosChanged", ringSet[ballIndex].IndexDegree);
			index++;
			if (index == length)
			{
				index = 0;
			}
			ballIndex++;
			if (ballIndex == length)
			{
				ballIndex = 0;
			}
		}
	}
}
