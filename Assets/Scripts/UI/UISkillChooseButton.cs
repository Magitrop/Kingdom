using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UISkillChooseButton : UIInventoryActionButton, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject skillsPanel;
    public UIInventoryCraftButton craftButton;
    public Image skillIcon;

    public SkillIconsController iconsController;
    public int heroLevel;
    public int skillID;
    public string skillName;
    public string skillDescription;

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        if (isUnavailable == true && player.levelupPanelController.gameObject.activeSelf == false)
            return;
        /*
        if (player.isAlive == false || player.inSpellMode == true || player.inExploreMode == true)
            return;
            */
        player.levelupPanelController.unusedSkillPoints--;
        player.GainSkill(skillID, heroLevel);
        player.map.SavePreparedSkills();
        player.levelupPanelController.PrepareSkills();
        iconsController.gameObject.SetActive(false);
        if (player.levelupPanelController.unusedSkillPoints <= 0)
        {
            player.levelupPanelController.gameObject.SetActive(false);
            skillsPanel.SetActive(false);
            tooltip.gameObject.SetActive(false);
            player.DeselectVisualSlots();
        }
        else
        {
            player.levelupPanelController.ShowSkills();
            player.levelupPanelController.gameObject.SetActive(true);
        }

        player.RefreshStatsVisual();
    }

    new public void OnPointerEnter(PointerEventData eventData)
    {
        iconsController.transform.position = transform.position + new Vector3(0, 30);
        iconsController.skillNameText.text = Translate.TranslateText(skillName);
        iconsController.skillDescriptionText.text = skillDescription;
        iconsController.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, iconsController.skillDescriptionText.preferredHeight + 70);
        iconsController.gameObject.SetActive(true);
    }

    new public void OnPointerExit(PointerEventData eventData)
    {
        iconsController.gameObject.SetActive(false);
    }
}