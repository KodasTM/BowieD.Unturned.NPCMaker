﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;

namespace BowieD.Unturned.NPCMaker.Localization
{
    public static class LocUtil
    {
        public static IEnumerable<string> SupportedLanguages()
        {
            yield return "en-US";
            yield return "ru-RU";
        }
        public static IEnumerable<CultureInfo> SupportedCultures()
        {
            foreach (var k in SupportedLanguages())
            {
                yield return new CultureInfo(k);
            }
        }
        public static string LocalizeInterface(string key)
        {
            return MainWindow.Instance.TryFindResource(key) as string ?? key;
        }
        public static string LocalizeInterface(string key, params object[] args)
        {
            if (LocalizeInterface(key) == key)
                return key;
            return string.Format(LocalizeInterface(key), args);
        }
        public static string LocalizeCondition(string key)
        {
            if (_conditionsLang != null && _conditionsLang.ContainsKey(key))
                return _conditionsLang[key];
            return key;
        }
        public static string LocalizeCondition(string key, params object[] args)
        {
            if (LocalizeCondition(key) == key)
                return key;
            return string.Format(LocalizeCondition(key), args);
        }
        public static string LocalizeReward(string key)
        {
            if (_rewardsLang != null && _rewardsLang.ContainsKey(key))
                return _rewardsLang[key];
            return key;
        }
        public static string LocalizeReward(string key, params object[] args)
        {
            if (LocalizeReward(key) == key)
                return key;
            return string.Format(LocalizeReward(key), args);
        }
        public static string LocalizeMistake(string key)
        {
            if (mistakesLang != null && mistakesLang.ContainsKey(key))
                return mistakesLang[key];
            return key;
        }
        public static string LocalizeMistake(string key, params object[] args)
        {
            if (LocalizeMistake(key) == key)
                return key;
            return string.Format(LocalizeMistake(key), args);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="langCode">ex: en-US</param>
        public static void LoadLanguage(string langCode)
        {
            if (_isInit)
                throw new Exception("Language already loaded!");
            // lang.interface.LANGCODE.xaml
            // lang.conditions.LANGCODE.json
            // lang.rewards.LANGCODE.json
            // lang.mistakes.LANGCODE.json
            try
            {
                ResourceDictionary oldDict = (from d in Application.Current.Resources.MergedDictionaries
                                              where d.Source != null && d.Source.OriginalString.StartsWith("Localization/lang.interface.")
                                              select d).First();
                ResourceDictionary dict = new ResourceDictionary()
                {
                    Source = new Uri($"Localization/lang.interface.{langCode}.xaml", UriKind.Relative)
                };
                if (oldDict != null)
                {
                    int ind = Application.Current.Resources.MergedDictionaries.IndexOf(oldDict);
                    Application.Current.Resources.MergedDictionaries.Remove(oldDict);
                    Application.Current.Resources.MergedDictionaries.Insert(ind, dict);
                }
                else
                {
                    Application.Current.Resources.MergedDictionaries.Add(dict);
                }
            }
            catch (Exception) { Debug.WriteLine("Could not load interface lang."); }
            try
            {
                using (StreamReader sr = new StreamReader(Application.GetResourceStream(new Uri($"Localization/lang.conditions.{langCode}.json")).Stream))
                {
                    string text = sr.ReadToEnd();
                    var des = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(text);
                }
            }
            catch (Exception) { Debug.WriteLine("Could not load conditions lang."); }
            try
            {
                using (StreamReader sr = new StreamReader(Application.GetResourceStream(new Uri($"Localization/lang.rewards.{langCode}.json")).Stream))
                {
                    string text = sr.ReadToEnd();
                    var des = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(text);
                }
            }
            catch (Exception) { Debug.WriteLine("Could not load rewards lang."); }
            try
            {
                using (StreamReader sr = new StreamReader(Application.GetResourceStream(new Uri($"Localization/lang.mistakes.{langCode}.json")).Stream))
                {
                    string text = sr.ReadToEnd();
                    var des = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(text);
                }
            }
            catch (Exception) { Debug.WriteLine("Could not load mistakes lang."); }
            _isInit = true;
        }
        private static bool _isInit = false;
        private static Dictionary<string, string> 
            _conditionsLang = null,
            _rewardsLang = null,
            mistakesLang = null;
    }
}
