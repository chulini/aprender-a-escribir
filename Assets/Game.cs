using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour {
    static Game instance;
    [SerializeField] InputField input;
    [SerializeField] Text wordIndicator;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip correctClip;
    [SerializeField] AudioClip incorrectClip;
    [SerializeField] AudioClip winClip;
    [SerializeField] Image wordImage;
    [SerializeField] Image semitranspImage;
    Sprite[] wordImages;
    static List<string> words;
    static int m_wordIndex = 0;
    static string charToGuess;
    static int wordIndex
    {
        get
        {
            return m_wordIndex;
        }
        set
        {
            m_wordIndex = value;
            if(m_wordIndex < currentWord.Length)
            {
                charToGuess = currentWord.Substring(m_wordIndex, 1);
            } else
            {
                instance.gameStatus = GameStatus.Winning;
                instance.semitranspImage.enabled = false;
                instance.audioSource.Stop();
                instance.audioSource.clip = instance.winClip;
                instance.audioSource.Play();
                instance.Invoke("NewWord", 4);
            }
        }
    }
    static string m_currentWord;
    public static string currentWord
    {
        get
        {
            return m_currentWord;
        }
        set
        {
            m_currentWord = value.ToUpper();
            instance.wordIndicator.text = currentWord;
            wordIndex = 0;
        }
    }

    
    enum GameStatus { Guessing, Winning };
    GameStatus gameStatus = GameStatus.Guessing;

    private void Awake()
    {
        instance = this;
        wordImages = Resources.LoadAll<Sprite>("images");
        JSONObject w = new JSONObject((Resources.Load("words") as TextAsset).text);
        words = new List<string>();
        for(int i = 0; i < w.list.Count; i++)
            words.Add(w[i].str);
       
        input.onValueChanged.AddListener(delegate { NewInput(); });
    }
    int wordsIndex = 0;
    void NewWord()
    {
        gameStatus = GameStatus.Guessing;
        instance.semitranspImage.enabled = true;
        currentWord = words[wordsIndex].ToUpper();

        wordImage.sprite = wordImages[wordsIndex];
        

        wordsIndex = (wordsIndex + 1) % words.Count;
    }
    void NewInput()
    {
        if(input.text.Length > 0 && gameStatus == GameStatus.Guessing)
        {
            instance.audioSource.Stop();

            string keyPressed = input.text.ToUpper();
            if (keyPressed == charToGuess)
            {
                instance.audioSource.clip = correctClip;
                wordIndex++;
            } else
            {
                instance.audioSource.clip = incorrectClip;

            }
            instance.audioSource.Play();

            input.text = "";
            input.ActivateInputField();

        }
    }
    void Start () {
        input.text = "";
        input.ActivateInputField();
        NewWord();

    }
    bool tilt = false;
    void FixedUpdate () {
        if(gameStatus == GameStatus.Guessing)
        {
            string indicatorText = "";
            for(int i = 0; i < currentWord.Length; i++)
            {
                if(i < wordIndex){
                    indicatorText += "<color=white>" + currentWord.Substring(i,1) + "</color>";
                }
                else if(i == wordIndex)
                {

                    indicatorText += tilt ? "<color=yellow>" +charToGuess+"</color>" : "<color=red>" + charToGuess + "</color>";
                    tilt = !tilt;
                } else
                {
                    indicatorText += "<color=black>" + currentWord.Substring(i, 1) + "</color>";
                }
            }
            wordIndicator.text = indicatorText;
        }

    }
}
