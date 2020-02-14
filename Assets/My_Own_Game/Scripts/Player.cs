using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MovingObject {

	public int wallDamage = 1;
	public int pointsperFood = 10;
	public int pointsperSoda = 20;
	public float restartLevelDelay = 1f;
	public Text foodText;
	public AudioClip moveSound1;
	public AudioClip moveSound2;
	public AudioClip eatSound1;
	public AudioClip eatSound2;
	public AudioClip drinkSound1;
	public AudioClip drinkSound2;
	public AudioClip gameoverSound;


	private Animator animator;
	private int food;
	// 화면 밖의 터치로 간주하고 초기값 세팅
	private Vector2 touchOrigin = -Vector2.one;



	// Use this for initialization
	protected override void Start () {
		// player의 animator를 caching
		animator = GetComponent<Animator>();
		food = GameManager.Instance.playerFoodPoints;
		// movingobject.start()
		foodText.text = "Food: " + food;
		base.Start();
	}
	/// <summary>
	/// player가 disable될때 호출되는 함수
	/// </summary>
	private void OnDisable()
	{
		GameManager.Instance.playerFoodPoints = food;
	}
	
	// Update is called once per frame
	void Update () {
		if (!GameManager.Instance.playersTurn)
			return;

		int horizontal = 0;
		int vertical = 0;

		// unityeditor를 if문에 포함시키면 스트리밍형식으로 폰에 전달되는 것이기 때문에
		// 결국 play버튼을 누르면 editor에서 실행시킨다고 인식된다.
		// 주의!
#if UNITY_STANDALONE || UNITY_WEBPLAYER || UNITY_EDITOR
		horizontal = (int)Input.GetAxisRaw("Horizontal");
		vertical = (int)Input.GetAxisRaw("Vertical");

		// 대각선 이동 방지. 두 축이 다 0이 아닐시 하나 소거
		if (horizontal != 0)
			vertical = 0;

#else
		if (Input.touchCount > 0)
		{
			Touch mytouch = Input.touches[0];
			if (mytouch.phase == TouchPhase.Began)
				touchOrigin = mytouch.position;
			else if (mytouch.phase == TouchPhase.Ended && touchOrigin.x >= 0)
			{
				Vector2 touchEnd = mytouch.position;
				float x = touchEnd.x - touchOrigin.x;
				float y = touchEnd.y - touchOrigin.y;
				touchOrigin.x = -1; // 반복방지
				if (Mathf.Abs(x) > Mathf.Abs(y))
					horizontal = x > 0 ? 1 : -1;
				else
					vertical = y > 0 ? 1 : -1;
			}
		}

#endif

		if (horizontal != 0 || vertical != 0)
			AttemptMove<Wall>(horizontal, vertical);
	}

	protected override void AttemptMove<T>(int xdir, int ydir)
	{
		food--;
		foodText.text = "Food: " + food;
		base.AttemptMove<T>(xdir, ydir);
		RaycastHit2D hit;

		if(Move(xdir,ydir, out hit))
		{
			SoundManager.instance.RandomizeSfx(moveSound1, moveSound2);
		}

		CheckIfGameOver();
		// 플레이어 턴 종료
		GameManager.Instance.playersTurn = false;
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "Exit")
		{
			// restartLevelDelay 만큼 후에 restart() 호출됨
			Invoke("Restart", restartLevelDelay);
			enabled = false;
		}
		else if (other.tag == "Food")
		{
			food += pointsperFood;
			foodText.text = "+" + pointsperFood + " Food :" + food;
			SoundManager.instance.RandomizeSfx(eatSound1, eatSound2);
			other.gameObject.SetActive(false);
		}
		else if (other.tag == "Soda")
		{
			food += pointsperSoda;
			foodText.text = "+" + pointsperSoda + " Food :" + food;
			SoundManager.instance.RandomizeSfx(drinkSound1, drinkSound2);
			other.gameObject.SetActive(false);
		}
	}
	protected override void OnCantMove<T>(T component)
	{
		Wall hitWall = component as Wall;
		hitWall.DamageWall(wallDamage);
		animator.SetTrigger("PlayerChop");
	}

	private void Restart()
	{
		// 씬 다시 활성화
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	/// <summary>
	/// 적에게 맞았을때 호출
	/// </summary>
	/// <param name="loss"></param>
	public void LoseFood(int loss)
	{
		animator.SetTrigger("PlayerHit");
		food -= loss;
		foodText.text = "-" + loss + " Food :" + food;
		CheckIfGameOver();
	}

	private void CheckIfGameOver()
	{
		if (food < 0)
		{
			SoundManager.instance.PlaySingle(gameoverSound);
			SoundManager.instance.musicSource.Stop();
			GameManager.Instance.GameOver();
		}
	}
}
