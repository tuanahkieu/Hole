using UnityEngine;
using UnityEditor;

public class ItemPlacerWindow : EditorWindow
{
    public enum ShapeType { Circle, Grid, Line, Cube, RectangularBox, Cylinder }

    private GameObject prefabToSpawn;
    private ShapeType currentShape = ShapeType.Circle;

    // Common
    private Transform parentTransform;
    private Vector3 centerPosition = Vector3.zero;
    private bool autoVerticalSpacing = true;
    private bool alignToGround = true;

    // Circle Settings
    private int numberOfItems = 10;
    private float radius = 2f;
    private int circleRings = 1;
    private float ringSpacing = 1.5f;

    // Grid Settings
    private int rows = 5;
    private int columns = 5;
    private float spacing = 1.5f;

    // Line Settings
    private int lineItems = 10;
    private float lineSpacing = 1.5f;
    private Vector3 lineDirection = Vector3.right;

    // Cube Settings
    private int cubeSize = 3;
    private float cubeSpacingHorizontal = 1.5f;
    private float cubeSpacingVertical = 1.0f;

    // Rectangular Box Settings
    private int boxSizeX = 3;
    private int boxSizeY = 3;
    private int boxSizeZ = 3;
    private float boxSpacingHorizontal = 1.5f;
    private float boxSpacingVertical = 1.0f;

    // Cylinder Settings
    private int cylinderLayers = 3;
    private float layerHeight = 1.5f;
    private int itemsPerLayer = 10;
    private float cylinderRadius = 2f;
    private int cylinderRings = 1;
    private float cylinderRingSpacing = 1.5f;

    [MenuItem("Tools/Quick Item Placer")]
    public static void ShowWindow()
    {
        GetWindow<ItemPlacerWindow>("Quick Item Placer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Settings", EditorStyles.boldLabel);

        prefabToSpawn = (GameObject)EditorGUILayout.ObjectField("Prefab to Spawn", prefabToSpawn, typeof(GameObject), false);
        parentTransform = (Transform)EditorGUILayout.ObjectField("Parent (Optional)", parentTransform, typeof(Transform), true);
        centerPosition = EditorGUILayout.Vector3Field("Center Position", centerPosition);
        
        // Auto Spacing for 3D
        if (currentShape == ShapeType.Cube || currentShape == ShapeType.RectangularBox || currentShape == ShapeType.Cylinder)
        {
            autoVerticalSpacing = EditorGUILayout.Toggle("Auto Vertical Spacing", autoVerticalSpacing);
        }
        alignToGround = EditorGUILayout.Toggle("Auto Align To Ground", alignToGround);

        EditorGUILayout.Space();

        currentShape = (ShapeType)EditorGUILayout.EnumPopup("Shape", currentShape);

        EditorGUILayout.Space();

        switch (currentShape)
        {
            case ShapeType.Circle:
                numberOfItems = EditorGUILayout.IntSlider("Items (Inner Ring)", numberOfItems, 1, 100);
                radius = EditorGUILayout.Slider("Inner Radius", radius, 0.1f, 50f);
                circleRings = EditorGUILayout.IntSlider("Number of Rings", circleRings, 1, 20);
                if (circleRings > 1) ringSpacing = EditorGUILayout.Slider("Ring Spacing", ringSpacing, 0.1f, 10f);
                break;
            case ShapeType.Grid:
                rows = EditorGUILayout.IntSlider("Rows", rows, 1, 50);
                columns = EditorGUILayout.IntSlider("Columns", columns, 1, 50);
                spacing = EditorGUILayout.Slider("Spacing", spacing, 0.1f, 20f);
                break;
            case ShapeType.Line:
                lineItems = EditorGUILayout.IntSlider("Number of Items", lineItems, 1, 100);
                lineSpacing = EditorGUILayout.Slider("Spacing", lineSpacing, 0.1f, 20f);
                lineDirection = EditorGUILayout.Vector3Field("Direction", lineDirection).normalized;
                break;
            case ShapeType.Cube:
                cubeSize = EditorGUILayout.IntSlider("Size (X,Y,Z)", cubeSize, 1, 20);
                cubeSpacingHorizontal = EditorGUILayout.Slider("Horizontal Spacing", cubeSpacingHorizontal, 0.1f, 20f);
                if (!autoVerticalSpacing) cubeSpacingVertical = EditorGUILayout.Slider("Vertical Spacing", cubeSpacingVertical, 0f, 20f);
                break;
            case ShapeType.RectangularBox:
                boxSizeX = EditorGUILayout.IntSlider("Width (X)", boxSizeX, 1, 20);
                boxSizeY = EditorGUILayout.IntSlider("Height (Y)", boxSizeY, 1, 20);
                boxSizeZ = EditorGUILayout.IntSlider("Depth (Z)", boxSizeZ, 1, 20);
                boxSpacingHorizontal = EditorGUILayout.Slider("Horizontal Spacing", boxSpacingHorizontal, 0.1f, 20f);
                if (!autoVerticalSpacing) boxSpacingVertical = EditorGUILayout.Slider("Vertical Spacing", boxSpacingVertical, 0f, 20f);
                break;
            case ShapeType.Cylinder:
                itemsPerLayer = EditorGUILayout.IntSlider("Items (Inner Ring)", itemsPerLayer, 1, 50);
                cylinderLayers = EditorGUILayout.IntSlider("Number of Layers (Y)", cylinderLayers, 1, 50);
                cylinderRadius = EditorGUILayout.Slider("Inner Radius", cylinderRadius, 0.1f, 50f);
                if (!autoVerticalSpacing) layerHeight = EditorGUILayout.Slider("Layer Height", layerHeight, 0.1f, 20f);
                cylinderRings = EditorGUILayout.IntSlider("Number of Rings", cylinderRings, 1, 20);
                if (cylinderRings > 1) cylinderRingSpacing = EditorGUILayout.Slider("Ring Spacing", cylinderRingSpacing, 0.1f, 10f);
                break;
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("Spawn Items", GUILayout.Height(40)))
        {
            SpawnItems();
        }

        if (GUILayout.Button("Clear Placed Items (Undoable)"))
        {
            // Note: Cntrl-Z will work for Spawned items because we use Undo.RegisterCreatedObjectUndo
            Debug.Log("Use Ctrl+Z / Cmd+Z to undo placement.");
        }
    }

