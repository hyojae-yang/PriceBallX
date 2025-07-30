using System.Collections.Generic;
using UnityEngine;
using System.Linq;
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
    float dropCooldown = 10f;

    [Header("�浹 ƨ�ܳ��� �� ����")]
    float bounceForce = 0.008f;

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

    /// <summary>
    /// ������ �պ� �̵�
    /// </summary>
    

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
            // DropFromPlayer�� List<Transform> ��ȯ�ϵ��� ���� ��,
            List<Transform> droppedObjects = DropFromPlayer(player);

            // �и��� ������Ʈ�� ƨ�ܳ���
            BounceObjects(player, droppedObjects);

            hasRecentlyTriggered = true;
            Invoke(nameof(ResetTrigger), dropCooldown);
        }
    }


    /// <summary>
    /// �÷��̾ ���� ������Ʈ�� �� �Ϻθ� �����ϰ� ���
    /// </summary>
    private List<Transform> DropFromPlayer(PlayerController player)
    {
        GameObject playerObj = player.gameObject;
        List<PickupObject> pickups = new List<PickupObject>();

        // �÷��̾� �ڽ� �� PickupObject ������Ʈ�� �ִ� �͵��� ����
        foreach (Transform child in playerObj.transform)
        {
            var pickup = child.GetComponent<PickupObject>();
            if (pickup != null)
                pickups.Add(pickup);
        }

        if (pickups.Count == 0)
        {
            return new List<Transform>(); // �и��� �� ������ �� ����Ʈ ��ȯ
        }

        AudioManager.Instance?.PlayDistroySFX();

        int dropCount = Mathf.Min(5, pickups.Count);
        var randomPickups = pickups.OrderBy(x => Random.value).Take(dropCount).ToList();

        float totalShrinkAmount = 0f;
        List<Transform> droppedObjects = new List<Transform>(); // �и��� ������Ʈ ����Ʈ

        foreach (var pickup in randomPickups)
        {
            Transform obj = pickup.transform;
            obj.parent = null;

            Rigidbody rb = obj.GetComponent<Rigidbody>();
            bool newlyAdded = false;

            // Rigidbody ������ ���� �߰�
            if (rb == null)
            {
                rb = obj.gameObject.AddComponent<Rigidbody>();
                newlyAdded = true;
            }

            // �ʱ�ȭ
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.linearVelocity = Vector3.zero;
            StartCoroutine(AddTrailAndRemove(obj.gameObject, 0.6f));
            // ���� �ð� �� Rigidbody ���� (Collider�� ����)
            if (newlyAdded)
            {
                MonoBehaviour runner = player.GetComponent<MonoBehaviour>(); // �ڷ�ƾ �����
                if (runner != null)
                    runner.StartCoroutine(RemoveRigidbodyAfterDelay(obj.gameObject, rb, 7f));
            }

            totalShrinkAmount += pickup.GetGrowthAmount();
            droppedObjects.Add(obj);
        }

        if (totalShrinkAmount > 0f)
        {
            player.ShrinkBy(totalShrinkAmount);
        }

        return droppedObjects;
    }

    // ���� �ð� �� Rigidbody ����
    private IEnumerator RemoveRigidbodyAfterDelay(GameObject obj, Rigidbody rb, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (obj != null && rb != null)
        {
            Object.Destroy(rb); // Rigidbody�� ����
        }
    }



    /// <summary>
    /// �÷��̾�� �ε�ģ �� ƨ���� ������ �ϴ� �Լ�
    /// ������ٵ� ������ ��ġ�� ��¦ �о�� ��� ����
    /// </summary>
    private void BounceObjects(PlayerController player, List<Transform> droppedObjects)
    {
        Rigidbody playerRb = player.GetComponent<Rigidbody>();
        float scaledBounceForce = bounceForce * player.GetRadius();
        // �÷��̺� ƨ���� ���⿡ Y�� ���� ���� �߰� (0.5f ���� �÷��ֱ�)
        Vector3 bounceDir = (player.transform.position - transform.position).normalized + Vector3.up * 0.5f;
        bounceDir.Normalize();

        // �÷��̾� �� ƨ�ܳ���
        if (playerRb != null)
        {
            playerRb.AddForce(bounceDir * scaledBounceForce, ForceMode.Impulse);
        }
        else
        {
            player.transform.position += bounceDir * 0.5f;
        }

        foreach (var objTransform in droppedObjects)
        {
            Rigidbody rb = objTransform.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // �÷��̾� �߽��� �������� ���� ���
                Vector3 baseDir = (objTransform.position - player.transform.position).normalized;

                // ���� Ȯ�� �߰� (�� �ڿ�������)
                Vector3 randomSpread = new Vector3(
                    Random.Range(-0.3f, 0.3f),
                    Random.Range(0.6f, 1.0f),  // ���� ƨ��� ���� ����
                    Random.Range(-0.3f, 0.3f)
                );

                Vector3 forceDir = (baseDir + randomSpread).normalized;

                rb.isKinematic = false;
                rb.useGravity = true;
                rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                rb.detectCollisions = true;
               
                // ��¦ ����� ���� ����
                objTransform.position += Vector3.up * 0.2f;

                // �����ϸ��� �� ����
                rb.AddForce(forceDir * scaledBounceForce*4, ForceMode.VelocityChange);
                
            }
            else
            {
                // ������ٵ� ���� ��쵵 ����� �������� ���ϰ� �б�
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
    /// �÷��̾ ���� �� ȣ��Ǵ� �޼��� (��� ������)
    /// </summary>
    public void OnAttachedToPlayer()
    {
        isAttached = true;
        StopAllCoroutines();
    }

    private IEnumerator AddTrailAndRemove(GameObject obj, float duration)
    {
        // Trail Renderer ������Ʈ �߰�
        TrailRenderer trail = obj.AddComponent<TrailRenderer>();

        // Ʈ���� �⺻ ����
        trail.time = 0.5f; // �ܻ� ���� �ð�
        trail.startWidth = 0.01f;
        trail.endWidth = 0f;
        trail.material = trailMaterial;
        trail.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        trail.receiveShadows = false;

        // ���� �ð� �� Ʈ���� ����
        yield return new WaitForSeconds(duration);

        if (trail != null)
        {
            Destroy(trail);
        }
    }

}
