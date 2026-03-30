//using SA.Common.Pattern;
using UnityEngine;

public class SocialManager : MonoBehaviour
{
	public static SocialManager Instance;

	private const string achivments1_bronze = "CgkIlZ-pk4EGEAIQAQ";

	private const string achivments1_silver = "CgkIlZ-pk4EGEAIQAg";

	private const string achivments1_gold = "CgkIlZ-pk4EGEAIQAw";

	private const string achivments1_platinum = "CgkIlZ-pk4EGEAIQBA";

	private const string achivments1_diamond = "CgkIlZ-pk4EGEAIQBQ";

	private const string achivments2_bronze = "CgkIlZ-pk4EGEAIQCA";

	private const string achivments2_silver = "CgkIlZ-pk4EGEAIQCQ";

	private const string achivments2_gold = "CgkIlZ-pk4EGEAIQCg";

	private const string achivments2_platinum = "CgkIlZ-pk4EGEAIQCw";

	private const string achivments2_diamond = "CgkIlZ-pk4EGEAIQDA";

	private const string leaderboard_touch_07 = "CgkIlZ-pk4EGEAIQDQ";

	private const string leaderboard_touch_10 = "CgkIlZ-pk4EGEAIQBg";

	private const string leaderboard_touch_13 = "CgkIlZ-pk4EGEAIQDg";

	private const string leaderboard_gyro_07 = "CgkIlZ-pk4EGEAIQDw";

	private const string leaderboard_gyro_10 = "CgkIlZ-pk4EGEAIQBw";

	private const string leaderboard_gyro_13 = "CgkIlZ-pk4EGEAIQEA";

	private const int LimitScore = 999999;

	private const int Bronze = 300;

	private const int Silver = 500;

	private const int Gold = 700;

	private const int Platinum = 1200;

	private const int Diamond = 2500;

	private bool isTargetLeaderboard;

	private bool showAfterConnect;

	private bool Reservated;

	private int score;

	//private void OnDestroy()
	//{
	//	Instance = null;
	//}

	//private void Awake()
	//{
	//	GooglePlayConnection.ActionConnectionResultReceived += ActionConnectionResultReceived;
	//	Object.DontDestroyOnLoad(base.gameObject);
	//	Instance = this;
	//}

	//private void Start()
	//{
	//	switch (GooglePlayConnection.State)
	//	{
	//	case GPConnectionState.STATE_CONNECTED:
	//		break;
	//	case GPConnectionState.STATE_DISCONNECTED:
	//		Singleton<GooglePlayConnection>.Instance.Connect();
	//		break;
	//	case GPConnectionState.STATE_UNCONFIGURED:
	//		Singleton<GooglePlayConnection>.Instance.Connect();
	//		break;
	//	case GPConnectionState.STATE_CONNECTING:
	//		break;
	//	}
	//}

	//private void ActionConnectionResultReceived(GooglePlayConnectionResult result)
	//{
	//	if (result.IsSuccess && showAfterConnect)
	//	{
	//		showAfterConnect = false;
	//		if (isTargetLeaderboard)
	//		{
	//			mShowLeaderboard();
	//		}
	//		else
	//		{
	//			mShowAchievements();
	//		}
	//	}
	//}

	//public void mShowLeaderboard()
	//{
	//	isTargetLeaderboard = true;
	//	switch (GooglePlayConnection.State)
	//	{
	//	case GPConnectionState.STATE_CONNECTED:
	//		showLeaderBoardsUI();
	//		break;
	//	case GPConnectionState.STATE_DISCONNECTED:
	//		showAfterConnect = true;
	//		Singleton<GooglePlayConnection>.Instance.Connect();
	//		break;
	//	case GPConnectionState.STATE_UNCONFIGURED:
	//		showAfterConnect = true;
	//		Singleton<GooglePlayConnection>.Instance.Connect();
	//		break;
	//	case GPConnectionState.STATE_CONNECTING:
	//		TGUI_Message.Instance.StartPopUp("please try again after a few seconds ...");
	//		break;
	//	}
	//}

	//public void mShowAchievements()
	//{
	//	isTargetLeaderboard = false;
	//	switch (GooglePlayConnection.State)
	//	{
	//	case GPConnectionState.STATE_CONNECTED:
	//		showAchivmentsUI();
	//		break;
	//	case GPConnectionState.STATE_DISCONNECTED:
	//		showAfterConnect = true;
	//		Singleton<GooglePlayConnection>.Instance.Connect();
	//		break;
	//	case GPConnectionState.STATE_UNCONFIGURED:
	//		showAfterConnect = true;
	//		Singleton<GooglePlayConnection>.Instance.Connect();
	//		break;
	//	case GPConnectionState.STATE_CONNECTING:
	//		TGUI_Message.Instance.StartPopUp("please try again after a few seconds ...");
	//		break;
	//	}
	//}

	//private void showLeaderBoardsUI()
	//{
	//	Singleton<GooglePlayManager>.Instance.ShowLeaderBoardsUI();
	//}

	//private void showAchivmentsUI()
	//{
	//	Singleton<GooglePlayManager>.Instance.ShowAchievementsUI();
	//}

