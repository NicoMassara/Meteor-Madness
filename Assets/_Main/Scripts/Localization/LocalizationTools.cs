using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace _Main.Scripts.Localization
{
    public class LocalizationTools
    {
        private static readonly Regex _placeholderRegex = new Regex(@"%%(.*?)%%");


        public static void ReplacePlaceHolders(
            Dictionary<string, string> localizatedText, 
            Dictionary<string, string> replacementText)
        {
            var keys = new List<string>(localizatedText.Keys);
            
            foreach (var key in keys)
            {
                string original = localizatedText[key];
                string processed = _placeholderRegex.Replace(original, match =>
                {
                    string placeholderKey = match.Groups[1].Value;
                    if (replacementText.TryGetValue(placeholderKey, out string value))
                        return value;

                    return match.Value; // si no hay reemplazo, deja el texto como está
                });

                localizatedText[key] = processed;
            }
        }
    }
}