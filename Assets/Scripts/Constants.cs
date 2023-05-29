using UnityEngine;

namespace EWords
{
    public class Constants
    {
        public static readonly string Path = Application.dataPath + "/Resources/Text/Words.txt";
        public static readonly string LearnedPath = Application.persistentDataPath + "/LearnedWords.dat";

        public const string WordsListIsEmpty = "Words list is empty";
        public const string API = "AIzaSyBDzFypp2m8DUJaUEjqkBxMUwFj6Rj3IVo";
        public const string Cx = "f6f738af933044457";
    }
}