    private void SpawnItems()
    {
        if (prefabToSpawn == null)
        {
            EditorUtility.DisplayDialog("Error", "Please assign a prefab to spawn.", "OK");
            return;
        }

        // Create a temporary parent if we don't have one, just to group this batch
        GameObject batchGroup = new GameObject($"Batch_{prefabToSpawn.name}_{currentShape}");
        if (parentTransform != null) batchGroup.transform.SetParent(parentTransform);
        batchGroup.transform.position = centerPosition;
        Undo.RegisterCreatedObjectUndo(batchGroup, "Spawn Batch Items");

        float calculatedHeight = autoVerticalSpacing ? GetPrefabHeight(prefabToSpawn) : 1f;
        float pivotOffset = alignToGround ? GetPrefabPivotOffset(prefabToSpawn) : 0f;
        
        Vector3 originalCenter = centerPosition;
        centerPosition += new Vector3(0, pivotOffset, 0);

        switch (currentShape)
        {
            case ShapeType.Circle:
                SpawnCircle(batchGroup.transform);
                break;
            case ShapeType.Grid:
                SpawnGrid(batchGroup.transform);
                break;
            case ShapeType.Line:
                SpawnLine(batchGroup.transform);
                break;
            case ShapeType.Cube:
                Spawn3DGrid(batchGroup.transform, cubeSize, cubeSize, cubeSize, cubeSpacingHorizontal, autoVerticalSpacing ? calculatedHeight : cubeSpacingVertical);
                break;
            case ShapeType.RectangularBox:
                Spawn3DGrid(batchGroup.transform, boxSizeX, boxSizeY, boxSizeZ, boxSpacingHorizontal, autoVerticalSpacing ? calculatedHeight : boxSpacingVertical);
                break;
            case ShapeType.Cylinder:
                SpawnCylinder(batchGroup.transform, autoVerticalSpacing ? calculatedHeight : layerHeight);
                break;
        }
        
        centerPosition = originalCenter;
        
        Debug.Log($"Spawned items in {currentShape} pattern.");
    }

    private void SpawnCircle(Transform parent)
    {
        for (int r = 0; r < circleRings; r++)
        {
            float currentRadius = radius + r * ringSpacing;
            int itemsInThisRing = Mathf.Max(1, Mathf.RoundToInt(numberOfItems * (currentRadius / radius)));
            float angleStep = 360f / itemsInThisRing;
            
            for (int i = 0; i < itemsInThisRing; i++)
            {
                float angle = i * angleStep * Mathf.Deg2Rad;
                Vector3 pos = new Vector3(Mathf.Cos(angle) * currentRadius, 0, Mathf.Sin(angle) * currentRadius) + centerPosition;
                InstantiatePrefab(pos, parent);
            }
        }
    }

