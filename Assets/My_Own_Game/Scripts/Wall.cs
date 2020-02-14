using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour {

	public Sprite dmgSprite;
	public int hp = 4;
	public AudioClip chopSound1;
	public AudioClip chopSound2;


	private SpriteRenderer spriterenderer;

	// Use this for initialization
	void Awake () {
		// 렌더러 캐싱
		spriterenderer = GetComponent<SpriteRenderer>();
	}
	
	/// <summary>
	/// 벽에 데미지를 주고 스프라이트를 바꿔주는 함수
	/// </summary>
	/// <param name="loss"></param>
	public void DamageWall(int loss)
	{
		SoundManager.instance.RandomizeSfx(chopSound1, chopSound2);
		spriterenderer.sprite = dmgSprite;
		hp -= loss;
		if (hp < 0)
			gameObject.SetActive(false);
	}
}
