using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjInfoPopUp : MonoBehaviour {
    protected string id;
    protected int type;
    protected int level;

    SlotManager slotManager;

    public Image ObjImage;

    public GameObject LockImage;
    public Button PriceButton;

    public Text Price;

    public GameObject LackOfCoin_Activation;
    public GameObject LackOfCoin_Upgrade;
    

    void InitPriceInfo(int level)
    {
        int price = GetPrice(level);
        if (price != -1)
        {
            Price.gameObject.SetActive(true);
            Price.text = price.ToString();

            LockImage.SetActive(false);
            PriceButton.enabled = false;
        }
        else
        {
            Price.gameObject.SetActive(false);
        }
    }

    void InitActiveCostInfo(int level)
    {
        LockImage.SetActive(true);
        PriceButton.enabled = true;

        int price = GetActiveCost();
        if (price != -1)
        {
            Price.gameObject.SetActive(true);
            Price.text = price.ToString();
        }
        else
            Price.gameObject.SetActive(false);
    }

    public virtual void InitObjInfoPopUp(string id, int type, SlotManager slotManager)
    {
        this.id = id;
        this.type = type;
        this.slotManager = slotManager;

        ObjImage.sprite = JsonDataManager.slotImage[id];
        level = JsonDataManager.slotInfoList[id].level;

        if (level != 0)
        {
            InitPriceInfo(level);
        }
        else
        {
            InitActiveCostInfo(level);
        }
    }

    int GetPrice(int level)
    {
        switch (type)
        {
            case 0: return JsonDataManager.GetBuildingInfo(id, level).Price;
            case 2: return JsonDataManager.GetOurForcesInfo(id, level).Price;
            case 3: return JsonDataManager.GetTrapInfo(id, level).Price;
            case 4:
                try
                {
                    return JsonDataManager.GetSecretInfo(id, Data_Player.Fame).Price;
                }
                catch (System.NullReferenceException) { return -1; }
            default: return -1;
        }
    }

    int GetActiveCost()
    {
        switch (type)
        {
            case 0: return JsonDataManager.GetBuildingInfo(id, 1).ActiveCost;
            case 2: return JsonDataManager.GetOurForcesInfo(id, 1).ActiveCost;
            case 3: return JsonDataManager.GetTrapInfo(id, 1).ActiveCost;
            default: return -1;
        }
    }

    protected int GetUpgradeCost()
    {
        SlotInfo slotInfo = JsonDataManager.slotInfoList[id];
        int type = slotInfo.type;
        try
        {
            switch (type)
            {
                case 0: return JsonDataManager.GetBuildingInfo(id, slotInfo.level).UpgradeCost;
                case 2: return JsonDataManager.GetOurForcesInfo(id, slotInfo.level).UpgradeCost;
                case 3: return JsonDataManager.GetTrapInfo(id, slotInfo.level).UpgradeCost;
                default: return -1;
            }
        }
        catch(System.NullReferenceException) { return -1; }
    }

	public void LevelUp()
    {
        int upgradeCost = GetUpgradeCost();
        if (upgradeCost == -1)
            return;

        if (Data_Player.isEnough_G(upgradeCost))
        {
            Data_Player.subGold(upgradeCost);
            JsonDataManager.SetSlotInfo(JsonDataManager.slotInfoList[id].type, id, JsonDataManager.slotInfoList[id].level + 1);

            slotManager.RefreshInfo();
            InitObjInfoPopUp(id, type, slotManager);
        }
        else
            LackOfCoin_Upgrade.SetActive(true);
    }

    public void OnActivation()
    {
        int activeCost = GetActiveCost();

        if (Data_Player.isEnough_G(activeCost))
        {
            Data_Player.subGold(activeCost);

            slotManager.OnActivationSlot();
            InitObjInfoPopUp(id, type, slotManager);
        }
        else
        {
            LackOfCoin_Activation.SetActive(true);
        }
    }
}
