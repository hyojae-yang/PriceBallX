using System.Collections.Generic;
using UnityEngine;
using System.Collections;

/// <summary>
/// �÷��̾�� ���� ũ�� �̻��� ��� �÷��̾��� ������Ʈ�� �и���Ű�� ���ʹ�
/// + PickupObject �и� ��� ����
/// + �÷��̾� �浹 �� ƨ���� ������ �� ���� (������ٵ� ���� ���)
/// </summary>
public class Enemy : MonoBehaviour
{
    [Header("�÷��̾�� �󸶳� Ŀ�� �����ϴ°�")]
    private float sizeThresholdMultiplier = 1.5f;

    [Header("�и� ��ٿ� ����")]
    private float dropCooldown = 10f;

    [Header("�浹 ƨ�ܳ��� �� ����")]
    private float bounceForce = 0.008f;

    [Header("�и��� ������Ʈ�� �� Ʈ���� ��Ƽ����")]
    public Material trailMaterial;

    private PickupObject pickup;
    private bool hasRecentlyTriggered = false;

    // �÷��̾ �پ����� ����
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
            // �ֱ� ���� ������Ʈ 5���� �и�
            List<Transform> droppedObjects = DropFromPlayer(player);

            // �и��� ������Ʈ �� �÷��̾� ƨ���
            BounceObjects(player, droppedObjects);

            hasRecentlyTriggered = true;
            Invoke(nameof(ResetTrigger), dropCooldown);
        }
    }

    /// <summary>
    /// �÷��̾��� �ֱ� ���� ������Ʈ 5�� �и�
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
    /// �÷��̾�� �и��� ������Ʈ�� ƨ�ܳ��� ó��
    /// </summary>
    private void BounceObjects(PlayerController player, List<Transform> droppedObjects)
    {
        Rigidbody playerRb = player.GetComponent<Rigidbody>();
        float scaledBounceForce = bounceForce * player.GetRadius();

        Vector3 bounceDir = (player.transform.position - transform.position).normalized + Vector3.up * 0.5f;
        bounceDir.Normalize();

        // �÷��̾� ��ü ƨ���
        if (playerRb != null)
        {
            playerRb.AddForce(bounceDir * scaledBounceForce, ForceMode.Impulse);
        }
        else
        {
            player.transform.position += bounceDir * 0.5f;
        }

        // �� ������Ʈ ƨ���
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
    /// �÷��̾ �پ��� �� ���ʹ� ��Ȱ��ȭ
    /// </summary>
    public void OnAttachedToPlayer()
    {
        isAttached = true;
        StopAllCoroutines();
    }

    private IEnumerator AddTrailAndRemove(GameObject obj, float duration)
    {
        // �̹� TrailRenderer�� ������ �ߺ� �߰� ����
        TrailRenderer existingTrail = obj.GetComponent<TrailRenderer>();
        if (existingTrail != null)
        {
            yield break; // �̹� ������ �ڷ�ƾ ����
        }

        if (trailMaterial == null)
        {
            Debug.LogWarning($"Enemy.cs ���: trailMaterial�� �Ҵ���� �ʾҽ��ϴ�. ���ӿ�����Ʈ '{gameObject.name}' Ȯ�� �ʿ�.");
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
