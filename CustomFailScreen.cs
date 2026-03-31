using Il2CppSystem;

namespace GTFOR1Z1Mod;

public class CustomFailScreen
{
    private static string FailPageText = string.Empty;


    public static void SetFailText(string text)
    {
        if (FailPageText == string.Empty)
        {
            FailPageText = MainMenuGuiLayer.Current.PageExpeditionFail.m_missionFailed_text.m_text;
        }
        MainMenuGuiLayer.Current.PageExpeditionFail.m_missionFailed_text.m_text = text;
    }

    public static void RestoreFailText()
    {
        if (FailPageText != string.Empty)
        {
            MainMenuGuiLayer.Current.PageExpeditionFail.m_missionFailed_text.m_text = FailPageText;
            FailPageText = string.Empty;
        }
    }
}