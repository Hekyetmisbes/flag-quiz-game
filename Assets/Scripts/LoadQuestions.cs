using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine.Networking;

public class LoadQuestions : MonoBehaviour
{
    [SerializeField]
    Image questionImage;
    [SerializeField]
    Image gameOverImage;

    [SerializeField]
    TextMeshProUGUI chooseAText;
    [SerializeField]
    TextMeshProUGUI chooseBText;
    [SerializeField]
    TextMeshProUGUI chooseCText;
    [SerializeField]
    TextMeshProUGUI chooseDText;
    [SerializeField]
    TextMeshProUGUI gameOverText;
    [SerializeField]
    TextMeshProUGUI scoreText;

    [SerializeField]
    Button chooseAButton;
    [SerializeField]
    Button chooseBButton;
    [SerializeField]
    Button chooseCButton;
    [SerializeField]
    Button chooseDButton;

    [SerializeField]
    AudioSource CorrectSound;
    [SerializeField]
    AudioSource WrongSound;

    int score = 0;

    private string correctAnswer;

    private DatabaseReference dbReference;

    void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            FirebaseApp app = FirebaseApp.DefaultInstance;
            dbReference = FirebaseDatabase.DefaultInstance.RootReference;
            LoadNextQuestion();
        });
    }

    void LoadNextQuestion()
    {
        dbReference.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Error getting questions from Firebase.");
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                List<DataSnapshot> questions = new List<DataSnapshot>(snapshot.Children);

                int randomIndex = Random.Range(0, questions.Count);
                DataSnapshot question = questions[randomIndex];
                string countryCode = question.Child("countrycode").Value.ToString();
                string imageUrl = "https://flagcdn.com/h240/" + countryCode + ".png";
                string optionA = question.Child("countryname").Value.ToString();
                string optionB = GetRandomCountryName(questions, optionA);
                string optionC = GetRandomCountryName(questions, optionA);
                string optionD = GetRandomCountryName(questions, optionA);

                List<string> options = new List<string> { optionA, optionB, optionC, optionD };
                options = ShuffleList(options);

                chooseAText.text = options[0];
                chooseBText.text = options[1];
                chooseCText.text = options[2];
                chooseDText.text = options[3];

                correctAnswer = optionA;

                StartCoroutine(LoadImage(imageUrl));
            }
        });
    }

    IEnumerator LoadImage(string imageUrl)
    {
        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(imageUrl))
        {
            yield return www.SendWebRequest();
            if (www.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(www);
                questionImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            }
            else
            {
                Debug.LogError("Failed to load image: " + www.error);
            }
        }
    }


    public void OnChooseA()
    {
        CheckAnswer(chooseAText.text);
    }

    public void OnChooseB()
    {
        CheckAnswer(chooseBText.text);
    }

    public void OnChooseC()
    {
        CheckAnswer(chooseCText.text);
    }

    public void OnChooseD()
    {
        CheckAnswer(chooseDText.text);
    }

    void CheckAnswer(string chosenAnswer)
    {
        if (chosenAnswer == correctAnswer)
        {
            score++;
            scoreText.text = "Score: " + score;
            CorrectSound.Play();
            LoadNextQuestion();
        }
        else
        {
            chooseAButton.interactable = false;
            chooseBButton.interactable = false;
            chooseCButton.interactable = false;
            chooseDButton.interactable = false;
            WrongSound.Play();
            if (score > PlayerPrefs.GetInt("HighScore"))
            {
                PlayerPrefs.SetInt("HighScore", score);
            }
            gameOverText.text = "Game Over\r\n Answer: " + correctAnswer + "\r\nScore : " + score + "\r\nHigh Score: " + PlayerPrefs.GetInt("HighScore");
            gameOverImage.gameObject.SetActive(true);
        }
    }

    string GetRandomCountryName(List<DataSnapshot> questions, string exclude)
    {
        while (true)
        {
            int randomIndex = Random.Range(0, questions.Count);
            string countryName = questions[randomIndex].Child("countryname").Value.ToString();
            if (countryName != exclude)
            {
                return countryName;
            }
        }
    }

    List<string> ShuffleList(List<string> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            string temp = list[i];
            int randomIndex = Random.Range(0, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
        return list;
    }
}
