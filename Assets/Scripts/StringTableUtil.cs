using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MLand
{
    static class StringTableUtil
    {
        public static string Get(string key, StringParam param = null)
        {
            StringTable stringTable = MLand.GameData.StringTable.TryGet(key);

            int langCodeIdx = (int)MLand.SavePoint.LangCode;

            string text = stringTable?.text[langCodeIdx];

            if (param != null && param.IsValid && text.IsValid())
            {
                text = text.ApplyParam(param);
            }

            return text;
        }

        public static string GetDesc(string index, StringParam param = null)
        {
            return Get($"Desc_{index}", param);
        }

        public static string GetName(string index, StringParam param = null)
        {
            return Get($"Name_{index}", param);
        }

        public static string GetBuildingName(string buildingId, int buildingLevel)
        {
            StringParam param = new StringParam("level", $"{buildingLevel}"); 

            return Get($"Name_{buildingId}", param);
        }

        public static string GetGrade(ItemGrade grade, StringParam param = null)
        {
            return GetName($"{grade}", param);
        }

        public static string GetSystemMessage(string index, StringParam param = null)
        {
            return Get($"SystemMessage_{index}", param);
        }

        public static string GetTutorialMessage(string index, StringParam param = null)
        {
            return Get($"TutorialMessage_{index}", param);
        }

        public static string ApplyParam(this string s, StringParam param)
        {
            if (s.IsValid())
            {
                foreach(KeyValuePair<string, string> p in param.Params)
                {
                    s = s.Replace($"{{{p.Key}}}", p.Value);
                }
            }

            return s;
        }
    }

    class StringParam
    {
        public Dictionary<string, string> Params;
        public bool IsValid => Params.Count > 0;
        public StringParam(string target, string param)
        {
            Params = new Dictionary<string, string>();

            AddParam(target, param);
        }

        public void AddParam(string target, string param)
        {
            if (Params.ContainsKey(target) == false)
            {
                Params.Add(target, param);
            }
        }
    }
}