    private void SpawnGrid(Transform parent)
    {
        float startX = -((columns - 1) * spacing) / 2f;
        float startZ = -((rows - 1) * spacing) / 2f;

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                Vector3 pos = new Vector3(startX + c * spacing, 0, startZ + r * spacing) + centerPosition;
                InstantiatePrefab(pos, parent);
            }
        }
    }

    private void SpawnLine(Transform parent)
    {
        Vector3 dir = lineDirection == Vector3.zero ? Vector3.right : lineDirection;
        float startOffset = -((lineItems - 1) * lineSpacing) / 2f;
        
        for (int i = 0; i < lineItems; i++)
        {
            Vector3 pos = centerPosition + dir * (startOffset + i * lineSpacing);
            InstantiatePrefab(pos, parent);
        }
    }

    private void Spawn3DGrid(Transform parent, int sizeX, int sizeY, int sizeZ, float spacingHorizontal, float spacingVertical)
    {
        float startX = -((sizeX - 1) * spacingHorizontal) / 2f;
        float startZ = -((sizeZ - 1) * spacingHorizontal) / 2f;

        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                for (int z = 0; z < sizeZ; z++)
                {
                    Vector3 pos = new Vector3(startX + x * spacingHorizontal, y * spacingVertical, startZ + z * spacingHorizontal) + centerPosition;
                    InstantiatePrefab(pos, parent);
                }
            }
        }
    }

    private void SpawnCylinder(Transform parent, float heightSpacing)
    {
        for (int y = 0; y < cylinderLayers; y++)
        {
            for (int r = 0; r < cylinderRings; r++)
            {
                float currentRadius = cylinderRadius + r * cylinderRingSpacing;
                int itemsInThisRing = Mathf.Max(1, Mathf.RoundToInt(itemsPerLayer * (currentRadius / cylinderRadius)));
                float angleStep = 360f / itemsInThisRing;
                
                for (int i = 0; i < itemsInThisRing; i++)
                {
                    float angle = i * angleStep * Mathf.Deg2Rad;
                    Vector3 pos = new Vector3(Mathf.Cos(angle) * currentRadius, y * heightSpacing, Mathf.Sin(angle) * currentRadius) + centerPosition;
                    InstantiatePrefab(pos, parent);
                }
            }
        }
    }

    private float GetPrefabHeight(GameObject prefab)
    {
        if (prefab == null) return 1f;

        // Instantiate temporarily to get accurate bounds
        GameObject tempObj = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        if (tempObj == null) tempObj = Instantiate(prefab);

        tempObj.transform.position = Vector3.zero;
        tempObj.transform.rotation = Quaternion.identity;

        float maxHeight = 0f;
        Renderer[] renderers = tempObj.GetComponentsInChildren<Renderer>();
        foreach (var r in renderers)
        {
            if (r.bounds.size.y > maxHeight)
                maxHeight = r.bounds.size.y;
        }

        if (maxHeight == 0f)
        {
            Collider[] colliders = tempObj.GetComponentsInChildren<Collider>();
            foreach (var c in colliders)
            {
                if (c.bounds.size.y > maxHeight)
                    maxHeight = c.bounds.size.y;
            }
        }

        DestroyImmediate(tempObj);
        return maxHeight > 0f ? maxHeight : 1f;
    }

    private float GetPrefabPivotOffset(GameObject prefab)
    {
        if (prefab == null) return 0f;

        GameObject tempObj = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        if (tempObj == null) tempObj = Instantiate(prefab);

        tempObj.transform.position = Vector3.zero;
        tempObj.transform.rotation = Quaternion.identity;

        float minBoundY = float.MaxValue;
        
        Renderer[] renderers = tempObj.GetComponentsInChildren<Renderer>();
        foreach (var r in renderers)
        {
            if (r.bounds.min.y < minBoundY)
                minBoundY = r.bounds.min.y;
        }

        if (minBoundY == float.MaxValue)
        {
            Collider[] colliders = tempObj.GetComponentsInChildren<Collider>();
            foreach (var c in colliders)
            {
                if (c.bounds.min.y < minBoundY)
                    minBoundY = c.bounds.min.y;
            }
        }

        DestroyImmediate(tempObj);
        return minBoundY == float.MaxValue ? 0f : -minBoundY;
    }

    private void InstantiatePrefab(Vector3 position, Transform parent)
    {
        GameObject newObj = (GameObject)PrefabUtility.InstantiatePrefab(prefabToSpawn);
        newObj.transform.position = position;
        newObj.transform.SetParent(parent);
        
        Undo.RegisterCreatedObjectUndo(newObj, "Spawn Item");
    }
}
