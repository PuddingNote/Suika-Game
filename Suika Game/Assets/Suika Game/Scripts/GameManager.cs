using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("--------------[ Main ]")]
    public int score;               // ����
    public int maxLevel = 1;            // Circle �ִ� ����
    public bool isGameOver;         // GameOver �Ǵ�

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
    public GameObject startGroup;
    public GameObject endGroup;
    public GameObject circularGroup;
    public Text scoreText;
    public Text bestScoreText;
    public Text maxScoreText;
    public Text subScoreText;

    [Header("--------------[ ETC ]")]
    public GameObject mainGameGroup;

    void Awake()
    {
        Application.targetFrameRate = 60;   // ������ ����
        circlePool = new List<Circle>();
        effectPool = new List<ParticleSystem>();

        for (int index = 0; index < poolSize; index++)
        {
            MakeCircle();
        }

        // ó�� ���� MaxScore���� Fix
        if (!PlayerPrefs.HasKey("MaxScore"))
        {
            PlayerPrefs.SetInt("MaxScore", 0);
        }
        maxScoreText.text = PlayerPrefs.GetInt("MaxScore").ToString();  // ������ ������ ����ϴ� Class
    }

    public void GameStart()
    {
        // ������Ʈ Ȱ��ȭ
        mainGameGroup.SetActive(true);
        scoreText.gameObject.SetActive(true);
        bestScoreText.gameObject.SetActive(true);
        maxScoreText.gameObject.SetActive(true);
        startGroup.SetActive(false);
        circularGroup.SetActive(true);

        // ���ӽ��� (Circle ����)
        Invoke("NextCircle", 1.5f);
    }

    // Circle ���� �Լ�
    Circle MakeCircle()
    {
        // Effect ����
        GameObject instanceEffectObj = Instantiate(effectPrefab, effectGroup);
        instanceEffectObj.name = "Effect " + effectPool.Count;
        ParticleSystem instanceEffect = instanceEffectObj.GetComponent<ParticleSystem>();
        effectPool.Add(instanceEffect);

        // Circle ����
        GameObject instanceCircleObj = Instantiate(circlePrefab, circleGroup);
        instanceEffectObj.name = "Circle " + circlePool.Count;
        Circle instanceCircle = instanceCircleObj.GetComponent<Circle>();
        instanceCircle.gameManager = this;
        instanceCircle.effect = instanceEffect;
        circlePool.Add(instanceCircle);

        return instanceCircle;
    }

    // 
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

    // 
    void NextCircle()
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
        lastCircle.gameObject.SetActive(true);

        StartCoroutine(WaitNext());
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
        // 1. ��� �ȿ� Ȱ��ȭ �Ǿ��ִ� ��� Circle ��������
        Circle[] circles = FindObjectsOfType<Circle>();

        // 2. ����� ���� ��� Circle�� ����ȿ�� ��Ȱ��ȭ
        for (int index = 0; index < circles.Length; index++)
        {
            circles[index].rigid.simulated = false;

        }

        // 3. 1���� ����� �ϳ��� �����ؼ� �����
        for (int index = 0; index < circles.Length; index++)
        {
            circles[index].Hide(Vector3.up * 100); // ���� �÷����߿� ���ü����� ū���� �־ �����
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(1f);

        // �ְ� ���� ����
        int maxScore = Mathf.Max(score, PlayerPrefs.GetInt("MaxScore"));
        PlayerPrefs.SetInt("MaxScore", maxScore);

        // ���ӿ��� UI ǥ��
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
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("GameScene");
    }

    // ����� Quit ��ư
    //void Update()
    //{
    //    if (Input.GetButtonDown("Cancel"))
    //    {
    //        Application.Quit();
    //    }
    //}

    // 
    void LateUpdate()
    {
        scoreText.text = score.ToString();
    }

}
