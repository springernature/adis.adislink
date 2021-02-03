using System;
using System.Security.Cryptography;
using System.Data;
using System.Configuration;
using System.Text.RegularExpressions;

namespace ShortUrl
{

    /// <summary>
    /// Range of utility methods
    /// </summary>
    public static class Utils
    {
        private static string shorturl_chars_lcase = "abcdefgijkmnopqrstwxyz";
        private static string shorturl_chars_ucase = "ABCDEFGHJKLMNPQRSTWXYZ";
        private static string shorturl_chars_numeric = "23456789";

        public static string CheckIfUrlExists(string url)
        {
            string sql = "SELECT short_url FROM url WHERE real_url = @real_url";
            Params p = new Params();
            p.Add("@real_url", Clean(url));
            DataTable dt = SqlServer.Recordset(sql, p);
            string short_url = "";

            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                short_url = row["short_url"].ToString();
            }

            return short_url;
        }

        public static string UniqueShortUrl()
        {
            string short_url = RandomCharacters();

            string sql = "SELECT COUNT(*) FROM url WHERE short_url = @short_url";
            Params p = new Params();
            p.Add("@short_url", short_url);
            int url_count = Int32.Parse(SqlServer.Scalar(sql, p));

            if (url_count == 0)
            {
                return short_url;
            }
            else
            {
                return RandomCharacters();
            }
        }

        public static string Clean(string url)
        {
            string filter = @"((https?):((//)|(\\\\))+[\w\d:#@%/;$()~_?\+-=\\\.&]*)";
            Regex rx = new Regex(filter);


            return url;
        }

        public static string CleanAndExtractShortURL(string url)
        {
            string filter = @"((https?):((//)|(\\\\))+[\w\d:#@%/;$()~_?\+-=\\\.&]*)";
            Regex rx = new Regex(filter);

            string[] split = url.Split('/');
            url = split[split.Length - 1];

            return url;
        }

        public static string PublicShortUrl(string short_url)
        {
            return "http://" + ConfigurationManager.AppSettings["DomainName"].ToString() + "/" + short_url;
        }

        public static string InternalShortUrl(string short_url)
        {
            return short_url.Replace("http://" + ConfigurationManager.AppSettings["DomainName"].ToString() + "/", "");

        }

        public static string InternalShortUrlFromRedirect(string short_url)
        {
            return short_url.Replace("http://" + ConfigurationManager.AppSettings["DomainName"].ToString() + "/Redirection.aspx?404;http://" + ConfigurationManager.AppSettings["DomainName"].ToString() + ":80/", "");
        }

        public static void AddUrlToDatabase(Container oShortUrl)
        {
            string sql = "INSERT INTO url (short_url, created_by, real_url) VALUES (@short_url, @created_by, @real_url)";
            Params p = new Params();
            p.Add("@short_url", oShortUrl.ShortenedUrl);
            p.Add("@created_by", oShortUrl.CreatedBy);
            p.Add("@real_url", Clean(oShortUrl.RealUrl));
            SqlServer.Execute(sql, p);
        }

        public static Container RetrieveUrlFromDatabase(string internal_url)
        {
            Container oShortUrl = new Container();
            oShortUrl.ShortenedUrl = internal_url;

            string sql = "SELECT * FROM url WHERE short_url = @short_url";
            Params p = new Params();
            p.Add("@short_url", CleanAndExtractShortURL(internal_url));
            DataTable dt = SqlServer.Recordset(sql, p);

            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                oShortUrl.CreateDate = DateTime.Parse(row["create_date"].ToString());
                oShortUrl.CreatedBy = row["created_by"].ToString();
                oShortUrl.RealUrl = row["real_url"].ToString();
            }

