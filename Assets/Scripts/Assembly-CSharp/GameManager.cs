using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Video;

public class GameManager : MonoBehaviour
{
    public TrailRenderer[] trails;

    public TextMesh best_score;

    public MainController controller;

    public Transform bTransform;

    public Transform MainTransfrom;

    public Transform cTransform;

    public TextMesh txt_score;

    public ParticleSystem ps1;

    public ParticleSystem ps2;

    public Transform bgTiles;

    public GameObject[] Cubes;

    public Transform ps2t;

    public SkinManager TM;

    public float offsetZ;

    public CSEffect cse;

    public float degree;

    public float speedZ;

    public float near;

    public VideoPlayer gameOverAnimation;

    private const string ColorName = "_TintColor";

    public Material roadMat;

    //private Material bgMat;

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

    public Transform mTransform;

    private int resetPosCount;

    private int stand_degree;

    private int rand_degree;

    private int length;

    private int phase;

    private bool isAcc;

    private bool over;

    public int score;

    private float zPoint;

    private int index;

    private int variation;

    private int term;

    private int min;

    private int max;

    private int chroma;


    private int savedIndexAtDeath;

    private Vector3 lastSafeRingPosition;
    private float lastSafeRingRotZ;

    // Saved state variables
    private Vector3 savedMTransformEuler;
    private Vector3 savedCamLocalEuler;
    private Vector3 savedChdLocalEuler;
    private float savedMTransformZ;


    public static GameManager Instance;

    // Add these below your existing private fields
    private Vector3[] initialTilePositions;
    private Vector3 initialMTransformPos;
    private Quaternion initialMTransformRot;
    public float initialSpeedZ;



    [Header("Timer Settings")]
    private float currentTime = 0f;
    private bool isRunning = true; // Auto start

    private GUIStyle timerStyle;
    private GUIStyle hintStyle;





    private void Awake()
    {
        switch (PlayerPrefs.GetInt("SPEED"))
        {
            case 0:
                best_score.text = string.Format("{0}", PlayerPrefs.GetInt("BEST_SCORE_T_07x"));
                break;
            case 1:
                best_score.text = string.Format("{0}", PlayerPrefs.GetInt("BEST_SCORE_T_10x"));
                break;
            case 2:
                best_score.text = string.Format("{0}", PlayerPrefs.GetInt("BEST_SCORE_T_13x"));
                break;
        }
        chroma = PlayerPrefs.GetInt("CHROMA");
        //bgMat = TM.SetSkin(PlayerPrefs.GetInt("SKIN"));
        colorState = Random.Range(0, 6);
        length = bgTiles.childCount;
        tile = new Transform[length];
        ringSet = new RingSetting[length];
        for (int i = 0; i < bgTiles.childCount; i++)
        {
            tile[i] = bgTiles.GetChild(i);
            ringSet[i] = tile[i].GetComponent<RingSetting>();
        }
        // In Awake(), after the for loop that sets up tile[] and ringSet[]
        initialTilePositions = new Vector3[length];
        for (int i = 0; i < length; i++)
            initialTilePositions[i] = tile[i].position;

        initialMTransformPos = transform.position;
        initialMTransformRot = transform.rotation;
        initialSpeedZ = speedZ;
        Instance = this;

        Camera.main.farClipPlane = offsetZ + offsetZ * (float)length + 100;
        mTransform = base.transform;
        stand_degree = 13;
        rand_degree = 0;
        variation = 2;
        term = 15;
        min = 5;
        max = 10;
        phase = 0;
        controller.degree = 150f * (speedZ / 45f);
        StartColorAnimation();



    }


