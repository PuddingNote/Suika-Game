using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("--------------[ Main ]")]
    public int score;               // ����
    public int maxLevel;            // Circle �ִ� ����
    public bool isGameOver;         // GameOver �Ǵ�

    [Header("--------------[ Object Pooling ]")]
    // Circle ���� ������
    public GameObject circlePrefab;
    public Transform circleGroup;
    public List<Circle> circlePool;

    // Effect ���� ������
    public GameObject effectPrefab;
    public Transform effectGroup;
    public List<ParticleSystem> effectPool;

    public Circle currentCircle;    // ���� Circle

    [Range(1, 50)]
    public int poolSize;            // ������Ʈ Ǯ�� Size
    public int poolCursor;          // ������Ʈ Ǯ�� Cursor(��ġ)

    [Header("--------------[ Next Circle ]")]
    // ���� Circle ���� ������
    public Circle nextCircle;
    public Sprite[] circleSprites;
    public SpriteRenderer nextCircleImage;

    [Header("--------------[ UI ]")]
    public GameObject endGroup;
    public Text scoreText;
    public Text maxScoreText;
    public Text subScoreText;

    [field: SerializeField] public int a { get; private set; }
    public void Awake()
    {
        Application.targetFrameRate = 60;   // ������ ����
        circlePool = new List<Circle>();
        effectPool = new List<ParticleSystem>();
        nextCircleImage = GameObject.Find("NextCircle").GetComponent<SpriteRenderer>();
        maxLevel = 1;   // ���� maxLevel �ʱ�ȭ

        // ������Ʈ Ǯ��
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

    // ���ӽ��� �ڷ�ƾ
    IEnumerator GameStart()
    {
        yield return new WaitForSeconds(1.0f);

        FirstCircle();
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

    // Circle ���� ��������
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

    // ó�� Circle ���� (���� ���� �� ù��° Circle �����Ҷ� ȣ��)
    public void FirstCircle()
    {
        currentCircle = GetCircle();
        nextCircle = GetCircle();

        currentCircle.level = Random.Range(0, 2);
        nextCircle.level = Random.Range(0, 2);

        nextCircleImage.sprite = circleSprites[nextCircle.level];
        SetActiveRecursively(currentCircle.gameObject, true);
        
        StartCoroutine(WaitNext());
    }

    // ���� Circle ���� (���� ���� �� ù��°�� ������ Circle �����Ҷ� ȣ��)
    public void NextCircle()
    {
        if (isGameOver)
        {
            return;
        }

        currentCircle = nextCircle;
        nextCircle = GetCircle();
        if (maxLevel < 4)
        {
            nextCircle.level = Random.Range(0, maxLevel);
        }
        else
        {
            nextCircle.level = Random.Range(0, 5);
        }
        nextCircleImage.sprite = circleSprites[nextCircle.level];
        SetActiveRecursively(currentCircle.gameObject, true);

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

    // ���� Circle ��� �ڷ�ƾ
    IEnumerator WaitNext()
    {
        while (currentCircle != null)
        {
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);

        NextCircle();
    }

    // Ŭ���� O (��ġ�� O)
    public void TouchDown()
    {
        if (currentCircle == null) 
            return;

        currentCircle.Drag();
    }

    // Ŭ���� X (��ġ�� X)
    public void TouchUp()
    {
        if (currentCircle == null)
            return;

        currentCircle.Drop();
        currentCircle = null;
    }

    // �������� �Լ�
    public void GameOver()
    {
        if (isGameOver)
        {
            return;
        }
        isGameOver = true;
        StartCoroutine(GameOverRoutione());
    }

    // ���ӿ��� �ڷ�ƾ
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

    // ���� ����� �Լ�
    public void Reset()
    {
        StartCoroutine(ResetCoroutine());
    }

    // ���� ����� �ڷ�ƾ
    IEnumerator ResetCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("GameScene");
    }

    public void Update()
    {
        // ����� Quit ��ư
        if (Input.GetButtonDown("Cancel"))
        {
            Application.Quit();
        }
    }

    public void LateUpdate()
    {
        scoreText.text = score.ToString();
    }

}
