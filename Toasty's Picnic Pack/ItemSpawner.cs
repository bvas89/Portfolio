using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public GameObject[] collectibles;
    public WeightedItem[] weightedItems = {
        new WeightedItem("Nothing", 0.60f),
        new WeightedItem("Coin", 0.30f),
        new WeightedItem("Item", 0.10f)
    };

    public WeightedItemEnum wie;
    public enum WeightedItemEnum {Nothing, Coin, Item}

    public WI[] items = {
        new WI(WeightedItemEnum.Nothing, 0.50f),
        new WI(WeightedItemEnum.Coin, 0.33f),
        new WI(WeightedItemEnum.Item, 0.17f)
    };

    public void GetSpawnDelay(float delay)
    {
        StartCoroutine(ItemSpawn(delay));
    }

    private IEnumerator ItemSpawn(float delay)
    {
        yield return new WaitForSeconds(delay);
        //SpawnItem();
        Spawn();
    }

    private void Spawn()
    {
        var selectItem = Select();
        print("Select: " + selectItem);

        if (selectItem != WeightedItemEnum.Nothing)
        {
            float screenHeight = Camera.main.orthographicSize * 1.9f;
            float screenWidth = screenHeight * Camera.main.aspect;

            float posY = Random.Range(-screenHeight / 2f, screenHeight / 2f);

            Vector3 spawnPosition = new Vector3(screenWidth, posY, transform.position.z);

            GameObject go = null;
            if (selectItem == WeightedItemEnum.Coin) go = collectibles[0];
            else if (selectItem == WeightedItemEnum.Item) go = collectibles[1];
            else print("NADA");

            Instantiate(go, spawnPosition, Quaternion.identity);
        }
    }

    private void SpawnItem()
    {
        var selectItem = SelectItem();
        if (selectItem != "Nothing")
        {

            float screenHeight = Camera.main.orthographicSize * 1.9f;
            float screenWidth = screenHeight * Camera.main.aspect;

            float posY = Random.Range(-screenHeight / 2f, screenHeight / 2f);

            Vector3 spawnPosition = new Vector3(screenWidth, posY, transform.position.z);

            GameObject go = null;
            if (selectItem == "Coin") go = collectibles[0];
            if (selectItem == "Item") go = collectibles[1];

            Instantiate(go, spawnPosition, Quaternion.identity);
        }
       

        //GameObject item = Instantiate(items[0], spawnPosition, Quaternion.identity);
    }

    public string SelectItem()
    {
        float totalWeight = 0f;

        // Calculate the total weight of all items
        foreach (var weightedItem in weightedItems)
        {
            totalWeight += weightedItem.weight;
        }

        float randomValue = UnityEngine.Random.Range(0f, totalWeight);

        // Iterate over the items and choose based on the weighted probability
        foreach (var weightedItem in weightedItems)
        {
            if (randomValue < weightedItem.weight)
            {
                return weightedItem.itemName;
            }

            randomValue -= weightedItem.weight;
        }

        // Default case, in case the weights do not add up correctly
        return "Nothing";
    }

    public WeightedItemEnum Select()
    {
        float totalWeight = 0f;

        // Calculate the total weight of all items
        foreach (var weightedItem in items)
        {
            totalWeight += weightedItem.weight;
        }

        float randomValue = UnityEngine.Random.Range(0f, totalWeight);

        // Iterate over the items and choose based on the weighted probability
        foreach (var weightedItem in items)
        {
            if (randomValue < weightedItem.weight)
            {
                return weightedItem.wn;
            }

            randomValue -= weightedItem.weight;
        }

        // Default case, in case the weights do not add up correctly
        return WeightedItemEnum.Nothing;
    }

    [System.Serializable]
    public class WeightedItem
    {
        public string itemName;
        public float weight;

        public WeightedItem(string itemName, float weight)
        {
            this.itemName = itemName;
            this.weight = weight;
        }
    }

    [System.Serializable]
    public class WI
    {
        public WeightedItemEnum wn;
        public float weight;

        public WI(WeightedItemEnum name, float weight)
        {
            this.wn = name;
            this.weight = weight;
        }
    }
}
