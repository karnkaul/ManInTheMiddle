using UnityEngine;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Definitions
{
    public delegate void Void();
    public enum Choice { None, Left, Right };
    public enum DebugLevel { None, Notify, Verbose };

    [System.Serializable]
    public struct Page
    {
        public int number;
        public Choice playerChoice;
    }

    [System.Serializable]
    public class Checkpoint
    {
        public string sceneName;
        public Page previousPage;
        public List<Page> allPages;

        public Checkpoint(string sceneName, Page previousPage, List<Page> allPages)
        {
            this.sceneName = sceneName;
            this.previousPage = previousPage;
            this.allPages = allPages;
        }

        public override string ToString()
        {
            return "<b>{ " + sceneName + ": [" + previousPage.number + ", " + previousPage.playerChoice + "] }</b>";
        }
    }

    public static class Persistor
    {
        static string _path = (Application.platform == RuntimePlatform.WindowsEditor) ? Application.dataPath : Application.persistentDataPath;
        static string path = _path + "/checkpoint.sav";

        public static void Save(string sceneName)
        {
            var checkpoint = new Checkpoint(sceneName, GameState.previousPage, GameState.pages);
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(path);
            bf.Serialize(file, checkpoint);
            file.Close();

            Log(checkpoint, "saved");
        }

        public static Checkpoint Load()
        {
            Checkpoint checkpoint;
            if (File.Exists(path))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.OpenRead(path);

                checkpoint = (Checkpoint)bf.Deserialize(file);
                GameState.previousPage = checkpoint.previousPage;
                GameState.pages = checkpoint.allPages;

                file.Close();

                Log(checkpoint, "loaded");

                return checkpoint;
            }
            return null;
        }

        public static void Clear()
        {
            if (File.Exists(path))
            {
                File.Delete(path);

                if (GameManager.Instance.DebugLevel >= DebugLevel.Notify)
                    Debug.Log("Checkpoint file deleted.");
                return;
            }
            Debug.Assert(false, "No save file found.");
        }

        static void Log(Checkpoint checkpoint, string action)
        {
            if (GameManager.Instance.DebugLevel >= DebugLevel.Notify)
                Debug.Log("Checkpoint " + checkpoint + " " + action + ".");

            if (GameManager.Instance.DebugLevel >= DebugLevel.Verbose)
                foreach (Page page in checkpoint.allPages)
                    Debug.Log(page.number + " choice: " + page.playerChoice);
        }
    }

    public static class GameState
    {
        public static int firstPageNumber = 1;
        public static List<Page> pages = new List<Page>();
        public static Page previousPage;

        public static void PushBackPreviousPage()
        {
            pages.Add(previousPage);
            if (GameManager.Instance && GameManager.Instance.DebugLevel >= DebugLevel.Notify)
                Debug.Log("Page " + previousPage.number + " | playerChoice = " + previousPage.playerChoice + "\nRecorded and pushed to stack.");
        }

        public static void Reset()
        {
            pages = new List<Page>();
            previousPage = new Page();

            if (GameManager.Instance.DebugLevel >= DebugLevel.Notify)
                Debug.Log("GameState reset.");
        }
    }
}
