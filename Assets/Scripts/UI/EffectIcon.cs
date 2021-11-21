using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EffectIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image frame;
    public Image icon;
    public Text durationText;
    // 0 if buff, 1 if debuff
    public Sprite[] possibleFrames;

    public EffectIconsController iconsController;
    public int effectLevel;
    public string effectName;
    public string effectDescription;

    public void OnPointerEnter(PointerEventData eventData)
    {
        iconsController.transform.position = transform.position + new Vector3(-20, 70);
        iconsController.effectNameText.text = Translate.TranslateText(effectName) + " " + RomanNumerals.GetRomanNumeral(effectLevel);
        iconsController.effectDescriptionText.text = effectDescription;
        iconsController.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, iconsController.effectDescriptionText.preferredHeight + 70);
        iconsController.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        iconsController.gameObject.SetActive(false);
    }
}

public class RomanNumerals
{
    public static string GetRomanNumeral(int number)
    {
        string roman = "";

        while (number > 0)
        {
            if (number >= 1000)
            {
                roman = roman + "M";
                number = number - 1000;
            }
            else if (number >= 900)
            {
                roman = roman + "CM";
                number = number - 900;
            }
            else if (number >= 500)
            {
                roman = roman + "D";
                number = number - 500;
            }
            else if (number >= 400)
            {
                roman = roman + "CD";
                number = number - 400;
            }
            else if (number >= 100)
            {
                roman = roman + "C";
                number = number - 100;
            }
            else if (number >= 90)
            {
                roman = roman + "XC";
                number = number - 90;
            }
            else if (number >= 50)
            {
                roman = roman + "L";
                number = number - 50;
            }
            else if (number >= 40)
            {
                roman = roman + "XL";
                number = number - 40;
            }
            else if (number > 10)
            {
                roman = roman + "X";
                number = number - 10;
            }
            else if (number <= 10)
            {
                roman = roman + Numerals(number);
                number = 0;
            }
        }

        return roman;
    }

    public static string Numerals(int number)
    {
        string roman;

        switch (number)
        {
            case 1:
                roman = "I";
                break;
            case 2:
                roman = "II";
                break;
            case 3:
                roman = "III";
                break;
            case 4:
                roman = "IV";
                break;
            case 5:
                roman = "V";
                break;
            case 6:
                roman = "VI";
                break;
            case 7:
                roman = "VII";
                break;
            case 8:
                roman = "VIII";
                break;
            case 9:
                roman = "IX";
                break;
            case 10:
                roman = "X";
                break;
            default:
                roman = number.ToString();
                break;
        }

        return roman;
    }
}