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
    public float leftBorder;    // ���ʺ�
    public float rightBorder;   // �����ʺ�

    public bool isDrag;         // Ŭ�����϶�
    public int level;           // Circle ����

    public bool isMerge;        // Circle ��ġ��

    public float deadTime;      // ��輱�� �ɷ����� �� �ִ� �ð�

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
        // Circle �Ӽ� �ʱ�ȭ
        level = 0;
        isDrag = false;
        isMerge = false;

        // Circle Transform �ʱ�ȭ
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.zero;

        // Circle ���� �ʱ�ȭ
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

            // X�� ��� ����
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

    // Ŭ���� O (��ġ�� 0)
    public void Drag()
    {
        isDrag = true;
    }

    // Ŭ���� X (��ġ�� X)
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

            // Circle ��ġ��
            if (level == otherCircle.level && !isMerge && !otherCircle.isMerge && level < 9)
            {
                // ���� ����� ��ġ ��������
                float meX = transform.position.x;
                float meY = transform.position.y;
                float otherX = otherCircle.transform.position.x;
                float otherY = otherCircle.transform.position.y;

                // ���� �Ʒ��� ���� ��, ������ ������ ��, ���� �����ʿ� ������
                if (meY < otherY || (meY == otherY && meY > otherX))
                {
                    otherCircle.Hide(transform.position);

                    LevelUp();
                }
            }
        }
    }

    // �������� �������� �Լ�
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

    // Circle ����� �ڷ�ƾ
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

        gameManager.score += (int)Mathf.Pow(2, level); // ���� ������ �ŵ�����

        isMerge = false;
        gameObject.SetActive(false);
    }

    // Circle ������ �Լ�
    public void LevelUp()
    {
        isMerge = true;
        rigid.velocity = Vector2.zero;  // �����ӵ� ����
        rigid.angularVelocity = 0;      // ȸ���ӵ� ����

        StartCoroutine(LevelUpRoutine());
    }

    // Circle ������ �ڷ�ƾ
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

    // ����Ʈ ���� �Լ�
    public void EffectPlay()
    {
        effect.transform.position = transform.position;
        effect.transform.localScale = transform.localScale;
        effect.Play();
    }

}
