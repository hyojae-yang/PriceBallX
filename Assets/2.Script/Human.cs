using UnityEngine;
using System.Collections;

public class Human : MonoBehaviour
{
    float moveSpeed = 1f;
    float detectionRadius = 5f;
    float fleeDistance = 5f;
    float checkInterval = 0.1f;

    [Header("배회 영역 설정 (월드 좌표 기준)")]
    private Vector3 wanderAreaCenter = new Vector3(-15, 0, -25);
    Vector2 wanderAreaSize = new Vector2(50f, 30f);

    private Vector3 targetPosition;
    private bool isFleeing = false;
    private bool isAttached = false;
    private Coroutine currentRoutine;

    private float myRadius;
    private Animator animator;
    private Rigidbody rb;

    private void Awake()
    {
        PickupObject pickup = GetComponent<PickupObject>();
        myRadius = pickup != null ? pickup.GetSizeValue() / 2f : 0.5f;
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();

        animator.SetInteger("legs", 1);

        if (pickup != null)
        {
            pickup.onAttached += OnAttachedToPlayer;
        }
    }

    private void Start()
    {
        // Rigidbody의 물리 회전 방지 (앞으로 기울어지는 현상 제거)
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }

        currentRoutine = StartCoroutine(WanderRoutine());
        InvokeRepeating(nameof(CheckForPlayer), 0f, checkInterval);
    }

    private void CheckForPlayer()
    {
        if (isAttached) return;

        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj == null) return;

        float distance = Vector3.Distance(transform.position, playerObj.transform.position);

        if (distance <= detectionRadius)
        {
            PlayerController player = playerObj.GetComponent<PlayerController>();
            if (player != null && player.GetRadius() > myRadius)
            {
                if (!isFleeing)
                {
                    animator.SetInteger("legs", 2);
                    if (currentRoutine != null) StopCoroutine(currentRoutine);
                    isFleeing = true;
                    currentRoutine = StartCoroutine(FleeRoutine(playerObj.transform));
                }
            }
        }
        else
        {
            if (isFleeing && distance > fleeDistance)
            {
                animator.SetInteger("legs", 1);
                isFleeing = false;
                if (currentRoutine != null) StopCoroutine(currentRoutine);
                currentRoutine = StartCoroutine(WanderRoutine());
            }
        }
    }

    private IEnumerator WanderRoutine()
    {
        while (!isFleeing && !isAttached)
        {
            float randomX = Random.Range(-wanderAreaSize.x * 0.5f, wanderAreaSize.x * 0.5f);
            float randomZ = Random.Range(-wanderAreaSize.y * 0.5f, wanderAreaSize.y * 0.5f);

            targetPosition = new Vector3(wanderAreaCenter.x + randomX, transform.position.y, wanderAreaCenter.z + randomZ);

            while (!isFleeing && !isAttached && Vector3.Distance(transform.position, targetPosition) > 0.2f)
            {
                Vector3 dir = (targetPosition - transform.position).normalized;

                if (dir != Vector3.zero)
                    transform.rotation = Quaternion.LookRotation(dir);

                // Rigidbody가 있으면 MovePosition 사용하여 충돌 유지
                if (rb != null)
                {
                    rb.MovePosition(transform.position + dir * moveSpeed * Time.deltaTime);
                }
                else
                {
                    transform.position += dir * moveSpeed * Time.deltaTime;
                }

                yield return null;
            }

            yield return new WaitForSeconds(Random.Range(1f, 3f));
        }
    }

    private IEnumerator FleeRoutine(Transform threat)
    {
        while (isFleeing && !isAttached)
        {
            Vector3 awayDir = (transform.position - threat.position).normalized;

            if (awayDir != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(awayDir);

            if (rb != null)
            {
                rb.MovePosition(transform.position + awayDir * moveSpeed * 2f * Time.deltaTime);
            }
            else
            {
                transform.position += awayDir * moveSpeed * 2f * Time.deltaTime;
            }

            yield return null;
        }
    }

    private void OnAttachedToPlayer()
    {
        isAttached = true;
        isFleeing = false;

        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
        }

        CancelInvoke(nameof(CheckForPlayer));
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.cyan;
        Vector3 halfSize = new Vector3(wanderAreaSize.x * 0.5f, 0, wanderAreaSize.y * 0.5f);
        Vector3 topLeft = wanderAreaCenter + new Vector3(-halfSize.x, 0, halfSize.z);
        Vector3 topRight = wanderAreaCenter + new Vector3(halfSize.x, 0, halfSize.z);
        Vector3 bottomLeft = wanderAreaCenter + new Vector3(-halfSize.x, 0, -halfSize.z);
        Vector3 bottomRight = wanderAreaCenter + new Vector3(halfSize.x, 0, -halfSize.z);

        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
        Gizmos.DrawLine(bottomLeft, topLeft);
    }
}
