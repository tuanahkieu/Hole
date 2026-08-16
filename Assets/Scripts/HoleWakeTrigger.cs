using UnityEngine;

public class HoleWakeTrigger : MonoBehaviour
{
    [Tooltip("Bán kính vùng 'rã đông'. Vật phẩm nào lọt vào vùng này sẽ tự động bật vật lý (rơi xuống).")]
    public float wakeRadius = 3f;
    
    [Tooltip("Tốc độ quét (số lần quét mỗi giây). Quét ít thì game mượt hơn.")]
    public float scanRate = 15f;
    
    private float nextScanTime;
    
    // TỐI ƯU HÓA: Dùng mảng cấp phát sẵn để tránh tạo rác (GC) mỗi lần quét
    private Collider[] hitColliders = new Collider[100];

    void Update()
    {
        // Tối ưu hóa: Không cần quét mỗi khung hình (frame), chỉ quét vài lần 1 giây là đủ
        if (Time.time >= nextScanTime)
        {
            nextScanTime = Time.time + (1f / scanRate);
            
            // Tự động tính bán kính rã đông dựa theo độ to của cái hố (khi hố lên cấp)
            float currentRadius = wakeRadius;
            if (transform.childCount > 0)
            {
                currentRadius *= transform.GetChild(0).localScale.x;
            }

            // Dùng sóng radar hình cột trụ (Capsule) quét từ dưới hố đâm thẳng lên trời 50m
            // Để đảm bảo tháp bánh cao bao nhiêu cũng bị gọi dậy hết!
            Vector3 bottomPoint = transform.position + Vector3.down * 2f;
            Vector3 topPoint = transform.position + Vector3.up * 50f;
            int count = Physics.OverlapCapsuleNonAlloc(bottomPoint, topPoint, currentRadius, hitColliders);
            for (int i = 0; i < count; i++)
            {
                var col = hitColliders[i];
                // Nếu quét trúng đồ ăn
                if (col.CompareTag("food") || col.GetComponent<EatableItem>() != null)
                {
                    // Lấy Rigidbody của đồ ăn
                    Rigidbody rb = col.GetComponentInParent<Rigidbody>();
                    
                    // Nếu đồ ăn đang bị đóng băng (isKinematic = true)
                    if (rb != null && rb.isKinematic)
                    {
                        // Rã đông: Trả lại vật lý bình thường
                        rb.isKinematic = false;
                        
                        // TUYỆT CHIÊU TRỊ RUNG LẮC (PHYSICS POPPING):
                        // 1. Giới hạn lực đẩy lùi khi các vật phẩm lỡ đè lên nhau (chống nảy)
                        rb.maxDepenetrationVelocity = 0.5f; 
                        
                        // 2. Bật làm mượt chuyển động để che giấu các giật lag của physics
                        rb.interpolation = RigidbodyInterpolation.Interpolate;
                        
                        rb.WakeUp();
                    }
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        
        float currentRadius = wakeRadius;
        if (transform.childCount > 0)
        {
            currentRadius *= transform.GetChild(0).localScale.x;
        }

        Vector3 bottomPoint = transform.position + Vector3.down * 2f;
        Vector3 topPoint = transform.position + Vector3.up * 50f;
        Gizmos.DrawWireSphere(bottomPoint, currentRadius);
        Gizmos.DrawWireSphere(topPoint, currentRadius);
        Gizmos.DrawLine(bottomPoint + Vector3.left * currentRadius, topPoint + Vector3.left * currentRadius);
        Gizmos.DrawLine(bottomPoint + Vector3.right * currentRadius, topPoint + Vector3.right * currentRadius);
    }
}
