using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Burst;
using UnityEngine;
using UnityEngine.UI;


public class GameManger : MonoBehaviour
{
    public List<Button> bingoButtons; // Assign 25 buttons in Inspector, row-wise (top-left to bottom-right)
    public GameObject roundPrefabe;
    public List<GameObject> rounds = new List<GameObject>();
    public List<int> currentNumbers = new List<int>();
    public RectTransform roundSponePoint;
    public bool instiantiating = true;
    public Transform roundParent;
    public Vector3 position;
    int totalInstantiate = 0;
    public Image powerUpLoadingImage;
    public Button selectPower;
    public int powerUpPoint = 0;
    public TextMeshProUGUI powerUpPointText;
    public int currentNum;
    public GameObject powers;
    public bool isMagicToolActive = false;
    public Sprite powerUpBottonOldSprite;
    public Sprite powerUpBottonNewSprite;
    // tools Sprites
    public Sprite magicToolSprite;
    public Sprite powerToolsDefualtSPrite;

    public Image shaderImage;
    public Image currentTool;

    List<int> numbers = new List<int>();


    private void Awake()
    {
        // add listeners to selectPower button
        selectPower.onClick.AddListener(SelectPower);
        // Initialize players
        AddOneToFirst75(numbers, 75);
        foreach (Transform transform in roundParent)
        {
            rounds.Add(transform.gameObject);
            currentNumbers.Add(0);
        }
        Shuffle(numbers);
    }

    // Start is called before the first frame update
    void Start()
    {
        AssignBingoNumbers();
        StartCoroutine(InstanstiateRound());
    }

    public void CallIncreasePowerUpImage()
    {
        StartCoroutine(IncreasePowerUpImage());
    }

    // This function Increase power Up Image's fill amount by .25 every calling with lerp
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
            if(!selectPower.enabled)
            {
                selectPower.enabled = true;
                selectPower.gameObject.GetComponent<Image>().sprite = powerUpBottonNewSprite;
            }
            powerUpPoint++;
            powerUpPointText.text = powerUpPoint.ToString();
            powerUpLoadingImage.fillAmount = 0f;
        }
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

    /*
    public void MagicTool()
    {
            isMagicToolActive = true;
        currentTool.sprite = magicToolSprite;
        powers.SetActive(false);
    }
    */

    // This function adds numbers from 1 to max (75 in this case) to the list
    void AddOneToFirst75(List<int> list, int max)
    {
        for (int i = 0; i < max; i++)
        {
            list.Add(i + 1); // Add numbers from 1 to max (75 in this case) to the list
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

    // This function instantiates a round prefab and assigns a number to it
    IEnumerator InstanstiateRound()
    {
        while (instiantiating)
        {
            currentNum = numbers[totalInstantiate];
            instiantiating = false;
            GameObject round = Instantiate(roundPrefabe, roundSponePoint);
            round.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = currentNum.ToString();
            if (currentNum <= 15)
            {
                round.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "B";
            }
            else if (currentNum > 15 && currentNum <= 30)
            {
                round.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "I";
            }
            else if (currentNum > 30 && currentNum <= 45)
            {
                round.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "N";
            }
            else if (currentNum > 45 && currentNum <= 60)
            {
                round.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "G";
            }
            else if (currentNum > 60 && currentNum <= 75)
            {
                round.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "O";
            }
            Invoke("CallAi", UnityEngine.Random.Range(8,15));
            shaderImage.fillAmount = 0;
            round.transform.SetParent(roundSponePoint);
            float elapsedTime = 0;
            float duration = 5f;
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / duration;
                shaderImage.fillAmount = Mathf.Lerp(0, 1, t);
                yield return new WaitForEndOfFrame();
            }
            yield return new WaitForEndOfFrame();
            // Destroy the oldest round if there are more than 3
            if (rounds.Count > 3)
            {
                Destroy(rounds[0]);
                rounds.RemoveAt(0);
                currentNumbers.RemoveAt(0);
            }

            // change the font size of the text in the round prefab
            currentNumbers.Add(currentNum);
            round.transform.GetChild(0).GetComponent<TextMeshProUGUI>().fontSize = 35;
            round.transform.GetChild(1).GetComponent<TextMeshProUGUI>().fontSize = 35;
            round.transform.SetParent(roundParent);
            rounds.Add(round);
            totalInstantiate++;
            instiantiating = totalInstantiate < numbers.Count ? true : false;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            instiantiating = !instiantiating;
        }
    }

    // This function is called when the Bingo button is clicked
    public void BingoButtonClick()
    {
        if(WinChecking(bingoButtons))
        {
            // Show the winner UI and stop the game
            Debug.Log("You Won!");
            Time.timeScale = 0;
            Invoke("RestartGame", 1f);
            // You can add code here to show a UI or perform any other action when a player wins
        }
        else
        {
            Debug.Log("Adhyam Kalikkadaa");
        }
    }

    // This function restarts the game
    public void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1;
        // Restart the game logic here
        // For example, you can reload the scene or reset the game state
    }

    // This function checks if a player has won
    public bool WinChecking(List<Button> buttons)
    {
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
        //new int[] {0, 4, 12, 16, 20}
        };

        foreach (var pattern in winPatterns)
        {
            if (pattern.All(index => buttons[index].GetComponent<Num>().numberState == NumberState.Matched))
            {
                return true;
            }
        }

        return false;
    }

    public void CallAi()
    {
        FindAnyObjectByType<BotManager>().AiMove();
    }
}

// This enum represents the state of a number in the bingo game
public enum NumberState
{
    Unselected,
    Matched
}
