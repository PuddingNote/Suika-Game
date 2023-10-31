using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject circlePrefab;
    public Transform circleGroup;
    public List<Circle> circlePool;

    public GameObject effectPrefab;
    public Transform effectGroup;
    public List<ParticleSystem> effectPool;

    public Circle lastCircle;
    [Range(1, 30)]
    public int poolSize;            // 오브젝트풀링 Size
    public int poolCursor;          // 오브젝트풀링 Cursor

    public int score;               // 점수
    public int maxLevel;            // Circle 최대 레벨
    public bool isGameOver;         // GameOver 판단

    void Awake()
    {
        Application.targetFrameRate = 60;   // 프레임 설정
        circlePool = new List<Circle>();
        effectPool = new List<ParticleSystem>();

        for (int index = 0; index < poolSize; index++)
        {
            MakeCircle();
        }
    }

    void Start()
    {
        NextCircle();
    }

    Circle MakeCircle()
    {
        // Effect 생성
        GameObject instanceEffectObj = Instantiate(effectPrefab, effectGroup);
        instanceEffectObj.name = "Effect " + effectPool.Count;
        ParticleSystem instanceEffect = instanceEffectObj.GetComponent<ParticleSystem>();
        effectPool.Add(instanceEffect);

        // Circle 생성
        GameObject instanceCircleObj = Instantiate(circlePrefab, circleGroup);
        instanceEffectObj.name = "Circle " + circlePool.Count;
        Circle instanceCircle = instanceCircleObj.GetComponent<Circle>();
        instanceCircle.gameManager = this;
        instanceCircle.effect = instanceEffect;
        circlePool.Add(instanceCircle);

        return instanceCircle;
    }

    Circle GetCircle()
    {
        for (int index = 0; index < circlePool.Count; index++)
        {
            poolCursor = (poolCursor + 1) % circlePool.Count;
            if (!circlePool[poolCursor].gameObject.activeSelf)
            {
                return circlePool[poolCursor];
            }
        }

        return MakeCircle();
    }

    void NextCircle()
    {
        if (isGameOver)
        {
            return;
        }

        lastCircle = GetCircle();
        lastCircle.level = Random.Range(0, maxLevel);
        lastCircle.gameObject.SetActive(true);

        StartCoroutine(WaitNext());
    }

    IEnumerator WaitNext()
    {
        while (lastCircle != null)
        {
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);

        NextCircle();
    }

    public void TouchDown()
    {
        if (lastCircle == null) 
            return;

        lastCircle.Drag();
    }

    public void TouchUp()
    {
        if (lastCircle == null)
            return;

        lastCircle.Drop();
        lastCircle = null;
    }

    public void GameOver()
    {
        if (isGameOver)
        {
            return;
        }
        isGameOver = true;
        StartCoroutine(GameOverRoutione());
    }

    IEnumerator GameOverRoutione()
    {
        // 1. 장면 안에 활성화 되어있는 모든 Circle 가져오기
        Circle[] circles = FindObjectsOfType<Circle>();

        // 2. 지우기 전에 모든 Circle의 물리효과 비활성화
        for (int index = 0; index < circles.Length; index++)
        {
            circles[index].rigid.simulated = false;

        }

        // 3. 1번의 목록을 하나씩 접근해서 지우기
        for (int index = 0; index < circles.Length; index++)
        {
            circles[index].Hide(Vector3.up * 100); // 게임 플레이중에 나올수없는 큰값을 넣어서 숨기기
            yield return new WaitForSeconds(0.1f);
        }
    }

}
