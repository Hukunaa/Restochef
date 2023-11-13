using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using Voodoo.Sauce.Internal.Editor;

namespace Voodoo.Sauce.Internal.Analytics.Editor
{
    public class PrivacyPrebuild : IPreprocessBuildWithReport
    {
        public int callbackOrder => 1;

        public void OnPreprocessBuild(BuildReport report)
        {
            CheckAndUpdatePrivacySettingsOnBuild(TinySauceSettings.Load());
        }

        public static void CheckAndUpdatePrivacySettingsOnBuild(TinySauceSettings sauceSettings)
        {
            if (sauceSettings == null || string.IsNullOrEmpty(sauceSettings.companyName.Trim()))
            {
                throw new BuildFailedException("Company Name is empty");
            }

            if (sauceSettings == null ||
                string.IsNullOrEmpty(sauceSettings.privacyPolicyURL.Trim()))
            {
                throw new BuildFailedException("Privacy Policy is empty");
            }

            if (sauceSettings == null ||
                string.IsNullOrEmpty(sauceSettings.developerContactEmail.Trim()))
            {
                throw new BuildFailedException("Developer Contact Email is empty");
            }

        }
        public static bool CheckAndUpdatePrivacySettings(TinySauceSettings sauceSettings)
        {
            if (sauceSettings == null || string.IsNullOrEmpty(sauceSettings.companyName.Trim()))
            {
                return false;
            }

            if (sauceSettings == null ||
                string.IsNullOrEmpty(sauceSettings.privacyPolicyURL.Trim()))
            {
                return false;
            }

            if (sauceSettings == null ||
                string.IsNullOrEmpty(sauceSettings.developerContactEmail.Trim()))
            {
                return false;
            }

            return true;
        }
    }
}
