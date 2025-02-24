using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MLand
{
    //class CharacterGauge
    //{
    //    const float OffsetY = 2.5f;

    //    Character mCharacter;
    //    GameObject mGaugeUIObj;

    //    GameObject mHpGauge;
    //    Image mHpGaugeImg;
    //    TextMeshProUGUI mHpGaugeText;

    //    GameObject mMarsoCoolGauge;
    //    Image mMarsoCoolGaugeImg;
    //    TextMeshProUGUI mMarsoCoolGaugeText;

    //    GameObject mStunGauge;
    //    Image mStunGaugeImg;
    //    TextMeshProUGUI mStunGaugeText;
    //    public static CharacterGauge Create(Character character)
    //    {
    //        GameObject gaugeUIObj = Util.InstantiateUI("GaugeBar");

    //        CharacterGauge characterGauge = new CharacterGauge();
    //        characterGauge.Init(character, gaugeUIObj);

    //        return characterGauge;
    //    }

    //    #region Initialize Gauge 
    //    void Init(Character character, GameObject gaugeUIObj)
    //    {
    //        mCharacter = character;
    //        mGaugeUIObj = gaugeUIObj;

    //        InitHpGauge();
    //        InitMarsoCoolGauge();
    //        InitStunGauge();

    //        SetActiveStun(false);

    //        UpdateHp(true);
    //        UpdateMarsoCoolTime(true);
    //    }

    //    (Image gaugeImg, TextMeshProUGUI gaugeText) FindGaugeImgAndText(GameObject gaugeParent)
    //    {
    //        Image gaugeImg = gaugeParent.Find("Gauge", true)?.GetComponent<Image>();
    //        TextMeshProUGUI gaugeText = gaugeParent.Find("GaugeText", true)?.GetComponent<TextMeshProUGUI>();

    //        Debug.Assert(gaugeImg != null, $"{gaugeParent.name}에 Gauge Image가 없다.");
    //        Debug.Assert(gaugeText != null, $"{gaugeParent.name}에 Gauge TextMeshPro가 없다.");

    //        return (gaugeImg, gaugeText);
    //    }

    //    void InitHpGauge()
    //    {
    //        mHpGauge = mGaugeUIObj.Find("HpGauge", true);

    //        Debug.Assert(mHpGauge != null, $"{mCharacter.Id}의 HpGague가 없다");

    //        var result = FindGaugeImgAndText(mHpGauge);

    //        mHpGaugeImg = result.gaugeImg;
    //        mHpGaugeText = result.gaugeText;
    //    }

    //    void InitMarsoCoolGauge()
    //    {
    //        mMarsoCoolGauge = mGaugeUIObj.Find("MarsoCoolGauge", true);

    //        Debug.Assert(mMarsoCoolGauge != null, $"{mCharacter.Id}의 MarsoCoolGauge가 없다");

    //        var result = FindGaugeImgAndText(mMarsoCoolGauge);

    //        mMarsoCoolGaugeImg = result.gaugeImg;
    //        mMarsoCoolGaugeText = result.gaugeText;
    //    }

    //    void InitStunGauge()
    //    {
    //        mStunGauge = mGaugeUIObj.Find("StunGauge", true);

    //        Debug.Assert(mStunGauge != null, $"{mCharacter.Id}의 StunGauge가 없다");

    //        var result = FindGaugeImgAndText(mStunGauge);

    //        mStunGaugeImg = result.gaugeImg;
    //        mStunGaugeText = result.gaugeText;
    //    }
    //    #endregion

    //    #region Update Gauge
    //    public void OnUpdate(float dt)
    //    {
    //        UpdateHp();
    //        UpdateMarsoCoolTime();
    //        UpdatePosition();
    //    }

    //    public void UpdateStunTime(float curStunTime, float maxStunTime, bool forced = false)
    //    {
    //        float ratio = (float)(curStunTime / maxStunTime);

    //        if (ratio != mMarsoCoolGaugeImg.fillAmount || forced)
    //        {
    //            mStunGaugeImg.fillAmount = ratio;
    //            mStunGaugeText.text = $"{ratio:F1}초";
    //        }
    //    }

    //    void UpdateHp(bool forced = false)
    //    {
    //        double maxHp = mCharacter.Stat.MaxHp;
    //        double curHp = mCharacter.Stat.CurHp;

    //        float ratio = (float)(curHp / maxHp);

    //        if (ratio != mHpGaugeImg.fillAmount || forced)
    //        {
    //            mHpGaugeImg.fillAmount = ratio;
    //            mHpGaugeText.text = $"{curHp.ToString()}/{maxHp.ToString()}";
    //        }
    //    }

    //    void UpdateMarsoCoolTime(bool forced = false)
    //    {
    //        if (mCharacter.Type != FollowType.Slime ||
    //            mCharacter.Stat.MarsoDropCoolTime <= 0)
    //            return;

    //        float maxMarsoCoolTime = mCharacter.Stat.MarsoDropCoolTime;
    //        float curMarsoCoolTime = mCharacter.MarsoCoolTime;

    //        float ratio = (float)(curMarsoCoolTime / maxMarsoCoolTime);

    //        if (ratio != mMarsoCoolGaugeImg.fillAmount || forced)
    //        {
    //            mMarsoCoolGaugeImg.fillAmount = ratio;
    //            mMarsoCoolGaugeText.text = $"{curMarsoCoolTime:F1}/{maxMarsoCoolTime}";
    //        }
    //    }

    //    void UpdatePosition()
    //    {
    //        Vector3 worldPos = mCharacter.transform.position;

    //        Vector3 offsetPos = new Vector3(worldPos.x, worldPos.y + OffsetY);

    //        mGaugeUIObj.transform.position = Util.WorldToScreenPoint(offsetPos);
    //    }
    //    #endregion

    //    public void SetActiveStun(bool active)
    //    {
    //        if ( active )
    //        {
    //            mStunGauge.SetActive(true);

    //            mHpGauge.SetActive(false);

    //            if ( mCharacter.Stat.MarsoDropAmount > 0 && 
    //                mCharacter.Stat.MarsoDropCoolTime > 0 )
    //            {
    //                mMarsoCoolGauge.SetActive(false);
    //            }
    //        }
    //        else
    //        {
    //            mStunGauge.SetActive(false);

    //            mHpGauge.SetActive(true);

    //            if (mCharacter.Stat.MarsoDropAmount > 0 &&
    //                mCharacter.Stat.MarsoDropCoolTime > 0)
    //            {
    //                mMarsoCoolGauge.SetActive(true);
    //            }
    //        }
    //    }

    //    public void Release()
    //    {
    //        mGaugeUIObj.Destroy();
    //    }
    //}
}


