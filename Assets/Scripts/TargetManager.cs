using System.Collections.Generic;
using UnityEngine;

public class TargetManager : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Kéo object Target_bg (nằm trong TimeCount) vào đây")]
    public Transform targetBgParent;

    [Header("Danh sách Target của màn chơi")]
    [Tooltip("Chỉ cần kéo thả các Prefab UI khối màu xanh (Cheesecake, Burger...) vào đây")]
    public List<GameObject> targetPrefabsToSpawn;

    private void Start()
    {
        // Tự động sinh ra UI Target khi bắt đầu game
        SetupTargets();
    }

    public void SetupTargets()
    {
        foreach (Transform child in targetBgParent)
        {
            Destroy(child.gameObject);
        }


        foreach (var prefab in targetPrefabsToSpawn)
        {
            if (prefab != null)
            {
                Instantiate(prefab, targetBgParent);
            }
        }
    }
}
