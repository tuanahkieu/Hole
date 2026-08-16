using UnityEngine;

public class OnChangePosition : MonoBehaviour
{
    [SerializeField] private PolygonCollider2D hole2DCollider;
    [SerializeField] private PolygonCollider2D ground2DCollider;
    [SerializeField] private MeshCollider GenerateMeshCollider;
    [SerializeField] private Collider GroundCollider;
    [SerializeField] private float initalScale = 0.5f;
    Mesh GenerateMesh;

    private Transform holeVisual;

    private void Start(){
        holeVisual = transform.Find("Hole");

        GameObject[] AllGOs = FindObjectsOfType(typeof(GameObject)) as GameObject[];
        foreach(GameObject go in AllGOs){
            if(go.layer == LayerMask.NameToLayer("Obstacles")){
                Physics.IgnoreCollision(go.GetComponent<Collider>(), GenerateMeshCollider, true);
            }
        }
    }

    private void OnTriggerEnter(Collider other){
        Physics.IgnoreCollision(other, GroundCollider, true);
        Physics.IgnoreCollision(other, GenerateMeshCollider, false);
    }
    private void OnTriggerExit(Collider other){
        Physics.IgnoreCollision(other, GroundCollider, false);
        Physics.IgnoreCollision(other, GenerateMeshCollider, true);
    }

    
    private void FixedUpdate(){
        if(transform.hasChanged){
            transform.hasChanged = false;

            hole2DCollider.transform.position = new Vector2(transform.position.x, transform.position.z);   
            
            // Lấy scale của visual hole nếu có, ngược lại lấy của chính nó
            Vector3 targetScale = holeVisual != null ? holeVisual.localScale : transform.localScale;
            
            // Tính toán scale thực tế (Global Scale) vì Hole (visual) là con của HoleParent nên bị ảnh hưởng bởi scale của HoleParent
            Vector3 globalVisualScale = new Vector3(
                transform.localScale.x * targetScale.x,
                transform.localScale.y * targetScale.y,
                transform.localScale.z * targetScale.z
            );

            // Ánh xạ X và Z của 3D sang X và Y của 2D, vì cái lỗ 3D bị bóp dẹt ở trục Y (scale Y = 0.01)
            // LƯU Ý: Dùng globalVisualScale thay vì targetScale để lỗ 2D to bằng đúng lỗ 3D
            hole2DCollider.transform.localScale = new Vector3(globalVisualScale.x * initalScale, globalVisualScale.z * initalScale, 1f);
         

            MakeHole2D();
            Make3DMeshCollider();
        }
    }
    private void Update()
    {
        // Kiểm tra xem visual hole có thay đổi scale không để cập nhật mesh
        if (holeVisual != null && holeVisual.hasChanged)
        {
            holeVisual.hasChanged = false;
            // Ép buộc cập nhật lỗ vật lý bằng cách kích hoạt transform.hasChanged
            transform.hasChanged = true; 
        }
    }
    
    private void MakeHole2D(){
        Vector2[] PointPositions = hole2DCollider.GetPath(0);
        for(int i = 0; i < PointPositions.Length; i++){
            PointPositions[i] = hole2DCollider.transform.TransformPoint(PointPositions[i]);
            
        }
        ground2DCollider.pathCount = 2;
        ground2DCollider.SetPath(1, PointPositions);
    }

    private void Make3DMeshCollider(){
        if(GenerateMesh != null) Destroy(GenerateMesh);
        GenerateMesh = ground2DCollider.CreateMesh(true,true);
        GenerateMeshCollider.sharedMesh = GenerateMesh;
        
    }


}
