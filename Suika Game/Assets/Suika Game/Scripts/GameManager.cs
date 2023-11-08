using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("--------------[ Main ]")]
    public int score;               // 점수
    public int maxLevel = 1;        // Circle 최대 레벨
    public bool isGameOver;         // GameOver 판단

    [Header("--------------[ Object Pooling ]")]
    public GameObject circlePrefab;
    public Transform circleGroup;
    public List<Circle> circlePool;

    public GameObject effectPrefab;
    public Transform effectGroup;
    public List<ParticleSystem> effectPool;

    public Circle lastCircle;       
    [Range(1, 30)]
    public int poolSize;            // Size
    public int poolCursor;          // Cursor

    [Header("--------------[ UI ]")]
    public GameObject endGroup;
    public Text scoreText;
    public Text maxScoreText;
    public Text subScoreText;

    //[Header("--------------[ TEST ]")]



    public void Awake()
    {
        Application.targetFrameRate = 60;   // 프레임 설정
        circlePool = new List<Circle>();
        effectPool = new List<ParticleSystem>();

        for (int index = 0; index < poolSize; index++)
        {
            MakeCircle();
        }

        // 처음 시작 MaxScore버그 Fix
        if (!PlayerPrefs.HasKey("MaxScore"))
        {
            PlayerPrefs.SetInt("MaxScore", 0);
        }
        maxScoreText.text = PlayerPrefs.GetInt("MaxScore").ToString();  // 데이터 저장을 담당하는 Class

        // 게임시작 (Circle 생성)
        StartCoroutine(GameStart());
    }

    IEnumerator GameStart()
    {
        yield return new WaitForSeconds(1.0f);

        NextCircle();
    }

    // Circle 생성 함수
    public Circle MakeCircle()
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

    // 
    public Circle GetCircle()
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

    // 
    public void NextCircle()
    {
        if (isGameOver)
        {
            return;
        }

        lastCircle = GetCircle();
        if (maxLevel < 4)
        {
            lastCircle.level = Random.Range(0, maxLevel);
        }
        else
        {
            lastCircle.level = Random.Range(0, 5);
        }
        SetActiveRecursively(lastCircle.gameObject, true);

        StartCoroutine(WaitNext());
    }

    // 해당 Object및 모든 하위 자식오브젝트들을 재귀적으로 활성화 (DropLine 활성화할때 사용)
    public void SetActiveRecursively(GameObject obj, bool state)
    {
        obj.SetActive(state);

        foreach (Transform child in obj.transform)
        {
            SetActiveRecursively(child.gameObject, state);
        }
    }

    // 
    IEnumerator WaitNext()
    {
        while (lastCircle != null)
        {
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);

        NextCircle();
    }

    // 
    public void TouchDown()
    {
        if (lastCircle == null) 
            return;

        lastCircle.Drag();
    }

    // 
    public void TouchUp()
    {
        if (lastCircle == null)
            return;

        lastCircle.Drop();
        lastCircle = null;
    }

    // 
    public void GameOver()
    {
        if (isGameOver)
        {
            return;
        }
        isGameOver = true;
        StartCoroutine(GameOverRoutione());
    }

    // 
    IEnumerator GameOverRoutione()
    {
        // 장면 안에 활성화 되어있는 모든 Circle 가져오기
        Circle[] circles = FindObjectsOfType<Circle>();

        // 지우기 전에 모든 Circle의 물리효과 비활성화
        for (int index = 0; index < circles.Length; index++)
        {
            circles[index].rigid.simulated = false;

        }

        // circles 목록을 하나씩 접근해서 지우기
        for (int index = 0; index < circles.Length; index++)
        {
            circles[index].Hide(Vector3.up * 100); // 게임 플레이중에 나올수없는 큰값을 넣어서 숨기기
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(1f);

        // 최고 점수 갱신
        int maxScore = Mathf.Max(score, PlayerPrefs.GetInt("MaxScore"));
        PlayerPrefs.SetInt("MaxScore", maxScore);

        // 게임오버 UI 표시
        subScoreText.text = "Score: " + scoreText.text;
        endGroup.SetActive(true);
    }

    // 
    public void Reset()
    {
        StartCoroutine(ResetCoroutine());
    }

    // 
    IEnumerator ResetCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("GameScene");
    }

    // 모바일 Quit 버튼
    public void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            Application.Quit();
        }
    }

    // 
    public void LateUpdate()
    {
        scoreText.text = score.ToString();
    }

}
