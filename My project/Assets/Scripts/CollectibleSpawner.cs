using UnityEngine;

public class CollectibleSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject starPrefab;
    public GameObject diamondPrefab;

    [Header("Spawn Settings")]
    public float minInterval = 3f;
    public float maxInterval = 7f;
    public float spawnX = 12f;
    public float minY = -2f;
    public float maxY = 2f;
    public float diamondChance = 0.3f;

    float timer;
    float nextSpawn;

    void Start() => ScheduleNextSpawn();

    void Update()
    {
        if (GameManager.Instance.IsGameOver()) return;

        timer += Time.deltaTime;
        if (timer >= nextSpawn)
        {
            SpawnCollectible();
            timer = 0f;
            ScheduleNextSpawn();
        }
    }

    void ScheduleNextSpawn()
    {
        nextSpawn = Random.Range(minInterval, maxInterval);
    }

    void SpawnCollectible()
    {
        float y = Random.Range(minY, maxY);
        Vector3 pos = new Vector3(spawnX, y, 0);

        bool spawnDiamond = Random.value < diamondChance;

        if (spawnDiamond && diamondPrefab != null)
        {
            GameObject d = Instantiate(diamondPrefab, pos, Quaternion.identity);
            CollectibleItem item = d.GetComponent<CollectibleItem>();
            if (item != null)
            {
                item.type = CollectibleItem.CollectibleType.Diamond;
                item.pointValue = 50;
            }
        }
        else if (starPrefab != null)
        {
            GameObject s = Instantiate(starPrefab, pos, Quaternion.identity);
            CollectibleItem item = s.GetComponent<CollectibleItem>();
            if (item != null)
            {
                item.type = CollectibleItem.CollectibleType.Star;
                item.pointValue = 10;
            }
        }
    }
}