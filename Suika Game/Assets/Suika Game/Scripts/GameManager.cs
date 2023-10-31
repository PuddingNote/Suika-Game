using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Circle lastCircle;
    public GameObject circlePrefab;
    public Transform circleGroup;
    public GameObject effectPrefab;
    public Transform effectGroup;

    public int maxLevel;

    void Awake()
    {
        Application.targetFrameRate = 60;   // ������ ����
    }

    void Start()
    {
        NextCircle();
    }

    Circle GetCircle()
    {
        // Effect ����
        GameObject instanceEffectObj = Instantiate(effectPrefab, effectGroup);
        ParticleSystem instanceEffect = instanceEffectObj.GetComponent<ParticleSystem>();

        // Circle ����
        GameObject instanceCircleObj = Instantiate(circlePrefab, circleGroup);
        Circle instanceCircle = instanceCircleObj.GetComponent<Circle>();
        instanceCircle.effect = instanceEffect;

        return instanceCircle;
    }

    void NextCircle()
    {
        Circle newCircle = GetCircle();
        lastCircle = newCircle;
        lastCircle.gameManager = this;
        lastCircle.level = Random.Range(0, maxLevel);
        lastCircle.gameObject.SetActive(true);

        StartCoroutine("WaitNext"); // WaitNext()�� �־��
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

}
