using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// �÷��̾� ��Ʈ�ѷ�
/// - �̵� �� ũ�� ����
/// - ������Ʈ ���� �� �ֱ� ���� ������Ʈ ����
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("�̵� �ӵ�")]
    public float moveSpeed = 5f;
    public float speedGrowthFactor = 0.5f;

    [Header("���� ũ�� ���� (�������� ū ������Ʈ�� ���� ����)")]
    [SerializeField] private float attachSizeRatio;

    [Header("������Ʈ ũ��� ������ ���� ����")]
    [SerializeField] private float growthRatio;

    [SerializeField] private AudioClip attachSound;
    [SerializeField] private AudioSource audioSource;

    private Rigidbody rb;
    private SphereCollider sphereCol;
    private bool isPaused = false;

    public GameObject pausePanel;

    [Range(0f, 1f)]
    public float offsetTest;

    [SerializeField] private int maxRecentPickups = 10;
    private Stack<GameObject> recentPickups = new Stack<GameObject>();
    private Transform bulkRoot;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        sphereCol = GetComponent<SphereCollider>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        // ������ ���� ������Ʈ�� ���� �θ� ������Ʈ ����
        bulkRoot = new GameObject("BulkObjects").transform;
        bulkRoot.SetParent(transform);
        bulkRoot.localPosition = Vector3.zero;

        // ���� ���� �ڵ�� ���ŵǾ� �ڷ�ƾ �������� ����
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            TogglePause();

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.F1)) GrowBall(0.01f);
        else if (Input.GetKeyDown(KeyCode.F2)) GrowBall(0.1f);
        else if (Input.GetKeyDown(KeyCode.F3)) GrowBall(1f);
        if (transform.localScale.x > 0.1f)
        {
            if (Input.GetKeyDown(KeyCode.F4)) GrowBall(-0.01f);
            else if (Input.GetKeyDown(KeyCode.F5)) GrowBall(-0.1f);
            else if (Input.GetKeyDown(KeyCode.F6)) GrowBall(-1f);
        }
#endif
    }

    void FixedUpdate()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 input = new Vector3(h, 0, v).normalized;
        if (input.sqrMagnitude == 0f) return;

        Vector3 camForward = Camera.main.transform.forward;
        Vector3 camRight = Camera.main.transform.right;
        camForward.y = camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDir = camForward * v + camRight * h;
        float dynamicSpeed = moveSpeed + (GetRadius() * speedGrowthFactor);
        Vector3 moveOffset = moveDir * dynamicSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + moveOffset);

        Vector3 rotateAxis = Vector3.Cross(Vector3.up, moveDir);
        transform.Rotate(200f * Time.fixedDeltaTime * rotateAxis, Space.World);
    }

    /// <summary>
    /// ���� �÷��̾� ������ (������ ����)
    /// </summary>
    public float GetRadius()
    {
        return sphereCol.radius * transform.localScale.x;
    }

    /// <summary>
    /// ������ �׽�Ʈ�� (offset ����)
    /// </summary>
    public float GetRadiusTEST()
    {
        if (sphereCol == null) return 0f;
        return (sphereCol.radius + offsetTest) * transform.localScale.x;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.TryGetComponent<PickupObject>(out var pickup)) return;

        float playerDiameter = GetRadius() * 2f;
        float objectSize = pickup.GetSizeValue();

        if (objectSize > 0f && objectSize < playerDiameter / attachSizeRatio)
        {
            Vector3 contactPoint = collision.contacts[0].point;
            Vector3 surfaceDir = (contactPoint - transform.position).normalized;
            Vector3 offset = transform.position + surfaceDir * GetRadiusTEST();
            AttachObject(collision.gameObject, objectSize, offset);
        }
    }

    /// <summary>
    /// ������Ʈ ���̱� �� �ֱ� ���� ������Ʈ ����
    /// </summary>
    private void AttachObject(GameObject obj, float objectSize, Vector3 attachPosition)
    {
        if (obj.TryGetComponent<Rigidbody>(out var objRb))
        {
            objRb.isKinematic = true;
            objRb.detectCollisions = false;
        }

        obj.transform.position = attachPosition;
        obj.transform.SetParent(transform);

        if (obj.TryGetComponent<Enemy>(out var enemy))
            enemy.OnAttachedToPlayer();

        float growAmount = objectSize * growthRatio;
        if (obj.TryGetComponent<PickupObject>(out var pickup))
            pickup.SetGrowthAmount(growAmount);

        sphereCol.radius += growAmount;

        // �ֱ� ���� ������Ʈ ���ÿ� �߰�
        recentPickups.Push(obj);

        // ���� ũ�� �ʰ� �� ������ ������Ʈ�� bulkRoot�� �̵�
        if (recentPickups.Count > maxRecentPickups)
        {
            GameObject[] all = recentPickups.ToArray();
            recentPickups.Clear();
            for (int i = all.Length - 1; i >= 0; i--)
            {
                if (i >= maxRecentPickups)
                    all[i].transform.SetParent(bulkRoot);
                else
                    recentPickups.Push(all[i]);
            }
        }

        AudioManager.Instance?.PlaySetSFX();
    }

    /// <summary>
    /// ������ ����
    /// </summary>
    public void ShrinkBy(float shrinkAmount)
    {
        sphereCol.radius -= shrinkAmount;
        sphereCol.radius = Mathf.Max(0.1f, sphereCol.radius);
    }

    /// <summary>
    /// ũ�� ���� (���߿�)
    /// </summary>
    private void GrowBall(float amount)
    {
        Vector3 newScale = transform.localScale + Vector3.one * amount;
        transform.localScale = newScale;
    }

    /// <summary>
    /// �Ͻ����� ���
    /// </summary>
    public void TogglePause()
    {
        isPaused = !isPaused;
        pausePanel.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;
    }

    /// <summary>
    /// �ֱ� ���� ������Ʈ �� �Ϻ� �и� (���ʹ� �浹 �� ȣ��)
    /// </summary>
    /// <param name="count">�и��� ����</param>
    /// <returns>�и��� ������Ʈ Transform ����Ʈ</returns>
    public List<Transform> DetachRecentObjects(int count)
    {
        List<Transform> detached = new List<Transform>();

        for (int i = 0; i < count && recentPickups.Count > 0; i++)
        {
            GameObject go = recentPickups.Pop();
            go.transform.SetParent(null);
            detached.Add(go.transform);
        }

        return detached;
    }
}
