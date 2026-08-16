using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance;

    public enum PoolType { FloatingText } // Có thể thêm AppleIcon, StrawberryIcon sau

    [System.Serializable]
    public class PoolConfig
    {
        public PoolType type;
        public GameObject prefab;
        public int defaultCapacity = 20;
        public int maxSize = 100;
    }

    public List<PoolConfig> poolConfigs;
    private Dictionary<PoolType, ObjectPool<GameObject>> poolDictionary;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        poolDictionary = new Dictionary<PoolType, ObjectPool<GameObject>>();

        foreach (var config in poolConfigs)
        {
            ObjectPool<GameObject> newPool = new ObjectPool<GameObject>(
                createFunc: () => Instantiate(config.prefab, transform),
                actionOnGet: (obj) => obj.SetActive(true),
                actionOnRelease: (obj) => obj.SetActive(false),
                actionOnDestroy: (obj) => Destroy(obj),
                collectionCheck: false,
                defaultCapacity: config.defaultCapacity,
                maxSize: config.maxSize
            );
            poolDictionary.Add(config.type, newPool);
        }
    }

    public GameObject Spawn(PoolType type, Vector3 position)
    {
        if (poolDictionary.ContainsKey(type))
        {
            GameObject obj = poolDictionary[type].Get();
            obj.transform.position = position;
            return obj;
        }
        return null;
    }

    public void Despawn(PoolType type, GameObject obj)
    {
        if (poolDictionary.ContainsKey(type))
            poolDictionary[type].Release(obj);
    }
}