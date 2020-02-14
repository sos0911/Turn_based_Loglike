using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MovingObject {

	public int playerDamage;

	private Animator animator;
	private Transform target; // player의 transform
	private bool skipMove; // enemy의 move를 skip?
	public AudioClip enemyAttack1;
	public AudioClip enemyAttack2;

	protected override void Start()
	{
		GameManager.Instance.AddEnemyToList(this);
		animator = GetComponent<Animator>();
		target = GameObject.FindGameObjectWithTag("Player").transform;
		base.Start();
	}

	/// <summary>
	/// enemy가 움직이는 함수
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="xdir"></param>
	/// <param name="ydir"></param>
	protected override void AttemptMove<T>(int xdir, int ydir)
	{
		// 2번에 한번꼴로 움직이는듯.
		if (skipMove)
		{
			skipMove = false;
			return;
		}
		base.AttemptMove<T>(xdir, ydir);
		skipMove = true;
	}

	public void MoveEnemy()
	{
		int xdir = 0;
		int ydir = 0;

		// player가 점유한 칸은 enemy가 점유 불가능하므로 적어도 y나 x는 차이가 있음
		// enemy는 player와 마찬가지로 대각선 이동이 불가능하다.
		// y의 경우 윗방향이 1, x의 경우 오른쪽방향이 1
		if (Mathf.Abs(target.position.x - transform.position.x) < float.Epsilon)
			ydir = target.position.y > transform.position.y ? 1 : -1;
		else
			xdir = target.position.x > transform.position.x ? 1 : -1;

		AttemptMove<Player>(xdir, ydir);
	}

	/// <summary>
	/// enemy가 player를 만나 이동불가능해서 공격할때 호출됨
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="component"></param>
	protected override void OnCantMove<T>(T component)
	{
		// abstract method 구현
		// T형 component를 player로 받아옴
		Player hitPlayer = component as Player;
		animator.SetTrigger("EnemyAttack");
		SoundManager.instance.RandomizeSfx(enemyAttack1, enemyAttack2);
		hitPlayer.LoseFood(playerDamage);
	}
}
