using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

	// 레벨 표시 화면이 지속되는 시간
	public float levelStartDelay = 2f;
	public float turnDelay = 0.1f;
	// singleton
	// 이 게임매니저는 게임플레이 동안 무조건 한개만 존재한다.
	public static GameManager Instance = null;
	public BoardManager boardscript;
	// player의 누적 점수
	// 0으로 떨어지면 게임오버.
	public int playerFoodPoints = 100;
	// public이지만 에디터에는 뜨지 않게 설정
	// 플레이어의 턴인지 아닌지를 저장
	[HideInInspector] public bool playersTurn = true;

	// 레벨 text
	private Text levelText;
	// 레벨 image
	private GameObject levelImage;
	private GameObject GameOverImage;
	private Text ResultText;
	private int level = 0; // test
	private List<Enemy> enemies;
	private bool enemiesmoving;
	// 이게 true일 동안 player는 움직일 수 없음
	private bool doingSetup;

	void Awake () {

		// singleton
		if (Instance == null)
			// 참조형으로 넘겨주는 거겠지?
			Instance = this;
		else if (Instance != this)
			Destroy(gameObject);

		// level이 바뀐 후에도 이 게임매니저는 유지됨.
		DontDestroyOnLoad(gameObject);
		enemies = new List<Enemy>();
		boardscript = GetComponent<BoardManager>();
		//Initgame();
	}

	void OnEnable()
	{
		SceneManager.sceneLoaded += OnLevelFinishedLoading;
	}

	void OnDisable()
	{
		SceneManager.sceneLoaded -= OnLevelFinishedLoading;
	}

	/// <summary>
	/// 씬이 로드된 후에 호출. 애초에 게임이 이 신에서 시작한 경우는 로드된 것으로 치지 않는다.
	/// </summary>
	/// <param name="scene"></param>
	/// <param name="mode"></param>
	void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
	{
		// 레벨 증가시마다 level++
		level++;
		if (level == 1 && playerFoodPoints<0)
		{
			playerFoodPoints = 100;
			SoundManager.instance.musicSource.Play();
		}
		Initgame();
	}

	void Initgame()
	{
		doingSetup = true;
		levelImage = GameObject.Find("LevelImage");
		levelText = GameObject.Find("LevelText").GetComponent<Text>();
		GameOverImage = GameObject.Find("GameOverImage");
		ResultText = GameOverImage.GetComponentInChildren<Text>();
		GameOverImage.SetActive(false);
		levelText.text = "Day " + level;
		levelImage.SetActive(true);
		// levelStartDelay만큼 흐른 후에 레벨안내 이미지 hide
		Invoke("HideLevelImage", levelStartDelay);

		enemies.Clear(); // 매 level 시작때마다 초기화
		boardscript.SetupScene(level);
	}

	private void HideLevelImage()
	{
		levelImage.SetActive(false);
		doingSetup = false; // player가 움직일 수 있게 함
	}
	public void GameOver()
	{
		// 게임매니저 비활성화
		ResultText.text = "After " + level + " days, you survived.";
		GameOverImage.SetActive(true);
		enabled = false;
	}

	public void RestartGame()
	{
		level = 0;
		enabled = true;
		// 아래에서 이전 scene을 파괴하고 신 불러올때 점수가 -가 되어 버린다.
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	void Update()
	{
		// 플레이어 턴이거나 적들이 이동중이거나 씬 구성중이면 종료
		if (playersTurn || enemiesmoving || doingSetup)
			return;

		StartCoroutine(MoveEnemies());
	}

	/// <summary>
	/// 게임매니저에 enemy를 등록하는 함수
	/// </summary>
	/// <param name="script"></param>
	public void AddEnemyToList(Enemy script)
	{
		enemies.Add(script);
	}

	IEnumerator MoveEnemies()
	{
		enemiesmoving = true;
		yield return new WaitForSeconds(turnDelay);
		if (enemies.Count == 0)
			yield return new WaitForSeconds(turnDelay);
		
		for(int i = 0; i < enemies.Count; i++)
		{
			enemies[i].MoveEnemy();
			yield return new WaitForSeconds(enemies[i].moveTime);
		}

		playersTurn = true;
		enemiesmoving = false;

	}
}
