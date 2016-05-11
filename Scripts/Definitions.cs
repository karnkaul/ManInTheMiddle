using UnityEngine;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Definitions
{
    public delegate void Void();
    public enum Choice { None, Left, Right };
    public enum DebugLevel { None, Errors, Verbose };

    [System.Serializable]
    public struct Page
    {
        public int number;
        public Choice playerChoice;
    }

    // Not much use due to tight coupling of scene buildindex and "page" number
    public static class Persistor
    {
        public static void Save()
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(Application.persistentDataPath + "/savedGames.gd");
            bf.Serialize(file, GameState.pages);
            file.Close();

            if(GameManager.Instance.DebugLevel >= DebugLevel.Verbose)
                foreach(Page page in GameState.pages)
                    Debug.Log(page.number + " " + page.playerChoice + " saved.");
        }

        public static void Load()
        {

            List<Page> loaded = new List<Page>();
            if (File.Exists(Application.persistentDataPath + "/savedGames.gd"))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.OpenRead(Application.persistentDataPath + "/savedGames.gd");
                loaded = (List<Page>)bf.Deserialize(file);
                file.Close();
            }

            if (GameManager.Instance.DebugLevel >= DebugLevel.Verbose)
                foreach (Page page in loaded)
                    Debug.Log(page.number + " choice: " + page.playerChoice);
        }
    }

    [System.Serializable]
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
            if (GameManager.Instance.DebugLevel >= DebugLevel.Errors)
                Debug.Log("Ignoring push back.");
            return false;          
        }

        public static void Reset()
        {
            pages = new List<Page>();
            previousPage = new Page();
            Debug.Log("GameState reset.");
        }
    }
}
