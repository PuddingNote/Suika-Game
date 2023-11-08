using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("--------------[ Main ]")]
    public int score;               // ����
    public int maxLevel = 1;        // Circle �ִ� ����
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
    public GameObject endGroup;
    public Text scoreText;
    public Text maxScoreText;
    public Text subScoreText;

    //[Header("--------------[ TEST ]")]



    public void Awake()
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

        // ���ӽ��� (Circle ����)
        StartCoroutine(GameStart());
    }

    IEnumerator GameStart()
    {
        yield return new WaitForSeconds(1.0f);

        NextCircle();
    }

    // Circle ���� �Լ�
    public Circle MakeCircle()
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

    // �ش� Object�� ��� ���� �ڽĿ�����Ʈ���� ��������� Ȱ��ȭ (DropLine Ȱ��ȭ�Ҷ� ���)
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
        // ��� �ȿ� Ȱ��ȭ �Ǿ��ִ� ��� Circle ��������
        Circle[] circles = FindObjectsOfType<Circle>();

        // ����� ���� ��� Circle�� ����ȿ�� ��Ȱ��ȭ
        for (int index = 0; index < circles.Length; index++)
        {
            circles[index].rigid.simulated = false;

        }

        // circles ����� �ϳ��� �����ؼ� �����
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
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("GameScene");
    }

    // ����� Quit ��ư
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
