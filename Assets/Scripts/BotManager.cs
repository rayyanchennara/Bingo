using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BotManager : MonoBehaviour
{
    public List<Button> bingoButtons; // Assign 25 buttons in Inspector, row-wise (top-left to bottom-right)
    List<int> numbers = new List<int>();
    public GameManger gameManger;
    public Image powerUpLoadingImage;
    public Button selectPower;
    public Sprite powerUpBottonOldSprite;
    public Sprite powerUpBottonNewSprite;
    public int powerUpPoint = 0;
    public TextMeshProUGUI powerUpPointText;
    public bool isMagicToolActive = false;

    private void Awake()
    {
        // Initialize players
        AddOneToFirst75(numbers, 75);
        Shuffle(numbers);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AssignBingoNumbers();
    }

    // This function adds numbers from 1 to max (75 in this case) to the list
    void AddOneToFirst75(List<int> list, int max)
    {
        for (int i = 0; i < max; i++)
        {
            list.Add(i + 1); // Add numbers from 1 to max (75 in this case) to the list
        }
    }

    // This function shuffles a list of integers
    void Shuffle(List<int> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randIndex = UnityEngine.Random.Range(0, i + 1); // inclusive of i
            int temp = list[i];
            list[i] = list[randIndex];
            list[randIndex] = temp;
        }
    }

    // This function assigns numbers to the bingo buttons in a BINGO format
    void AssignBingoNumbers()
    {
        // Create lists for each column's number range
        List<int> columnB = GenerateShuffledRange(1, 15);
        List<int> columnI = GenerateShuffledRange(16, 30);
        List<int> columnN = GenerateShuffledRange(31, 45);
        List<int> columnG = GenerateShuffledRange(46, 60);
        List<int> columnO = GenerateShuffledRange(61, 75);
        for (int col = 0; col < 5; col++)
        {
            // Column B (index 0,5,10,15,20)
            SetButtonText(col, columnB[col]);
            // Column I (index 1,6,11,16,21)
            SetButtonText(col + 5, columnI[col]);
            // Column N (index 2,7,12,17,22) — skip middle one for "FREE"
            if (col == 2)
                SetButtonText(col + 10, "");
            else
                SetButtonText(col + 10, columnN[col < 2 ? col : col - 1]);
            // Column G (index 3,8,13,18,23)
            SetButtonText(col + 15, columnG[col]);
            // Column O (index 4,9,14,19,24)
            SetButtonText(col + 20, columnO[col]);
        }
    }

    // This function generates a shuffled range of numbers from min to max
    List<int> GenerateShuffledRange(int min, int max)
    {
        List<int> numbers = new List<int>();
        for (int i = min; i <= max; i++)
        {
            numbers.Add(i);
        }
        // Shuffle
        for (int i = 0; i < numbers.Count; i++)
        {
            int rand = UnityEngine.Random.Range(i, numbers.Count);
            int temp = numbers[i];
            numbers[i] = numbers[rand];
            numbers[rand] = temp;
        }
        return numbers;
    }

    // This function sets the text of a button at a specific index
    void SetButtonText(int index, object content)
    {
        TMP_Text tmpText = bingoButtons[index].GetComponentInChildren<TMP_Text>();
        Num num = bingoButtons[index].GetComponent<Num>();
        // Set the number in the Num component
        if (num != null && int.TryParse(content.ToString(), out int parsedNumber))
        {
            num.number = parsedNumber;
        }
        // Set the text of the button
        if (tmpText != null)
        {
            tmpText.text = content.ToString();
        }
    }

    public void AiMove()
    {
        if (gameManger.currentNum < 16)
        {
            for (int i = 0; i < 5; i++)
            {
                bingoButtons[i].GetComponent<Num>().CheckNumBot();
            }
        }
        else if (gameManger.currentNum > 15 && gameManger.currentNum <= 30)
        {
            for (int i = 5; i < 10; i++)
            {
                bingoButtons[i].GetComponent<Num>().CheckNumBot();
            }
        }
        else if (gameManger.currentNum > 30 && gameManger.currentNum <= 45)
        {
            for (int i = 10; i < 15; i++)
            {
                bingoButtons[i].GetComponent<Num>().CheckNumBot();
            }
        }
        else if (gameManger.currentNum > 45 && gameManger.currentNum <= 60)
        {
            for (int i = 15; i < 20; i++)
            {
                bingoButtons[i].GetComponent<Num>().CheckNumBot();
            }
        }
        else if (gameManger.currentNum > 60 && gameManger.currentNum <= 75)
        {
            for (int i = 20; i < 25; i++)
            {
                bingoButtons[i].GetComponent<Num>().CheckNumBot();
            }
        }

        if (gameManger.WinChecking(bingoButtons))
        {
            // Show the winner UI and stop the game
            Debug.Log("Bot Won!");
            Invoke("RestartGame", 1f);
            Time.timeScale = 0;
            // You can add code here to show a UI or perform any other action when a player wins
        }
        else
        {
            Debug.Log("Please Completed...");
        }
    }

    IEnumerator IncreasePowerUpImage()
    {
        float temp = 0;
        float tempNext = powerUpLoadingImage.fillAmount + .25f;
        while (temp <= 1f)
        {
            temp += Time.deltaTime * 1;
            powerUpLoadingImage.fillAmount = Mathf.Lerp(powerUpLoadingImage.fillAmount, tempNext, temp);
            yield return new WaitForEndOfFrame();
        }
        powerUpLoadingImage.fillAmount = powerUpLoadingImage.fillAmount >= 1f ? 1 : powerUpLoadingImage.fillAmount;
        if (powerUpLoadingImage.fillAmount >= 1f)
        {
            if (!selectPower.enabled)
            {
                selectPower.enabled = true;
                selectPower.gameObject.GetComponent<Image>().sprite = powerUpBottonNewSprite;
            }
            powerUpPoint++;
            powerUpPointText.text = powerUpPoint.ToString();
            powerUpLoadingImage.fillAmount = 0f;
        }

        if (powerUpPoint > 0)
        {
            powerUpPoint--;
            powerUpPointText.text = powerUpPoint.ToString();
            if (powerUpPoint <= 0)
            {
                selectPower.enabled = false;
                selectPower.gameObject.GetComponent<Image>().sprite = powerUpBottonOldSprite;
            }
            isMagicToolActive = true;
            int count = 0;
            int[] ints = CheckWinPosiblity();
            if (ints.Length > 0)
            {
                foreach (int index in ints)
                {
                    if (bingoButtons[index].GetComponent<Num>().numberState == NumberState.Matched)
                    {
                        count++;
                    }
                }
            }
            if(count == 4)
            {
                Debug.Log("Direct");
                MagicTool(CheckWinPosiblity());
            }
            else
            {
                Invoke("CallMagicTool", Random.Range(3f, 7f)); // Call the method after 1 second delay
            }
            // Add your power-up logic here
            // For example, you can change the color of the button or perform any other action
        }
    }

    public void CallMagicTool()
    {
        MagicTool(CheckWinPosiblity());
    }
    private void MagicTool(int[] ints)
    {
        if (ints.Length > 0)
        {
            foreach (int index in ints)
            {
                if (bingoButtons[index].GetComponent<Num>().numberState != NumberState.Matched)
                {
                    bingoButtons[index].GetComponent<Num>().CheckNumBot();
                    return;
                }
            }
        }
    }

    public int[] CheckWinPosiblity()
    {
        int maxUnmatched = -1;
        int[] patternWithMostUnmatched = null;

        int[][] winPatterns = new int[][]
        {
    new int[] {0, 1, 2, 3, 4},
    new int[] {5, 6, 7, 8, 9},
    new int[] {10, 11, 12, 13, 14},
    new int[] {15, 16, 17, 18, 19},
    new int[] {20, 21, 22, 23, 24},
    new int[] {0, 5, 10, 15, 20},
    new int[] {1, 6, 11, 16, 21},
    new int[] {2, 7, 12, 17, 22},
    new int[] {3, 8, 13, 18, 23},
    new int[] {4, 9, 14, 19, 24},
    new int[] {0, 6, 12, 20, 24},
        };

        foreach (var pattern in winPatterns)
        {
            int unmatchedCount = 0;

            foreach (int index in pattern)
            {
                if (bingoButtons[index].GetComponent<Num>().numberState == NumberState.Matched)
                {
                    unmatchedCount++;
                }
            }

            if (unmatchedCount > maxUnmatched)
            {
                maxUnmatched = unmatchedCount;
                patternWithMostUnmatched = pattern;
            }

            // If you want to return early if all are matched:
            if (unmatchedCount == 0)
            {

            }
        }

        // Now you have the pattern with the most unmatched buttons:
        if (patternWithMostUnmatched != null)
        {
            Debug.Log($"Pattern with most unmatched buttons has {maxUnmatched} unmatched elements: [{string.Join(",", patternWithMostUnmatched)}]");
        }

        return patternWithMostUnmatched;
    }

    public void CallIncreasePowerUpImage()
    {
        StartCoroutine(IncreasePowerUpImage());
    }

    public void SelectPower()
    {
        if (powerUpPoint > 0)
        {
            powerUpPoint--;
            powerUpPointText.text = powerUpPoint.ToString();
            if (powerUpPoint <= 0)
            {
                selectPower.enabled = false;
                selectPower.gameObject.GetComponent<Image>().sprite = powerUpBottonOldSprite;
            }
            isMagicToolActive = true;
            // Add your power-up logic here
            // For example, you can change the color of the button or perform any other action
        }
    }
}