    private void Start()
    {
        ps1.Play();
        ps2.Play();


        TicketManager.Instance.FetchTicketBalance();
        TicketManager.Instance.LoadFreeTicketTimer();


        Debug.Log("START - mTransform pos: " + mTransform.position);
        Debug.Log("START - mTransform rot: " + mTransform.eulerAngles);
        Debug.Log("START - MainTransfrom pos: " + MainTransfrom.position);
        Debug.Log("START - MainTransfrom rot: " + MainTransfrom.eulerAngles);
        Debug.Log("START - bTransform local pos: " + bTransform.localPosition);
        Debug.Log("START - cTransform local pos: " + cTransform.localPosition);
        Debug.Log("START - cTransform local rot: " + cTransform.localEulerAngles);
        Debug.Log("START - camTransform local rot: " + Camera.main.transform.localEulerAngles);

        timerStyle = new GUIStyle();
        timerStyle.fontSize = 48;
        timerStyle.fontStyle = FontStyle.Bold;
        timerStyle.normal.textColor = Color.white;
        timerStyle.alignment = TextAnchor.MiddleCenter;

        hintStyle = new GUIStyle();
        hintStyle.fontSize = 14;
        hintStyle.normal.textColor = new Color(1, 1, 1, 0.6f);
        hintStyle.alignment = TextAnchor.MiddleCenter;




    }

    public void ResetGame()
    {
        // Reset all ring tiles to their original positions
        for (int i = 0; i < length; i++)
        {
            tile[i].position = initialTilePositions[i];
            ringSet[i].Init(13); // 13 = stand_degree default
        }

        // Reset all game state variables
        score = 0;
        index = 0;
        phase = 0;
        isAcc = false;
        over = false;
        resetPosCount = 0;
        stand_degree = 13;
        rand_degree = 0;
        variation = 2;
        term = 15;
        min = 5;
        max = 10;
        speedZ = 0f; // PlayGame() will restore this

        ps1.Stop(); ps1.Clear();
        ps2.Stop(); ps2.Clear();

        // ✅ ps2t position bhi reset karo warna yeh peeche reh jaata hai
        ps2t.position = initialMTransformPos;

        // Reset score text
        txt_score.text = "0";

        // Reset transforms back to scene-start positions
        mTransform.position = initialMTransformPos;
        mTransform.rotation = initialMTransformRot;

        MainTransfrom.localPosition = new Vector3(0f, -2f, 9f);
        MainTransfrom.localRotation = Quaternion.identity;

        // Re-attach bird
        bTransform.parent = MainTransfrom;
        bTransform.gameObject.layer = 8;
        bTransform.localPosition = new Vector3(0f, 0.232f, 0f);
        bTransform.localRotation = Quaternion.identity;

        // Reset camera
        cTransform.localPosition = new Vector3(0f, 2.49f, -6.45f);
        cTransform.localEulerAngles = new Vector3(5f, 0f, 0f);

        // Reset controller input state
        controller.ResetRotationState();
        controller.enabled = true;

        // Restart color animation from beginning
        colorState = 0;
        StartColorAnimation();
    }

    public void SetContollerDegree()
    {
        controller.degree = 150f * (speedZ / 45f);

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
        CheckColor();


        if (isRunning)
            currentTime += Time.deltaTime; // 0 se shuru, unlimited upar jata hai

        // Space = Pause / Resume
        if (Input.GetKeyDown(KeyCode.Space))
            isRunning = !isRunning;

        // R = Reset (wapis 0 par)
        if (Input.GetKeyDown(KeyCode.R))
        {
            currentTime = 0f;
            isRunning = true;
        }
    }

