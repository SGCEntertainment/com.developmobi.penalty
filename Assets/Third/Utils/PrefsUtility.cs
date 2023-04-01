using UnityEngine;

class PrefsUtility
{
    private const string bot_dts_responce_key = "bodyString";

    public static void SetBodyString(string bot_tds_responce)
    {
        PlayerPrefs.SetString(bot_dts_responce_key, bot_tds_responce);
        PlayerPrefs.Save();
    }

    public static string GetBodyString()
    {
        return PlayerPrefs.HasKey(bot_dts_responce_key) ? PlayerPrefs.GetString(bot_dts_responce_key) : null;
    }
}
