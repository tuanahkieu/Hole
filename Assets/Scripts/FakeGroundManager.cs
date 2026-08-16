using UnityEngine;

public class FakeGroundManager : MonoBehaviour
{
    [Tooltip("Kéo thả Transform của cái Hố (Hole) vào đây để script biết khi nào hố to lên")]
    public Transform holeVisual;

    [Tooltip("Bán kính của hố (khoảng cách từ tâm đến mép) khi scale của hố là 1")]
    public float baseRadius = 0.5f;

    [Tooltip("Kích thước của mỗi miếng đất (BoxCollider). Chỉnh dài rộng sao cho bao phủ hết màn hình (ví dụ 50x50)")]
    public Vector3 colliderSize = new Vector3(50f, 1f, 50f);

    [Tooltip("Danh sách 8 cái collider xung quanh")]
    public Transform[] colliders = new Transform[8];

    private Vector3 lastHoleScale;

    void Start()
    {
        // Tự động lấy 8 object con nếu chưa được gán
        if (colliders == null || colliders.Length == 0 || colliders[0] == null)
        {
            int childCount = Mathf.Min(transform.childCount, 8);
            colliders = new Transform[childCount];
            for (int i = 0; i < childCount; i++)
            {
                colliders[i] = transform.GetChild(i);
            }
        }

        UpdateColliders();
        if (holeVisual != null) lastHoleScale = holeVisual.localScale;
    }

    void Update()
    {
        // Kiểm tra liên tục xem hố có bị đổi kích thước không (SizeUp, etc)
        if (holeVisual != null && holeVisual.localScale != lastHoleScale)
        {
            UpdateColliders();
            lastHoleScale = holeVisual.localScale;
        }
    }

    // Nút này cho phép bạn bấm test trực tiếp trong Editor mà không cần Play game
    [ContextMenu("Cập nhật lại khoảng cách")]
    public void UpdateColliders()
    {
        if (colliders == null || colliders.Length == 0) return;

        // Giả sử hố to lên đều đặn, lấy trục X làm chuẩn
        float currentScale = holeVisual != null ? holeVisual.localScale.x : 1f;
        
        // Tính bán kính hiện tại
        float currentRadius = baseRadius * currentScale;

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i] == null) continue;

            // Xoay 8 cái, mỗi cái lệch nhau 45 độ (360 / 8 = 45)
            float angle = i * 45f;
            float angleRad = angle * Mathf.Deg2Rad;

            // Vì tâm của BoxCollider nằm ở giữa hộp, nên ta phải lùi nó ra xa thêm 1 nửa chiều dài (Z) của hộp
            float distanceToCenter = currentRadius + (colliderSize.z / 2f);

            // Tính toán vị trí X và Z dựa vào Sin Cos để tạo thành hình tròn xung quanh hố
            Vector3 pos = new Vector3(Mathf.Sin(angleRad) * distanceToCenter, 0f, Mathf.Cos(angleRad) * distanceToCenter);

            // Set vị trí và góc xoay
            colliders[i].localPosition = pos;
            colliders[i].localRotation = Quaternion.Euler(0f, angle, 0f);

            // Cập nhật lại kích thước của cái BoxCollider
            BoxCollider box = colliders[i].GetComponent<BoxCollider>();
            if (box != null)
            {
                box.size = colliderSize;

                // Tối ưu hóa quan trọng: Đặt ma sát (friction) = 0 để khi hố di chuyển không kéo rê các vật liệu thức ăn đi theo
                if (box.sharedMaterial == null)
                {
                    PhysicsMaterial zeroFrictionMat = new PhysicsMaterial("ZeroFriction");
                    zeroFrictionMat.dynamicFriction = 0f;
                    zeroFrictionMat.staticFriction = 0f;
                    zeroFrictionMat.frictionCombine = PhysicsMaterialCombine.Minimum;
                    box.sharedMaterial = zeroFrictionMat;
                }
            }
        }
    }
}