    private void ChangeTrail()
    {
        for (int i = 0; i < trails.Length; i++)
        {
            trails[i].time = 0.3f * (15f / speedZ);
        }
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
                ChangeTrail();
                break;
            case 2:
                if (speedZ < 35f)
                {
                    speedZ += 3f * Time.deltaTime;
                    if (speedZ > 35f)
                    {
                        speedZ = 35f;
                        isAcc = false;
                    }
                }
                ChangeTrail();
                break;
            case 3:
                if (speedZ < 45f)
                {
                    speedZ += 3f * Time.deltaTime;
                    if (speedZ > 45f)
                    {
                        ps1.Play();
                        speedZ = 45f;
                        isAcc = false;
                    }
                }
                ChangeTrail();
                break;
            case 4:
                if (speedZ < 55f)
                {
                    speedZ += 3f * Time.deltaTime;
                    if (speedZ > 55f)
                    {
                        changeSpeed = 1f;
                        ps2.Play();
                        speedZ = 55f;
                        isAcc = false;
                    }
                }
                ChangeTrail();
                break;
            case 5:
                if (speedZ < 65f)
                {
                    speedZ += 3f * Time.deltaTime;
                    if (speedZ > 65f)
                    {
                        StartCoroutine(SpeicalPhaseManager());
                        speedZ = 65f;
                        isAcc = false;
                    }
                }
                ChangeTrail();
                break;
            case 6:
                StartCoroutine(WaitPhase8());
                phase = 7;
                break;
            case 8:
                if (cTransform.localPosition.z < 4f)
                {
                    cTransform.localPosition += new Vector3(0f, -0.875f * Time.deltaTime, 2f * Time.deltaTime);
                    if (cTransform.localPosition.z > 4f)
                    {
                        cTransform.localPosition = new Vector3(0f, -0.5f, 4f);
                        StartCoroutine(DelayOffAcc());
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
            savedMTransformEuler = mTransform.eulerAngles;
            savedCamLocalEuler = cTransform.localEulerAngles;
            savedChdLocalEuler = controller.GetChdLocalEuler();
            savedMTransformZ = mTransform.position.z;

            score++;
            txt_score.text = string.Format("{0}", score);

            if (TournamentManager.Instance != null && TournamentManager.Instance.isInTournament)
                TournamentManager.Instance.UpdateScore(score);
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
                    rand_degree = 0;
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
                    rand_degree = 0;
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
                    rand_degree = 0;
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
                    rand_degree = 0;
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
                    rand_degree = 0;
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
            case 7:
                resetNextPhase();
                break;
            case 8:
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
            case 2:
                rand_degree = 1;
                break;
            case 3:
                rand_degree = -1;
                break;
            case 4:
                rand_degree = 0;
                break;
        }
    }

    private void CheckColor()
    {
        //if (isAniTime)
        //{
        //	bgMat.SetColor("_TintColor", new Color(bgMat.GetColor("_TintColor").r + delta_R * Time.deltaTime / changeSpeed, bgMat.GetColor("_TintColor").g + delta_G * Time.deltaTime / changeSpeed, bgMat.GetColor("_TintColor").b + delta_B * Time.deltaTime / changeSpeed));
        //	if (delta_R > 0f)
        //	{
        //		if (bgMat.GetColor("_TintColor").r > dest_R)
        //		{
        //			bgMat.SetColor("_TintColor", new Color(dest_R, bgMat.GetColor("_TintColor").g, bgMat.GetColor("_TintColor").b));
        //		}
        //	}
        //	else if (bgMat.GetColor("_TintColor").r < dest_R)
        //	{
        //		bgMat.SetColor("_TintColor", new Color(dest_R, bgMat.GetColor("_TintColor").g, bgMat.GetColor("_TintColor").b));
        //	}
        //	if (delta_G > 0f)
        //	{
        //		if (bgMat.GetColor("_TintColor").g > dest_G)
        //		{
        //			bgMat.SetColor("_TintColor", new Color(bgMat.GetColor("_TintColor").r, dest_G, bgMat.GetColor("_TintColor").b));
        //		}
        //	}
        //	else if (bgMat.GetColor("_TintColor").g < dest_G)
        //	{
        //		bgMat.SetColor("_TintColor", new Color(bgMat.GetColor("_TintColor").r, dest_G, bgMat.GetColor("_TintColor").b));
        //	}
        //	if (delta_B > 0f)
        //	{
        //		if (bgMat.GetColor("_TintColor").b > dest_B)
        //		{
        //			bgMat.SetColor("_TintColor", new Color(bgMat.GetColor("_TintColor").r, bgMat.GetColor("_TintColor").g, dest_B));
        //		}
        //	}
        //	else if (bgMat.GetColor("_TintColor").b < dest_B)
        //	{
        //		bgMat.SetColor("_TintColor", new Color(bgMat.GetColor("_TintColor").r, bgMat.GetColor("_TintColor").g, dest_B));
        //	}
        //	if (bgMat.GetColor("_TintColor").r == dest_R && bgMat.GetColor("_TintColor").g == dest_G && bgMat.GetColor("_TintColor").b == dest_B)
        //	{
        //		isAniTime = false;
        //	}
        //}
        //else if (score % 30 == 0)
        //{
        //	StartColorAnimation();
        //}
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
            PausingManager.Instance.PauseGameOver();
            bTransform.gameObject.layer = 0;
            bTransform.parent = null;
            StartCoroutine(DelayEnding());
            controller.enabled = false;
            over = true;


            savedIndexAtDeath = index;

            if (TournamentManager.Instance != null && TournamentManager.Instance.isInTournament)
            {
                TournamentManager.Instance.UpdateScore(score);
                TournamentManager.Instance.OnGameFailed();
            }
        }
    }

