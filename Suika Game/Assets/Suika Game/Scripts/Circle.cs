using System.Collections;
using UnityEngine;

public class Circle : MonoBehaviour
{
    [Header("--------------[ Main ]")]
    public GameManager gameManager;
    public ParticleSystem effect;
    public Rigidbody2D rigid;
    public CircleCollider2D circle;
    public Animator anim;
    public SpriteRenderer spriteRenderer;

    [Header("--------------[ DropLine ]")]
    public GameObject dropLine;
    public GameObject childPrefab;

    [Header("--------------[ Circle ]")]
    public float leftBorder;    // 왼쪽벽
    public float rightBorder;   // 오른쪽벽

    public bool isDrag;         // 클릭중일때
    public int level;           // Circle 레벨

    public bool isMerge;        // Circle 합치기

    public float deadTime;      // 경계선에 걸려있을 수 있는 시간

    public void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        circle = GetComponent<CircleCollider2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        childPrefab = Resources.Load<GameObject>("Prefabs/DropLine");
        dropLine = Instantiate(childPrefab);
        dropLine.transform.SetParent(this.gameObject.transform, false);
    }

    public void OnEnable()
    {
        anim.SetInteger("Level", level);
    }

    public void OnDisable()
    {
        // Circle 속성 초기화
        level = 0;
        isDrag = false;
        isMerge = false;

        // Circle Transform 초기화
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.zero;

        // Circle 물리 초기화
        rigid.simulated = false;
        rigid.velocity = Vector2.zero;
        rigid.angularVelocity = 0;
        circle.enabled = true;
    }

    public void Update()
    {
        if (isDrag)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.y = 4.25f;
            mousePos.z = 0;

            // X축 경계 설정
            leftBorder = -2.9f + transform.localScale.x / 2f;
            rightBorder = 2.9f - transform.localScale.x / 2f;

            if (mousePos.x < leftBorder)
            {
                mousePos.x = leftBorder;
            }
            else if (mousePos.x > rightBorder)
            {
                mousePos.x = rightBorder;
            }
            transform.position = Vector3.Lerp(transform.position, mousePos, 0.8f);
        }
    }

    // 클릭중 O (터치중 0)
    public void Drag()
    {
        isDrag = true;
    }

    // 클릭중 X (터치중 X)
    public void Drop()
    {
        isDrag = false;
        rigid.simulated = true;
        dropLine.SetActive(false);
    }

    public void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Circle")
        {
            Circle otherCircle = collision.gameObject.GetComponent<Circle>();

            // Circle 합치기
            if (level == otherCircle.level && !isMerge && !otherCircle.isMerge && level < 9)
            {
                // 나와 상대편 위치 가져오기
                float meX = transform.position.x;
                float meY = transform.position.y;
                float otherX = otherCircle.transform.position.x;
                float otherY = otherCircle.transform.position.y;

                // 내가 아래에 있을 때, 동일한 높이일 때, 내가 오른쪽에 있을때
                if (meY < otherY || (meY == otherY && meY > otherX))
                {
                    otherCircle.Hide(transform.position);

                    LevelUp();
                }
            }
        }
    }

    // 합쳤을때 숨겨지는 함수
    public void Hide(Vector3 targetPos)
    {
        isMerge = true;
        rigid.simulated = false;
        circle.enabled = false;

        if (targetPos == Vector3.up * 100)
        {
            EffectPlay();
        }

        if (this.gameObject.activeInHierarchy)
        {
            StartCoroutine(HideRoutine(targetPos));
        }
    }

    // Circle 숨기기 코루틴
    IEnumerator HideRoutine(Vector3 targetPos)
    {
        int frameCount = 0;

        while (frameCount < 20)
        {
            frameCount++;

            if (targetPos != Vector3.up * 100)
            {
                transform.position = Vector3.Lerp(transform.position, targetPos, 0.5f);
            }
            else if (targetPos == Vector3.up * 100)
            {
                transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, 0.2f);
            }

            yield return null;
        }

        gameManager.score += (int)Mathf.Pow(2, level); // 지정 숫자의 거듭제곱

        isMerge = false;
        gameObject.SetActive(false);
    }

    // Circle 레벨업 함수
    public void LevelUp()
    {
        isMerge = true;
        rigid.velocity = Vector2.zero;  // 물리속도 제거
        rigid.angularVelocity = 0;      // 회전속도 제거

        StartCoroutine(LevelUpRoutine());
    }

    // Circle 레벨업 코루틴
    IEnumerator LevelUpRoutine()
    {
        yield return new WaitForSeconds(0.2f);

        anim.SetInteger("Level", level + 1);
        EffectPlay();

        yield return new WaitForSeconds(0.3f);
        level++;

        gameManager.maxLevel = Mathf.Max(level, gameManager.maxLevel);

        isMerge = false;
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Finish")
        {
            deadTime += Time.deltaTime;

            if (deadTime > 2)
            {
                spriteRenderer.color = Color.red;
            }
            if (deadTime > 5)
            {
                gameManager.GameOver();
            }
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Finish")
        {
            deadTime = 0;
            spriteRenderer.color = Color.white;
        }
    }

    // 이펙트 실행 함수
    public void EffectPlay()
    {
        effect.transform.position = transform.position;
        effect.transform.localScale = transform.localScale;
        effect.Play();
    }

}
