using UnityEngine;

public class SwitchingScript : MonoBehaviour
{
    public GameObject player;
    public GameObject bot;

    public void SwitchBotAndPlayer(int a)
    {
        player.transform.SetSiblingIndex(a/10);
        bot.transform.SetSiblingIndex(a%10);
        /*
        bool isPlayerActive = player.activeSelf;
        // Switch the active state of player and bot
        player.SetActive(!isPlayerActive);
        bot.SetActive(isPlayerActive);
        */
    }
}
