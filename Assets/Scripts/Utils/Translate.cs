using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Translate : MonoBehaviour
{
    public static string _language;
    public bool translateSelf;

    string startTextKey;
    Text textComponent;

    public void Awake()
    {
        textComponent = GetComponent<Text>();
        if (textComponent != null)
            startTextKey = textComponent.text;
        if (translateSelf == false)
        {
            PlayerPrefs.SetString("lang", "ru");
            PlayerPrefs.Save();

            _language = PlayerPrefs.GetString("lang");
        }
    }

    public void OnEnable()
    {
        if (translateSelf == true && textComponent != null)
            textComponent.text = TranslateText(startTextKey);
    }

    public static string TranslateText (string text)
    {
        _language = PlayerPrefs.GetString("lang");
        string result;

        Translations.Load("/Translates/" + _language + ".json");
        result = Translations.Get(text);

        return result;
    }
}