using UnityEngine;
using UnityEngine.UI;

public class Num : MonoBehaviour
{
     public int number;
     public NumberState numberState = NumberState.Unselected;
    public ButtonState buttonState;
     public Sprite selectedImage;
     Button button;
     GameManger gameManger;
    BotManager botManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManger = FindAnyObjectByType<GameManger>();
        botManager = FindAnyObjectByType<BotManager>();
        button = GetComponent<Button>();
        if(buttonState == ButtonState.Player)
        {
            // Add a listener to the button to call CheckNum when clicked
            button.onClick.AddListener(CheckNum);
        }
    }

    // This function check the num and GameManger currentNum is same
    public void CheckNum()
    {
        if(numberState == NumberState.Matched) { return; }
        if (gameManger.currentNum == number)
        {
            gameManger.CallIncreasePowerUpImage();
            // Change the state to selected
            numberState = NumberState.Matched;
            // Change the button Image to selectedImage
            button.GetComponent<Image>().sprite = selectedImage;
        }
        else if(gameManger.isMagicToolActive)
        {
            gameManger.isMagicToolActive = false;
            // Change the state to selected
            numberState = NumberState.Matched;
            // Change the button Image to selectedImage
            button.GetComponent<Image>().sprite = selectedImage;
            gameManger.currentTool.sprite = gameManger.powerToolsDefualtSPrite;
        }
        else if(gameManger.currentNumbers.Contains(number))
        {
            // Change the state to selected
            numberState = NumberState.Matched;
            // Change the button Image to selectedImage
            button.GetComponent<Image>().sprite = selectedImage;
        }
    }

    // This function check the num and GameManger currentNum is same
    public void CheckNumBot()
    {
        if (numberState == NumberState.Matched) { return; }
        if (gameManger.currentNum == number)
        {
            botManager.CallIncreasePowerUpImage();
            // Change the state to selected
            numberState = NumberState.Matched;
            // Change the button Image to selectedImage
            button.GetComponent<Image>().sprite = selectedImage;
        }
        else if (botManager.isMagicToolActive)
        {
            botManager.isMagicToolActive = false;
            // Change the state to selected
            numberState = NumberState.Matched;
            // Change the button Image to selectedImage
            button.GetComponent<Image>().sprite = selectedImage;
            gameManger.currentTool.sprite = gameManger.powerToolsDefualtSPrite;
        }
        else if (gameManger.currentNumbers.Contains(number))
        {
            // Change the state to selected
            numberState = NumberState.Matched;
            // Change the button Image to selectedImage
            button.GetComponent<Image>().sprite = selectedImage;
        }
    }
}

public enum ButtonState
{
    Player,
    Bot
}
