using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MB.TextParsing
{
    public class SeasonEpisode
    {
        public int Season { get; set; }
        public int[] Episode { get; set; }

        public SeasonEpisode()
        {

        }

        public SeasonEpisode(int Season, int[] EpisodeNumber)
        {
            this.Season = Season;
            this.Episode = EpisodeNumber;
        }

        public SeasonEpisode(int Season, int EpisodeNumber)
        {
            this.Season = Season;
            this.Episode = new int[] { EpisodeNumber };
        }

        public SeasonEpisode (string Season, string[] EpisodeNumber)
        {
            this.Season = Convert.ToInt32(Season);

            int[] ep;
            ep = new int[EpisodeNumber.Length];
            for (int i = 0; i < EpisodeNumber.Length; i++)
            {
                ep[i] = Convert.ToInt32(EpisodeNumber[i]);
            }

            this.Episode = ep;
        }

        public SeasonEpisode(string Season, string Episode)
        {
            this.Season = Convert.ToInt32(Season);
            this.Episode = new int[] { Convert.ToInt32(Episode) };
        }
    }

    public static class TextParser
    {
        static List<char> RestrictedCharacters = new List<char> { '(', ')', '!', '?', '/', '[', ']', '*', '&', '%', '$', '#', '@', '-', '\'' };
        static List<String> RestrictedVideoInfoStrings = new List<string> { "720p", "1080p", "x.264", "x264" };
        static Dictionary<string, string> SnEpMatches = new Dictionary<string, string>()
        {
            { "SxxExxExx", @"s\d{1,4}e\d{1,3}e\d{1,3}" },
            { "SxxExx",  @"s\d{1,4}e\d{1,3}" },
            { "xxXxx", @"\d{1,2}x\d{1,3}" }
        };
        static Dictionary<string, string> DateMatches = new Dictionary<string, string>()
        {
            { "xxxxxx", @"\d{6}" },
            { "yyyy.mm.dd", @"\d{4}[^a-zA-Z0-9]\d{2}[^a-zA-Z0-9]\d{2}" },
            { "xx.xx.yyyy", @"\d{2}[^a-zA-Z0-9]\d{2}[^a-zA-Z0-9]\d{4}" }
        };

        public static string GetSafeString(string FileName)
        {
            string resultString = FileName.ToLower();
            if (FileName.Contains("(") && FileName.Contains(")"))
            {
                int x = resultString.IndexOf('(');
                int y = resultString.IndexOf(')');
                string parenthetical = resultString.Substring(x, y - x);

                resultString = resultString.Replace(parenthetical, null);
            }

            foreach (char restrictedCharacter in RestrictedCharacters)
            {
                resultString = resultString.Replace(restrictedCharacter.ToString(), null);
            }

            foreach (string restrictedVideoInfo in RestrictedVideoInfoStrings)
            {
                resultString = resultString.Replace(restrictedVideoInfo, null);
            }

            resultString = new Regex(@"\s\s+").Replace(resultString, " ").Trim();
            return resultString;
        }        
        public static string GetRegexString(string FileName)
        {
            string resultString = GetSafeString(FileName);
            resultString = resultString.Replace('?', '.');
            resultString = resultString.Replace(".", ".?");
            resultString = resultString.Replace(' ', '.');

            foreach (char restrictedChar in RestrictedCharacters)
                if (restrictedChar != '.' && restrictedChar != '?')
                    resultString = resultString.Replace(restrictedChar.ToString(), restrictedChar + "?");

            return resultString;
        }
        public static string GetFileExtension(string FileName)
        {
            int f = FileName.LastIndexOf('.');
            int l = FileName.Length - f;

            return FileName.Substring(f, l);
        }

        public static bool TestSimpleMatch(string Pattern, string Text)
        {
            string regString = string.Format("^{0}$", Regex.Escape(Pattern.ToLower()).Replace("\\*", ".*"));
            Regex matchRegex = new Regex(regString);

            return matchRegex.IsMatch(Text.ToLower());
        }

        public static SeasonEpisode GetSeasonEpisodeNumber(string FileName)
        {
            SeasonEpisode output = null;

            foreach (var matchObject in SnEpMatches)
            {
                string regKey = matchObject.Key;
                string regString = matchObject.Value;
                Regex regex = new Regex(regString);
                Match regResult = regex.Match(FileName.ToLower());

                if (regResult.Success)
                {
                    string matchString = regResult.Value;
                    string outputString;
                    string[] snep;
                    switch (regKey)
                    {
                        case "SxxExxExx":
                            outputString = matchString.Replace('s'.ToString(), null);
                            snep = outputString.Split('e');
                            output = new SeasonEpisode(snep[0], new string[] { snep[1], snep[2] });
                            break;

                        case "SxxExx":
                            outputString = matchString.Replace('s'.ToString(), null);
                            snep = outputString.Split('e');
                            output = new SeasonEpisode(snep[0], snep[1]);
                            break;

                        case "xxXxx":
                            snep = matchString.Split('x');
                            output = new SeasonEpisode(snep[0], snep[1]);
                            break;
                    }

                    break;
                }
            }
            
            return output;
        }
        public static DateTime? GetAirDate(string FileName)
        {
            DateTime output = DateTime.MinValue;

            foreach (var matchObject in DateMatches)
            {
                string regKey = matchObject.Key;
                string regString = matchObject.Value;
                Regex regex = new Regex(regString);
                Match regResult = regex.Match(FileName.ToLower());

                if (regResult.Success)
                {
                    string matchString = regResult.Value;
                    string[] dateArr;
                    int val1;
                    int val2;
                    int val3;
                    int y = 0;
                    int m = 0;
                    int d = 0;

                    switch (regKey)
                    {
                        case "xxxxxx":
                            val1 = Convert.ToInt32(matchString.Substring(0, 2));
                            val2 = Convert.ToInt32(matchString.Substring(2, 2));
                            val3 = Convert.ToInt32(matchString.Substring(4, 2));

                            if (val1 > 12)
                            {
                                y = Convert.ToInt32("20" + val1.ToString());
                                m = val2;
                                d = val3;
                            }
                            else
                            {
                                y = Convert.ToInt32("20" + val3.ToString());
                                m = val1;
                                d = val2;
                            }
                            break;

                        case "yyyy.mm.dd":
                            dateArr = new Regex(@"\D").Split(matchString);

                            y = Convert.ToInt32(dateArr[0]);
                            m = Convert.ToInt32(dateArr[1]);
                            d = Convert.ToInt32(dateArr[2]);
                            break;

                        case "xx.xx.yyyy":
                            dateArr = new Regex(@"\D").Split(matchString);
                            val1 = Convert.ToInt32(dateArr[0]);
                            val2 = Convert.ToInt32(dateArr[1]);
                            y = Convert.ToInt32(dateArr[2]);

                            if (val1 > 12)
                            {
                                m = val2;
                                d = val1;
                            }
                            else
                            {
                                m = val1;
                                d = val2;
                            }

                            break;
                    }

                    output = new DateTime(y, m, d);
                    break;
                }
            }

            if (output != DateTime.MinValue)
                return output;
            else
                return null;
        }
        public static string GetSeriesName(string FileName, List<string> SeriesList, Dictionary<string, string> SearchTerm = null)
        {
            string seriesName = null;

            FileName = GetSafeString(FileName);
            foreach (string series in SeriesList)
            {
                string seriesRegex = GetRegexString(series);

                if (new Regex(seriesRegex).IsMatch(FileName))
                    return series;

                else if (SearchTerm != null)
                {
                    foreach (string searchTerm in SearchTerm.Keys)
                    {
                        if (new Regex(GetRegexString(searchTerm)).IsMatch(FileName))
                            return SearchTerm[searchTerm];
                    }
                }
            }

            return seriesName;
        }
    }
}
