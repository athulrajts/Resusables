
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace Localizer.Core
{
    /// <summary>
    /// Translates text using Google's online language tools.
    /// </summary>
    public class Translator
    {
        #region Properties

        /// <summary>
        /// Gets the supported languages.
        /// </summary>
        public static IEnumerable<string> Languages
        {
            get
            {
                EnsureInitialized();
                return Language.Keys.OrderBy(p => p);
            }
        }

        /// <summary>
        /// Gets the time taken to perform the translation.
        /// </summary>
        public TimeSpan TranslationTime
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the url used to speak the translation.
        /// </summary>
        /// <value>The url used to speak the translation.</value>
        public string TranslationSpeechUrl
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the error.
        /// </summary>
        public Exception Error
        {
            get;
            private set;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Translates the specified source text.
        /// </summary>
        /// <param name="sourceText">The source text.</param>
        /// <param name="sourceLanguage">The source language.</param>
        /// <param name="targetLanguage">The target language.</param>
        /// <returns>The translation.</returns>
        public string Translate
            (string sourceText,
             string sourceLanguage,
             string targetLanguage)
        {
            // Initialize
            this.Error = null;
            this.TranslationSpeechUrl = null;
            this.TranslationTime = TimeSpan.Zero;
            DateTime tmStart = DateTime.Now;
            string translation = string.Empty;

            try
            {
                // Download translation
                string url = string.Format("https://translate.googleapis.com/translate_a/single?client=gtx&sl={0}&tl={1}&dt=t&q={2}",
                                            LanguageEnumToIdentifier(sourceLanguage),
                                            LanguageEnumToIdentifier(targetLanguage),
                                            HttpUtility.UrlEncode(sourceText));
                string outputFile = Path.GetTempFileName();
                using (WebClient wc = new WebClient())
                {
                    wc.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2228.0 Safari/537.36");
                    wc.DownloadFile(url, outputFile);
                }

                // Get translated text
                if (File.Exists(outputFile))
                {

                    // Get phrase collection
                    string text = File.ReadAllText(outputFile);
                    File.Delete(outputFile);
                    int index = text.IndexOf(string.Format(",,\"{0}\"", Translator.LanguageEnumToIdentifier(sourceLanguage)));
                    if (index == -1)
                    {
                        // Translation of single word
                        int startQuote = text.IndexOf('\"');
                        if (startQuote != -1)
                        {
                            int endQuote = text.IndexOf('\"', startQuote + 1);
                            if (endQuote != -1)
                            {
                                translation = text.Substring(startQuote + 1, endQuote - startQuote - 1);
                            }
                        }
                    }
                    else
                    {
                        // Translation of phrase
                        text = text.Substring(0, index);
                        text = text.Replace("],[", ",");
                        text = text.Replace("]", string.Empty);
                        text = text.Replace("[", string.Empty);
                        text = text.Replace("\",\"", "\"");

                        // Get translated phrases
                        string[] phrases = text.Split(new[] { '\"' }, StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 0; (i < phrases.Count()); i += 2)
                        {
                            string translatedPhrase = phrases[i];
                            if (translatedPhrase.StartsWith(",,"))
                            {
                                i--;
                                continue;
                            }
                            translation += translatedPhrase + "  ";
                        }
                    }

                    // Fix up translation
                    translation = translation.Trim();
                    translation = translation.Replace(" ?", "?");
                    translation = translation.Replace(" !", "!");
                    translation = translation.Replace(" ,", ",");
                    translation = translation.Replace(" .", ".");
                    translation = translation.Replace(" ;", ";");

                    // And translation speech URL
                    this.TranslationSpeechUrl = string.Format("https://translate.googleapis.com/translate_tts?ie=UTF-8&q={0}&tl={1}&total=1&idx=0&textlen={2}&client=gtx",
                                                               HttpUtility.UrlEncode(translation), Translator.LanguageEnumToIdentifier(targetLanguage), translation.Length);
                }
            }
            catch (Exception ex)
            {
                this.Error = ex;
            }

            // Return result
            this.TranslationTime = DateTime.Now - tmStart;
            return translation;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Converts a language to its identifier.
        /// </summary>
        /// <param name="language">The language."</param>
        /// <returns>The identifier or <see cref="string.Empty"/> if none.</returns>
        private static string LanguageEnumToIdentifier
            (string language)
        {
            string mode = string.Empty;
            Translator.EnsureInitialized();
            Translator.Language.TryGetValue(language, out mode);
            return mode;
        }

        /// <summary>
        /// Ensures the translator has been initialized.
        /// </summary>
        private static void EnsureInitialized()
        {
            if (Translator.Language == null)
            {
                Translator.Language = new Dictionary<string, string>();
                Translator.Language.Add("Afrikaans", "af");
                Translator.Language.Add("Albanian", "sq");
                Translator.Language.Add("Arabic", "ar");
                Translator.Language.Add("Armenian", "hy");
                Translator.Language.Add("Azerbaijani", "az");
                Translator.Language.Add("Basque", "eu");
                Translator.Language.Add("Belarusian", "be");
                Translator.Language.Add("Bengali", "bn");
                Translator.Language.Add("Bulgarian", "bg");
                Translator.Language.Add("Catalan", "ca");
                Translator.Language.Add("Chinese", "zh-CN");
                Translator.Language.Add("Croatian", "hr");
                Translator.Language.Add("Czech", "cs");
                Translator.Language.Add("Danish", "da");
                Translator.Language.Add("Dutch", "nl");
                Translator.Language.Add("English", "en");
                Translator.Language.Add("Esperanto", "eo");
                Translator.Language.Add("Estonian", "et");
                Translator.Language.Add("Filipino", "tl");
                Translator.Language.Add("Finnish", "fi");
                Translator.Language.Add("French", "fr");
                Translator.Language.Add("Galician", "gl");
                Translator.Language.Add("German", "de");
                Translator.Language.Add("Georgian", "ka");
                Translator.Language.Add("Greek", "el");
                Translator.Language.Add("Haitian Creole", "ht");
                Translator.Language.Add("Hebrew", "iw");
                Translator.Language.Add("Hindi", "hi");
                Translator.Language.Add("Hungarian", "hu");
                Translator.Language.Add("Icelandic", "is");
                Translator.Language.Add("Indonesian", "id");
                Translator.Language.Add("Irish", "ga");
                Translator.Language.Add("Italian", "it");
                Translator.Language.Add("Japanese", "ja");
                Translator.Language.Add("Korean", "ko");
                Translator.Language.Add("Lao", "lo");
                Translator.Language.Add("Latin", "la");
                Translator.Language.Add("Latvian", "lv");
                Translator.Language.Add("Lithuanian", "lt");
                Translator.Language.Add("Macedonian", "mk");
                Translator.Language.Add("Malay", "ms");
                Translator.Language.Add("Maltese", "mt");
                Translator.Language.Add("Norwegian", "no");
                Translator.Language.Add("Persian", "fa");
                Translator.Language.Add("Polish", "pl");
                Translator.Language.Add("Portuguese", "pt");
                Translator.Language.Add("Romanian", "ro");
                Translator.Language.Add("Russian", "ru");
                Translator.Language.Add("Serbian", "sr");
                Translator.Language.Add("Slovak", "sk");
                Translator.Language.Add("Slovenian", "sl");
                Translator.Language.Add("Spanish", "es");
                Translator.Language.Add("Swahili", "sw");
                Translator.Language.Add("Swedish", "sv");
                Translator.Language.Add("Tamil", "ta");
                Translator.Language.Add("Telugu", "te");
                Translator.Language.Add("Thai", "th");
                Translator.Language.Add("Turkish", "tr");
                Translator.Language.Add("Ukrainian", "uk");
                Translator.Language.Add("Urdu", "ur");
                Translator.Language.Add("Vietnamese", "vi");
                Translator.Language.Add("Welsh", "cy");
                Translator.Language.Add("Yiddish", "yi");
            }
        }

        #endregion

        #region Fields

        /// <summary>
        /// The language to translation mode map.
        /// </summary>
        public static Dictionary<string, string> Language;

        #endregion
    }
}
