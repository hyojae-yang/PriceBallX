using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 플레이어 컨트롤러
/// - 이동 및 크기 성장
/// - 오브젝트 붙임 및 최근 붙은 오브젝트 관리
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("이동 속도")]
    public float moveSpeed = 5f;
    public float speedGrowthFactor = 0.5f;

    [Header("붙임 크기 비율 (작을수록 큰 오브젝트가 쉽게 붙음)")]
    [SerializeField] private float attachSizeRatio;

    [Header("오브젝트 크기당 반지름 성장 비율")]
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

        // 오래된 붙은 오브젝트를 담을 부모 오브젝트 생성
        bulkRoot = new GameObject("BulkObjects").transform;
        bulkRoot.SetParent(transform);
        bulkRoot.localPosition = Vector3.zero;

        // 병합 관련 코드는 제거되어 코루틴 시작하지 않음
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
    /// 현재 플레이어 반지름 (스케일 포함)
    /// </summary>
    public float GetRadius()
    {
        return sphereCol.radius * transform.localScale.x;
    }

    /// <summary>
    /// 반지름 테스트용 (offset 포함)
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
    /// 오브젝트 붙이기 및 최근 붙은 오브젝트 관리
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

        // 최근 붙은 오브젝트 스택에 추가
        recentPickups.Push(obj);

        // 스택 크기 초과 시 오래된 오브젝트를 bulkRoot로 이동
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
    /// 반지름 감소
    /// </summary>
    public void ShrinkBy(float shrinkAmount)
    {
        sphereCol.radius -= shrinkAmount;
        sphereCol.radius = Mathf.Max(0.1f, sphereCol.radius);
    }

    /// <summary>
    /// 크기 조절 (개발용)
    /// </summary>
    private void GrowBall(float amount)
    {
        Vector3 newScale = transform.localScale + Vector3.one * amount;
        transform.localScale = newScale;
    }

    /// <summary>
    /// 일시정지 토글
    /// </summary>
    public void TogglePause()
    {
        isPaused = !isPaused;
        pausePanel.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;
    }

    /// <summary>
    /// 최근 붙은 오브젝트 중 일부 분리 (에너미 충돌 시 호출)
    /// </summary>
    /// <param name="count">분리할 개수</param>
    /// <returns>분리된 오브젝트 Transform 리스트</returns>
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
