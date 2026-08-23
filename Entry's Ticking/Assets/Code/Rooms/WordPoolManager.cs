using UnityEngine;
using System.Collections.Generic;

public class WordPoolManager : MonoBehaviour
{
    public static WordPoolManager instance;
    List<string> availableWordPool = new();

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    public void InitializePool(List<string> allOriginalWords, string excludeWord)
    {
        availableWordPool = new List<string>(allOriginalWords);
        availableWordPool.Remove(excludeWord);
    }

    public List<string> PickWordsForRoom(int count)
    {
        var picked = new List<string>();
        var poolCopy = new List<string>(availableWordPool);
        int number = Mathf.Min(count, poolCopy.Count);

        for (int i = 0; i < number; i++) {
            int index = Random.Range(0, poolCopy.Count);
            picked.Add(poolCopy[index]);
            poolCopy.RemoveAt(index);
        }
        return picked;
    }

    public void RemoveWordPermanently(string word)
    {
        availableWordPool.Remove(word);
    }
}

