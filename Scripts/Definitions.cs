using UnityEngine;
using System.Collections.Generic;

namespace Definitions
{
    public delegate void Void();
    public enum Choice { None, Left, Right };
    public struct Page
    {
        public int number;
        public Choice playerChoice;
    }

    public static class GameState
    {
        public static int firstPageNumber = 1;
        public static List<Page> pages = new List<Page>();
        public static Page previousPage;

        public static bool PushBackPreviousPage()
        {
            if (previousPage.number == firstPageNumber || previousPage.number != pages[pages.Count - 1].number)
            {
                pages.Add(previousPage);
                Debug.Log("Page " + previousPage.number + " | playerChoice = " + previousPage.playerChoice + "\nRecorded and pushed to stack.");
                return true;
            }
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