	//public void ReservateReport(int _score)
	//{
	//	int intDB = DataManager.Instance.GetIntDB(DataManager.DBNameTag.MODE);
	//	int intDB2 = DataManager.Instance.GetIntDB(DataManager.DBNameTag.SPEED);
	//	Reservated = true;
	//	score = _score;
	//	if (score > 999999)
	//	{
	//		score = 999999;
	//	}
	//	switch (intDB)
	//	{
	//	case 1:
	//		switch (intDB2)
	//		{
	//		case 0:
	//			if (DataManager.Instance.GetIntDB(DataManager.DBNameTag.BEST_SCORE_T_07x) < score)
	//			{
	//				DataManager.Instance.SetIntDB(DataManager.DBNameTag.BEST_SCORE_T_07x, score);
	//			}
	//			break;
	//		case 1:
	//			if (DataManager.Instance.GetIntDB(DataManager.DBNameTag.BEST_SCORE_T_10x) < score)
	//			{
	//				DataManager.Instance.SetIntDB(DataManager.DBNameTag.BEST_SCORE_T_10x, score);
	//			}
	//			break;
	//		case 2:
	//			if (DataManager.Instance.GetIntDB(DataManager.DBNameTag.BEST_SCORE_T_13x) < score)
	//			{
	//				DataManager.Instance.SetIntDB(DataManager.DBNameTag.BEST_SCORE_T_13x, score);
	//			}
	//			break;
	//		}
	//		break;
	//	case 2:
	//		switch (intDB2)
	//		{
	//		case 0:
	//			if (DataManager.Instance.GetIntDB(DataManager.DBNameTag.BEST_SCORE_G_07x) < score)
	//			{
	//				DataManager.Instance.SetIntDB(DataManager.DBNameTag.BEST_SCORE_G_07x, score);
	//			}
	//			break;
	//		case 1:
	//			if (DataManager.Instance.GetIntDB(DataManager.DBNameTag.BEST_SCORE_G_10x) < score)
	//			{
	//				DataManager.Instance.SetIntDB(DataManager.DBNameTag.BEST_SCORE_G_10x, score);
	//			}
	//			break;
	//		case 2:
	//			if (DataManager.Instance.GetIntDB(DataManager.DBNameTag.BEST_SCORE_G_13x) < score)
	//			{
	//				DataManager.Instance.SetIntDB(DataManager.DBNameTag.BEST_SCORE_G_13x, score);
	//			}
	//			break;
	//		}
	//		break;
	//	}
	//}

	//public void CheckReservation()
	//{
	//	if (Reservated)
	//	{
	//		Reservated = false;
	//		mReportScore();
	//	}
	//}

	//private void mReportScore()
	//{
	//	if (GooglePlayConnection.State != GPConnectionState.STATE_CONNECTED)
	//	{
	//		return;
	//	}
	//	int intDB = DataManager.Instance.GetIntDB(DataManager.DBNameTag.MODE);
	//	int intDB2 = DataManager.Instance.GetIntDB(DataManager.DBNameTag.SPEED);
	//	switch (intDB)
	//	{
	//	case 1:
	//		switch (intDB2)
	//		{
	//		case 0:
	//			Singleton<GooglePlayManager>.Instance.SubmitScoreById("CgkIlZ-pk4EGEAIQDQ", score, string.Empty);
	//			break;
	//		case 1:
	//			Singleton<GooglePlayManager>.Instance.SubmitScoreById("CgkIlZ-pk4EGEAIQBg", score, string.Empty);
	//			break;
	//		case 2:
	//			Singleton<GooglePlayManager>.Instance.SubmitScoreById("CgkIlZ-pk4EGEAIQDg", score, string.Empty);
	//			break;
	//		}
	//		if (score < 300)
	//		{
	//			break;
	//		}
	//		Singleton<GooglePlayManager>.Instance.UnlockAchievementById("CgkIlZ-pk4EGEAIQAQ");
	//		if (score < 500)
	//		{
	//			break;
	//		}
	//		Singleton<GooglePlayManager>.Instance.UnlockAchievementById("CgkIlZ-pk4EGEAIQAg");
	//		if (score < 700)
	//		{
	//			break;
	//		}
	//		Singleton<GooglePlayManager>.Instance.UnlockAchievementById("CgkIlZ-pk4EGEAIQAw");
	//		if (score >= 1200)
	//		{
	//			Singleton<GooglePlayManager>.Instance.UnlockAchievementById("CgkIlZ-pk4EGEAIQBA");
	//			if (score >= 2500)
	//			{
	//				Singleton<GooglePlayManager>.Instance.UnlockAchievementById("CgkIlZ-pk4EGEAIQBQ");
	//			}
	//		}
	//		break;
	//	case 2:
	//		switch (intDB2)
	//		{
	//		case 0:
	//			Singleton<GooglePlayManager>.Instance.SubmitScoreById("CgkIlZ-pk4EGEAIQDw", score, string.Empty);
	//			break;
	//		case 1:
	//			Singleton<GooglePlayManager>.Instance.SubmitScoreById("CgkIlZ-pk4EGEAIQBw", score, string.Empty);
	//			break;
	//		case 2:
	//			Singleton<GooglePlayManager>.Instance.SubmitScoreById("CgkIlZ-pk4EGEAIQEA", score, string.Empty);
	//			break;
	//		}
	//		if (score < 300)
	//		{
	//			break;
	//		}
	//		Singleton<GooglePlayManager>.Instance.UnlockAchievementById("CgkIlZ-pk4EGEAIQCA");
	//		if (score < 500)
	//		{
	//			break;
	//		}
	//		Singleton<GooglePlayManager>.Instance.UnlockAchievementById("CgkIlZ-pk4EGEAIQCQ");
	//		if (score < 700)
	//		{
	//			break;
	//		}
	//		Singleton<GooglePlayManager>.Instance.UnlockAchievementById("CgkIlZ-pk4EGEAIQCg");
	//		if (score >= 1200)
	//		{
	//			Singleton<GooglePlayManager>.Instance.UnlockAchievementById("CgkIlZ-pk4EGEAIQCw");
	//			if (score >= 2500)
	//			{
	//				Singleton<GooglePlayManager>.Instance.UnlockAchievementById("CgkIlZ-pk4EGEAIQDA");
	//			}
	//		}
	//		break;
	//	}
	//}
}