    // ✅ Continue ke liye — over reset karo


    // ✅ GameManager.cs — sirf ResetForContinue() replace karo

    public void ResetForContinue()
    {
        over = false;

        // Current ring ka IndexDegree se exact center rotation nikalo
        int ringDegree = ringSet[savedIndexAtDeath].IndexDegree;

        float multiplier = (score >= 40 && Time.time >= 11f) ? 10f : 0f;

        float centeredZ = (ringDegree - 13) * multiplier;

        // mTransform ko ring center ke mutabiq set karo
        mTransform.position = new Vector3(0f, 0f, mTransform.position.z);
        mTransform.rotation = Quaternion.Euler(0f, 0f, centeredZ);

        // MainTransfrom game-start position
        MainTransfrom.localPosition = new Vector3(0f, -2f, 9f);
        MainTransfrom.localRotation = Quaternion.identity;

        // Bird center mein
        bTransform.parent = MainTransfrom;
        bTransform.gameObject.layer = 8;
        bTransform.localPosition = new Vector3(0f, 0.232f, 0f);
        bTransform.localRotation = Quaternion.identity;

        // Camera — mTransform ke opposite hamesha
        cTransform.localPosition = new Vector3(0f, 2.49f, -6.45f);
        cTransform.localEulerAngles = new Vector3(5f, 0f, -centeredZ);

        // Controller reset with centered values
        controller.ResetToCenter(centeredZ);
        controller.enabled = true;
    }
    //public void ResetForContinue()
    //{
    //    over = false;
    //    controller.enabled = true;
    //    bTransform.parent = mTransform;
    //    bTransform.gameObject.layer = 8; // tumhara original layer
    //}

    private IEnumerator SpeicalPhaseManager()
    {
        min = 12;
        max = 15;
        yield return new WaitForSeconds(15f);
        min = 7;
        max = 10;
        yield return new WaitForSeconds(15f);
        resetPosCount = 0;
        rand_degree = 0;
        isAcc = true;
        phase = 6;
        yield return new WaitForSeconds(15f);
        variation = 5;
    }

    private IEnumerator WaitPhase8()
    {
        yield return new WaitForSeconds(1f);
        phase = 8;
    }

    private IEnumerator DelayOffAcc()
    {
        yield return new WaitForSeconds(2f);
        isAcc = false;
    }

    private IEnumerator DelayEnding()
    {
        yield return new WaitForSeconds(1f);
        //cse.SetAni("GameScene_ModeT");
    }


    // ✅ Yahan koi Canvas nahi — seedha screen par print hota hai
    //void OnGUI()
    //{
    //    // Background
    //    GUI.color = new Color(0, 0, 0, 0.5f);
    //    GUI.Box(new Rect(Screen.width / 2 - 110, 20, 220, 70), "");
    //    GUI.color = Color.white;

    //    // MM:SS format
    //    int minutes = Mathf.FloorToInt(currentTime / 60f);
    //    int seconds = Mathf.FloorToInt(currentTime % 60f);
    //    string timeText = string.Format("{0:00}:{1:00}", minutes, seconds);

    //    GUI.Label(new Rect(Screen.width / 2 - 110, 25, 220, 65), timeText, timerStyle);

    //    // Hint
    //    string hint = isRunning ? "SPACE = Pause  |  R = Reset" : "SPACE = Resume  |  R = Reset";
    //    GUI.Label(new Rect(Screen.width / 2 - 150, 90, 300, 25), hint, hintStyle);
    //}
}