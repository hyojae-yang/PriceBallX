using System.Collections.Generic;
using UnityEngine;
using System.Collections;

/// <summary>
/// 플레이어보다 일정 크기 이상일 경우 플레이어의 오브젝트를 분리시키는 에너미
/// + PickupObject 분리 기능 포함
/// + 플레이어 충돌 시 튕겨져 나가는 힘 적용 (리지드바디 유무 고려)
/// </summary>
public class Enemy : MonoBehaviour
{
    [Header("플레이어보다 얼마나 커야 반응하는가")]
    private float sizeThresholdMultiplier = 1.5f;

    [Header("분리 쿨다운 설정")]
    private float dropCooldown = 10f;

    [Header("충돌 튕겨내기 힘 세기")]
    private float bounceForce = 0.008f;

    [Header("분리된 오브젝트에 들어갈 트레일 머티리얼")]
    public Material trailMaterial;

    private PickupObject pickup;
    private bool hasRecentlyTriggered = false;

    // 플레이어에 붙었는지 여부
    public bool isAttached = false;

    public virtual void Awake()
    {
        pickup = GetComponent<PickupObject>();
    }

    private void Update()
    {
        if (isAttached)
            return;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasRecentlyTriggered || isAttached)
            return;

        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        if (player == null)
            return;

        float playerRadius = player.GetRadius();
        float myRadius = pickup.GetSizeValue() / 2f;

        if (myRadius > playerRadius * sizeThresholdMultiplier)
        {
            // 최근 붙은 오브젝트 5개만 분리
            List<Transform> droppedObjects = DropFromPlayer(player);

            // 분리된 오브젝트 및 플레이어 튕기기
            BounceObjects(player, droppedObjects);

            hasRecentlyTriggered = true;
            Invoke(nameof(ResetTrigger), dropCooldown);
        }
    }

    /// <summary>
    /// 플레이어의 최근 붙은 오브젝트 5개 분리
    /// </summary>
    private List<Transform> DropFromPlayer(PlayerController player)
    {
        AudioManager.Instance?.PlayDistroySFX();

        int dropCount = 5;
        List<Transform> droppedObjects = player.DetachRecentObjects(dropCount);

        float totalShrinkAmount = 0f;

        foreach (var t in droppedObjects)
        {
            var pickup = t.GetComponent<PickupObject>();
            if (pickup != null)
                totalShrinkAmount += pickup.GetGrowthAmount();

            Rigidbody rb = t.GetComponent<Rigidbody>();
            bool newlyAdded = false;

            if (rb == null)
            {
                rb = t.gameObject.AddComponent<Rigidbody>();
                newlyAdded = true;
            }

            rb.isKinematic = false;
            rb.useGravity = true;

            if (newlyAdded)
            {
                MonoBehaviour runner = player.GetComponent<MonoBehaviour>();
                if (runner != null)
                    runner.StartCoroutine(RemoveRigidbodyAfterDelay(t.gameObject, rb, 7f));
            }

            StartCoroutine(AddTrailAndRemove(t.gameObject, 0.6f));
        }

        if (totalShrinkAmount > 0f)
            player.ShrinkBy(totalShrinkAmount);

        return droppedObjects;
    }

    private IEnumerator RemoveRigidbodyAfterDelay(GameObject obj, Rigidbody rb, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (obj != null && rb != null)
        {
            Object.Destroy(rb);
        }
    }

    /// <summary>
    /// 플레이어와 분리된 오브젝트들 튕겨내기 처리
    /// </summary>
    private void BounceObjects(PlayerController player, List<Transform> droppedObjects)
    {
        Rigidbody playerRb = player.GetComponent<Rigidbody>();
        float scaledBounceForce = bounceForce * player.GetRadius();

        Vector3 bounceDir = (player.transform.position - transform.position).normalized + Vector3.up * 0.5f;
        bounceDir.Normalize();

        // 플레이어 본체 튕기기
        if (playerRb != null)
        {
            playerRb.AddForce(bounceDir * scaledBounceForce, ForceMode.Impulse);
        }
        else
        {
            player.transform.position += bounceDir * 0.5f;
        }

        // 각 오브젝트 튕기기
        foreach (var objTransform in droppedObjects)
        {
            Rigidbody rb = objTransform.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 baseDir = (objTransform.position - player.transform.position).normalized;
                Vector3 randomSpread = new Vector3(
                    Random.Range(-0.3f, 0.3f),
                    Random.Range(0.6f, 1.0f),
                    Random.Range(-0.3f, 0.3f)
                );

                Vector3 forceDir = (baseDir + randomSpread).normalized;

                rb.isKinematic = false;
                rb.useGravity = true;
                rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                rb.detectCollisions = true;

                objTransform.position += Vector3.up * 0.2f;

                rb.AddForce(forceDir * scaledBounceForce * 4f, ForceMode.VelocityChange);
            }
            else
            {
                Vector3 fallbackDir = (objTransform.position - player.transform.position).normalized + Vector3.up * 0.5f;
                objTransform.position += fallbackDir.normalized * 0.3f;
            }
        }
    }

    private void ResetTrigger()
    {
        hasRecentlyTriggered = false;
    }

    /// <summary>
    /// 플레이어에 붙었을 때 에너미 비활성화
    /// </summary>
    public void OnAttachedToPlayer()
    {
        isAttached = true;
        StopAllCoroutines();
    }

    private IEnumerator AddTrailAndRemove(GameObject obj, float duration)
    {
        // 이미 TrailRenderer가 있으면 중복 추가 방지
        TrailRenderer existingTrail = obj.GetComponent<TrailRenderer>();
        if (existingTrail != null)
        {
            yield break; // 이미 있으면 코루틴 종료
        }

        if (trailMaterial == null)
        {
            Debug.LogWarning($"Enemy.cs 경고: trailMaterial이 할당되지 않았습니다. 게임오브젝트 '{gameObject.name}' 확인 필요.");
            yield break;
        }

        TrailRenderer trail = obj.AddComponent<TrailRenderer>();

        trail.time = 0.5f;
        trail.startWidth = 0.01f;
        trail.endWidth = 0f;
        trail.material = trailMaterial;
        trail.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        trail.receiveShadows = false;

        yield return new WaitForSeconds(duration);

        if (trail != null)
        {
            Destroy(trail);
        }
    }
}
