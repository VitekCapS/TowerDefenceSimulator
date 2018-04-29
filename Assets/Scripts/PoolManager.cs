using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    [System.Serializable]
    public class PreloadedPrefab
    {

        public GameObject Prefab = null;    // префаб
        public int Amount = 15;             // кол-во предварительно созданных экземпляров

    }

    public List<PreloadedPrefab> PreloadedPrefabs = new List<PreloadedPrefab>();

    protected Dictionary<string, List<GameObject>> m_AvailableObjects = new Dictionary<string, List<GameObject>>(); //доступные объекты в пулах.
    protected Dictionary<string, int> m_UniquePrefabNames = new Dictionary<string, int>();  // используется для правильной идентификации

    protected static PoolManager m_Instance = null;
    public static PoolManager Instance { get { return m_Instance; } }

    protected virtual void Awake()
    {

        m_Instance = this;

    }

    protected virtual void Start()
    {

        // добавляем все экземпляры объектов, которые должны быть предзагружены в пул
        foreach (PreloadedPrefab obj in PreloadedPrefabs)
        {
            if (obj == null)
                continue;
            if (obj.Prefab == null)
                continue;
            if (obj.Amount < 1)
                continue;
            AddToPool(obj.Prefab, Vector3.zero, Quaternion.identity, obj.Amount);
        }

    }


    /// <summary>
    /// Добавление одного или нескольких копий префаба в пул
    /// </summary>
    public virtual void AddToPool(GameObject prefab, Vector3 position, Quaternion rotation, int amount = 1)
    {

        if (prefab == null)
            return;

        string uniqueName = GetUniqueNameOf(prefab);

        // если нет пула с таким префабом, то создаем его
        if (!m_AvailableObjects.ContainsKey(uniqueName))
            m_AvailableObjects.Add(uniqueName, new List<GameObject>());

        // создаем объекты в пуле
        for (int i = 0; i < amount; i++)
        {
            GameObject newObj = GameObject.Instantiate(prefab, position, rotation) as GameObject;
            newObj.name = uniqueName;
            newObj.transform.parent = transform;
            newObj.SetActive(false);
            m_AvailableObjects[uniqueName].Add(newObj);
        }

    }

    /// <summary>
    /// Спавним объект в сцене. Если есть доступный в пуле, то активируется он.
    /// Если же нет, то создается новый экземпляр объекта.
    /// </summary>
    public static GameObject Spawn(GameObject original, Vector3 position, Quaternion rotation)
    {

        if (Instance == null)
        {
            return null;
        }
        return Instance.SpawnInternal(original, position, rotation);
    }

    public virtual GameObject SpawnInternal(GameObject prefab, Vector3 position, Quaternion rotation)
    {

        if (prefab == null)
            return null;

        GameObject go = null;
        List<GameObject> availableObjects = null;

        string uniqueName = GetUniqueNameOf(prefab);

        if (m_AvailableObjects.TryGetValue(uniqueName, out availableObjects))
        {

            Retry:

            if (availableObjects.Count < 1)
                goto SpawnNew;

            go = availableObjects[0];

            if (go == null)
            {
                availableObjects.Remove(go);
                goto Retry;
            }

            go.transform.position = position;
            go.transform.rotation = rotation;

            availableObjects.Remove(go);

            go.SetActive(true);
            for (int i = 0; i < go.transform.childCount; i++)
            {
                Transform child = go.transform.GetChild(i);
                if (child != null)
                    child.gameObject.SetActive(true);
            }

            if (go.transform.parent == transform)
                go.transform.parent = null;

            return go;

        }

        SpawnNew:

        // добавляем новый объект если он не был найден в пуле
        AddToPool(prefab, position, rotation);

        // возвращаем объект повторным вызовом функции
        return SpawnInternal(prefab, position, rotation);

    }


    /// <summary>
    /// Отключение объекта и возвращение его в пул                                      
    /// </summary>
    public static void Despawn(GameObject obj)
    {
        Instance.DespawnInternal(obj);
    }

    protected virtual void DespawnInternal(GameObject obj)
    {
        if (obj == null)
            return;

        List<GameObject> availableObjects = null;
        string objName = obj.name;
        bool isChild = false;
        Retry:
        m_AvailableObjects.TryGetValue(objName, out availableObjects);

        if (availableObjects == null)
        {
            if (obj.transform.parent != null)
            {
                isChild = true;
                objName = obj.transform.parent.name;
                goto Retry;
            }

            Destroy(obj);
            return;
        }

        obj.SetActive(false);

        if (isChild)
            return;
        if (obj.transform.parent == null)
            obj.transform.parent = transform;

        availableObjects.Add(obj);

    }

    protected string GetUniqueNameOf(GameObject prefab)
    {
        int id;
        if (m_UniquePrefabNames.TryGetValue(prefab.name, out id))
        {

            if (prefab.GetInstanceID() == id)
                return prefab.name;

            string newName = string.Join(" ", new string[] { prefab.name, prefab.GetInstanceID().ToString() });
            if (!m_UniquePrefabNames.ContainsKey(newName))
                m_UniquePrefabNames.Add(newName, prefab.GetInstanceID());
            return newName;

        }

        m_UniquePrefabNames.Add(prefab.name, prefab.GetInstanceID());

        return prefab.name;

    }
}
