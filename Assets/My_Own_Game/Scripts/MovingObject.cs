using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 이 스크립트는 enemy or player에 적용될 스크립트.
// 적의 경우 플레이어로 이동, player일 경우 벽 or enemy에 상호작용
public abstract class MovingObject : MonoBehaviour {

	public float moveTime = 0.1f;
	public LayerMask blockingLayer;

	private BoxCollider2D boxcollider;
	private Rigidbody2D rb2d;
	private float inverseMoveTime;

	protected virtual void Start()
	{
		boxcollider = GetComponent<BoxCollider2D>();
		rb2d = GetComponent<Rigidbody2D>();
		inverseMoveTime = 1f / moveTime;
	}

	/// <summary>
	/// vector end로 갈 수 있는지 없는지 검사하는 함수
	/// </summary>
	/// <param name="xdir"></param>
	/// <param name="ydir"></param>
	/// <param name="hit"></param>
	/// <returns></returns>
	protected bool Move(int xdir, int ydir, out RaycastHit2D hit)
	{
		// out hit >> move는 bool뿐만 아니라 hit도 return하게 되었음
		Vector2 start = transform.position;
		Vector2 end = start + new Vector2(xdir, ydir);
		// 레이캐스트에 자기자신의 충돌체가 검출되면 안되므로 비활성화
		boxcollider.enabled = false;
		hit = Physics2D.Linecast(start, end, blockingLayer);
		boxcollider.enabled = true;

		// 경로상에 충돌할 것이 없음 >> 이동가능
		if (hit.transform == null)
		{
			// 실제로 이동함
			StartCoroutine(SmoothMovement(end));
			return true;
		}
		// 아니라면 이동 불가능
		return false;
	}
	
	protected IEnumerator SmoothMovement(Vector3 end)
	{
		float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

		while (sqrRemainingDistance > float.Epsilon)
		{
			Vector3 newPosition = Vector3.MoveTowards(rb2d.position, end, inverseMoveTime * Time.deltaTime);
			// 오브젝트 위치이동
			// rigidbody의 transform이 이동해도 그 오브젝트의 transform도 같이 이동된다
			rb2d.MovePosition(newPosition);
			// 목표까지의 거리 재계산
			sqrRemainingDistance = (transform.position - end).sqrMagnitude;
			// 다음 프레임까지 기다림
			// 프레임마다 이동할 것임
			yield return null;
		}
	}

	protected abstract void OnCantMove<T>(T component)
		where T : Component;

	protected virtual void AttemptMove<T>(int xdir, int ydir)
		// 이동하면 T종류와 아마 상호작용 할것임
		where T:Component
	{
		RaycastHit2D hit;
		bool canMove = Move(xdir, ydir, out hit);

		if (hit.transform == null)
			return;
		// 광선에 검출된 오브젝트를 갖고옴.
		// 근데 그 오브젝트가 T일 때만 null이 아니게되서 아래 조건이 해당된다.
		T hitcomponent = hit.transform.GetComponent<T>();
		if (!canMove && hitcomponent != null)
			OnCantMove(hitcomponent);
	}
}
