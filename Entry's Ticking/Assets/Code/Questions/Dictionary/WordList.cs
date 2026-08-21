using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "WordList", menuName = "List/New WordList")]
public class WordList : ScriptableObject
{
    public List<string> words;
}
