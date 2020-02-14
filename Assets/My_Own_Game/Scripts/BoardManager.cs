using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour {

	// 직렬화 딱지를 붙임으로서 이 클래스는 시리얼라이즈가 가능하게 되었다.
	// 주로 컴퓨터 데이터로 저장할 필요가 있거나 서버로 데이터를 보낼때 시리얼라이즈 시킨다.
	[Serializable]
	public class Count
	{
		public int minimum;
		public int maximum;

		// constructor
		public Count (int min, int max)
		{
			minimum = min;
			maximum = max;
		}

	}

	// default : 8*8
	public int columns = 8;
	public int rows = 8;
	public Count wallcount = new Count(5, 9);
	public Count foodcount = new Count(1, 5);
	public GameObject exit;
	public GameObject[] floorTiles;
	public GameObject[] wallTiles;
	public GameObject[] foodTiles;
	public GameObject[] enemyTiles;
	public GameObject[] outerwallTiles;
	// boardholder는 모든 object들의 부모가 되게 할것임
	// why??
	private Transform boardHolder;
	// 오브젝트가 위치 가능한 모든 위치를 list로 관리한다.
	private List<Vector3> gridPositions = new List<Vector3>();

	/// <summary>
	/// gridpositions에 맵상 모든위치를 vector3 형식으로 넣어주면서 초기화하는 함수
	/// </summary>
	void InitializeList()
	{
		gridPositions.Clear();
		for(int x = 1; x < columns - 1; x++)
		{
			for(int y = 1; y < rows - 1; y++)
			{
				gridPositions.Add(new Vector3(x, y,0f));
			}
		}

	}

	/// <summary>
	/// map을 구현하는 함수(try마다 랜덤으로 생성됨)
	/// </summary>
	void BoardSetup()
	{
		boardHolder = new GameObject("Board").transform;
		for(int x = -1; x <= columns; x++)
		{
			for (int y = -1; y <= rows; y++)
			{
				GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];
				if (x == -1 || x == columns || y == -1 || y == rows)
					toInstantiate = outerwallTiles[Random.Range(0, outerwallTiles.Length)];
				// 어라 if문 한줄도 됬었네
				GameObject Instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;
				Instance.transform.SetParent(boardHolder);
			}
		}
	}

	/// <summary>
	/// 맵상 위치중 하나를 랜덤으로 vector3 형식으로 뽑아옴
	/// </summary>
	/// <returns></returns>
	Vector3 RandomPosition()
	{
		int randomIndex = Random.Range(0, gridPositions.Count);
		Vector3 randomposition = gridPositions[randomIndex];
		// 재사용 방지
		gridPositions.RemoveAt(randomIndex);
		return randomposition;
	}

	/// <summary>
	/// 임의의 위치에 오브젝트를 설치하는 함수
	/// </summary>
	/// <param name="tileArray"></param>
	/// <param name="minimum"></param>
	/// <param name="maximum"></param>
	void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum)
	{
		// range : maximum excluded.
		int objectcount = Random.Range(minimum, maximum+1);
		for(int i = 0; i < objectcount; i++)
		{
			// 임의의 위치 하나 가져옴
			Vector3 randomposition = RandomPosition();
			// 배열에서 임의의 오브젝트 선택해서 가져옴
			GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];
			Instantiate(tileChoice, randomposition, Quaternion.identity);
		}
	}

	// 유일한 public func. gamemanager에서 이 함수를 호출함
	public void SetupScene(int level)
	{
		// level마다 spawn하는 적의 가짓수가 다르다.
		BoardSetup();
		InitializeList();
		LayoutObjectAtRandom(wallTiles, wallcount.minimum, wallcount.maximum);
		LayoutObjectAtRandom(foodTiles, foodcount.minimum, foodcount.maximum);
		int enemycount = (int)Mathf.Log(level, 2f); // 밑이 2인 로그처리
		LayoutObjectAtRandom(enemyTiles, enemycount, enemycount);
		// 마지막으로 exit타일 생성
		// 항상 제일 오른쪽 위로 위치고정
		Instantiate(exit, new Vector3(columns - 1, rows - 1, 0f), Quaternion.identity);
	}
}
