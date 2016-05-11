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
            return "name: " + sceneName + ", prev: [" + previousPage.number + "," + previousPage.playerChoice + "]";
        }
    }

    public static class Persistor
    {
        static string path = Application.persistentDataPath + "/checkpoint.sav";

        public static void Save(string sceneName)
        {
            var checkpoint = new Checkpoint(sceneName, GameState.previousPage, GameState.pages);
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(path);
            bf.Serialize(file, checkpoint);
            file.Close();

            if (GameManager.Instance.DebugLevel >= DebugLevel.Notify)
            {
                Debug.Log("Checkpoint " + checkpoint + " saved.");
                foreach (Page page in GameState.pages)
                    Debug.Log(page.number + " " + page.playerChoice + " saved.");
            }
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

                if (GameManager.Instance.DebugLevel >= DebugLevel.Notify)
                {
                    Debug.Log("Checkpoint " + checkpoint + " loaded.");
                    foreach (Page page in checkpoint.allPages)
                        Debug.Log(page.number + " choice: " + page.playerChoice);
                }
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
    }

    public static class GameState
    {
        public static int firstPageNumber = 1;
        public static List<Page> pages = new List<Page>();
        public static Page previousPage;

        public static bool PushBackPreviousPage()
        {
            if (previousPage.number == firstPageNumber || (pages.Count > 0 && previousPage.number != pages[pages.Count - 1].number))
            {
                pages.Add(previousPage);
                if (GameManager.Instance.DebugLevel >= DebugLevel.Verbose)
                    Debug.Log("Page " + previousPage.number + " | playerChoice = " + previousPage.playerChoice + "\nRecorded and pushed to stack.");
                return true;
            }

            if (GameManager.Instance.DebugLevel >= DebugLevel.Notify)
                Debug.Log("Ignoring push back: this is not the first/next page.");
            return false;          
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
