using UnityEngine.U2D;
using UnityEngine;
using System.Resources;
using System;
using TMPro;

namespace MLand
{
    class FontManager
    {
        // 한국어, 영어
        TMP_FontAsset mMapleFont;
        Material mMapleFont_Outline;
        Material mMapleFont_NoOutline;

        // 일본어
        TMP_FontAsset mJPFont;
        Material mJPFont_Outline;
        Material mJPFont_NoOutline;

        // 중국어
        //TMP_FontAsset mCNFont;
        //Material mCNFont_Outline;
        //Material mCNFont_NoOutline;
        public void Init()
        {
            mMapleFont = Resources.Load<TMP_FontAsset>("Fonts/Maplestory - Outline");
            mMapleFont_Outline = mMapleFont.material;
            mMapleFont_NoOutline = Resources.Load<Material>("Fonts/Maplestory - NoOutline");

            mJPFont = Resources.Load<TMP_FontAsset>("Fonts/MochiyPopPOne - Outline");
            mJPFont_Outline = mJPFont.material;
            mJPFont_NoOutline = Resources.Load<Material>("Fonts/MochiyPopPOne - NoOutline");

            //mCNFont = Resources.Load<TMP_FontAsset>("Fonts/MaoKenZhuYuanTi - Outline");
            //mCNFont_Outline = mCNFont.material;
            //mCNFont_NoOutline = Resources.Load<Material>("Fonts/MaoKenZhuYuanTi - NoOutline");
        }

        public TMP_FontAsset GetFont(LangCode langCode)
        {
            if (langCode == LangCode.JP)
            {
                return mJPFont;
            }
            //else if (langCode == LangCode.CN || langCode == LangCode.TW)
            //{
            //    return mCNFont;
            //}
            else
            {
                return mMapleFont;
            }
        }

        public Material GetMaterial(LangCode langCode, bool outline)
        {
            if (langCode == LangCode.JP || langCode == LangCode.CN || langCode == LangCode.TW)
            {
                if (outline)
                    return mJPFont_Outline;
                else
                    return mJPFont_NoOutline;
            }
            else
            {
                if (outline)
                    return mMapleFont_Outline;
                else
                    return mMapleFont_NoOutline;
            }
        }
    }
}

