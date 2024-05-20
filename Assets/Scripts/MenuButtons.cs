using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
    // Play button
    public void PlayButton() => SceneManager.LoadScene("Game");

    // Menu button
    public void MenuButton() => SceneManager.LoadScene("MainMenu");

    // Settings button
    public void SettingsButton() => SceneManager.LoadScene("Settings");

    // Restart button
    public void RestartButton()
    {
        this.gameObject.SetActive(false);
        SceneManager.LoadScene("Game");
    }
}
