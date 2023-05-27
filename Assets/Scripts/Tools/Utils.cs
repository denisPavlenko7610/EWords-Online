using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace EWords
{
    public static class Utils
    {
        public static int GetRandomNumber(List<string> words) => Random.Range(0, words.Count);
    }
}