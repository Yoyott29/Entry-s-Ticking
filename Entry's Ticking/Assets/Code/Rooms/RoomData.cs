using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomData : MonoBehaviour
{
    public Vector3Int spawnTile;
    public Vector3Int exitTile;
    public Tilemap collisionTilemap;
    public List<RecoverableWord> wordSlots;

    public void AssignWords(List<string> words, List<string> alreadyLostWords)
    {
        for (int i = 0; i < wordSlots.Count; i++)
        {
            if (wordSlots[i] == null)
                continue;

            if (i >= words.Count) {
                Destroy(wordSlots[i].gameObject);
                continue;
            }

            string word = words[i];

            if (alreadyLostWords.Contains(word))
                Destroy(wordSlots[i].gameObject);
            else
                wordSlots[i].SetWord(word);
        }
    }
}