            return oShortUrl;
        }

        public static bool HasValue(object o)
        {
            if (o == null)
            {
                return false;
            }

            if (o == System.DBNull.Value)
            {
                return false;
            }

            if (o is String)
            {
                if (((String)o).Trim() == String.Empty)
                {
                    return false;
                }
            }

            return true;
        }

        public static string RandomCharacters()
        {
            // Create a local array containing supported short-url characters
            // grouped by types.
            char[][] charGroups = new char[][]
            {
                shorturl_chars_lcase.ToCharArray(),
                shorturl_chars_ucase.ToCharArray(),
                shorturl_chars_numeric.ToCharArray()
            };

            // Use this array to track the number of unused characters in each
            // character group.
            int[] charsLeftInGroup = new int[charGroups.Length];

            // Initially, all characters in each group are not used.
            for (int i = 0; i < charsLeftInGroup.Length; i++)
                charsLeftInGroup[i] = charGroups[i].Length;

            // Use this array to track (iterate through) unused character groups.
            int[] leftGroupsOrder = new int[charGroups.Length];

            // Initially, all character groups are not used.
            for (int i = 0; i < leftGroupsOrder.Length; i++)
                leftGroupsOrder[i] = i;

            // Because we cannot use the default randomizer, which is based on the
            // current time (it will produce the same "random" number within a
            // second), we will use a random number generator to seed the
            // randomizer.

            // Use a 4-byte array to fill it with random bytes and convert it then
            // to an integer value.
            byte[] randomBytes = new byte[4];

            // Generate 4 random bytes.
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            rng.GetBytes(randomBytes);

            // Convert 4 bytes into a 32-bit integer value.
            int seed = (randomBytes[0] & 0x7f) << 24 |
                        randomBytes[1] << 16 |
                        randomBytes[2] << 8 |
                        randomBytes[3];

            // Now, this is real randomization.
            Random random = new Random(seed);

            // This array will hold short-url characters.
            char[] short_url = null;

            // Allocate appropriate memory for the short-url.
            short_url = new char[random.Next(5, 5)];

            // Index of the next character to be added to short-url.
            int nextCharIdx;

            // Index of the next character group to be processed.
            int nextGroupIdx;

            // Index which will be used to track not processed character groups.
            int nextLeftGroupsOrderIdx;

            // Index of the last non-processed character in a group.
            int lastCharIdx;

            // Index of the last non-processed group.
            int lastLeftGroupsOrderIdx = leftGroupsOrder.Length - 1;

            // Generate short-url characters one at a time.
            for (int i = 0; i < short_url.Length; i++)
            {
                // If only one character group remained unprocessed, process it;
                // otherwise, pick a random character group from the unprocessed
                // group list. To allow a special character to appear in the
                // first position, increment the second parameter of the Next
                // function call by one, i.e. lastLeftGroupsOrderIdx + 1.
                if (lastLeftGroupsOrderIdx == 0)
                    nextLeftGroupsOrderIdx = 0;
                else
                    nextLeftGroupsOrderIdx = random.Next(0,
                                                         lastLeftGroupsOrderIdx);

                // Get the actual index of the character group, from which we will
                // pick the next character.
                nextGroupIdx = leftGroupsOrder[nextLeftGroupsOrderIdx];

                // Get the index of the last unprocessed characters in this group.
                lastCharIdx = charsLeftInGroup[nextGroupIdx] - 1;

                // If only one unprocessed character is left, pick it; otherwise,
                // get a random character from the unused character list.
                if (lastCharIdx == 0)
                    nextCharIdx = 0;
                else
                    nextCharIdx = random.Next(0, lastCharIdx + 1);

                // Add this character to the short-url.
                short_url[i] = charGroups[nextGroupIdx][nextCharIdx];

                // If we processed the last character in this group, start over.
                if (lastCharIdx == 0)
                    charsLeftInGroup[nextGroupIdx] =
                                              charGroups[nextGroupIdx].Length;
                // There are more unprocessed characters left.
                else
                {
                    // Swap processed character with the last unprocessed character
                    // so that we don't pick it until we process all characters in
                    // this group.
                    if (lastCharIdx != nextCharIdx)
                    {
                        char temp = charGroups[nextGroupIdx][lastCharIdx];
                        charGroups[nextGroupIdx][lastCharIdx] =
                                    charGroups[nextGroupIdx][nextCharIdx];
                        charGroups[nextGroupIdx][nextCharIdx] = temp;
                    }
                    // Decrement the number of unprocessed characters in
                    // this group.
                    charsLeftInGroup[nextGroupIdx]--;
                }

                // If we processed the last group, start all over.
                if (lastLeftGroupsOrderIdx == 0)
                    lastLeftGroupsOrderIdx = leftGroupsOrder.Length - 1;
                // There are more unprocessed groups left.
                else
                {
                    // Swap processed group with the last unprocessed group
                    // so that we don't pick it until we process all groups.
                    if (lastLeftGroupsOrderIdx != nextLeftGroupsOrderIdx)
                    {
                        int temp = leftGroupsOrder[lastLeftGroupsOrderIdx];
                        leftGroupsOrder[lastLeftGroupsOrderIdx] =
                                    leftGroupsOrder[nextLeftGroupsOrderIdx];
                        leftGroupsOrder[nextLeftGroupsOrderIdx] = temp;
                    }
                    // Decrement the number of unprocessed groups.
                    lastLeftGroupsOrderIdx--;
                }
            }

            // Convert password characters into a string and return the result.
            return new string(short_url);
        }
    }
}