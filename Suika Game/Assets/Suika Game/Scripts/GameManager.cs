using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Circle lastCircle;
    public GameObject circlePrefabs;
    public Transform circleGroup;

    void Awake()
    {
        Application.targetFrameRate = 60;   // 프레임 설정
    }

    void Start()
    {
        NextCircle();
    }

    Circle GetCircle()
    {
        GameObject instance = Instantiate(circlePrefabs, circleGroup);
        Circle instanceCircle = instance.GetComponent<Circle>();
        return instanceCircle;
    }

    void NextCircle()
    {
        Circle newCircle = GetCircle();
        lastCircle = newCircle;
        lastCircle.level = Random.Range(0, 8);
        lastCircle.gameObject.SetActive(true);
        StartCoroutine("WaitNext"); // WaitNext()로 넣어도됌
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
