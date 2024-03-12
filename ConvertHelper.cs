﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace Utils.ConvertHelper
{
    public static class ConvertHelper
    {
        /// <summary>
        /// string to camel case
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToCamelCase(this string str)
        {
            var words = str.Split(new[] { "_", " " }, StringSplitOptions.RemoveEmptyEntries);
            return string.Join(string.Empty, words);
        }

        /// <summary>
        /// convert string to unsign string
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ConvertToUnSign(this string s)
        {
            string stFormD = s.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();
            for (int ich = 0; ich < stFormD.Length; ich++)
            {
                System.Globalization.UnicodeCategory uc = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(stFormD[ich]);
                if (uc != System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(stFormD[ich]);
                }
            }
            sb = sb.Replace('Đ', 'D');
            sb = sb.Replace('đ', 'd');
            return (sb.ToString().Normalize(NormalizationForm.FormD));
        }

        /// <summary>
        /// convert string to unsign string
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string Convert2UnSign(this string s)
        {
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = s.Normalize(NormalizationForm.FormD);
            return regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
        }

        /// <summary>
        /// Convert DataTable to list object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<T> ConvertDataTableToObject<T>(this DataTable dt)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = row.ConvertDataRowToObject<T>();
                data.Add(item);
            }
            return data;
        }

        /// <summary>
        /// Convert DataRow to object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dr"></param>
        /// <returns></returns>
        public static T ConvertDataRowToObject<T>(this DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name == column.ColumnName)
                        pro.SetValue(obj, dr[column.ColumnName], null);
                    else
                        continue;
                }
            }
            return obj;
        }

        /// <summary>
        /// hex string to decimal number
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static int ToDecimalNumber(this string hexString)
        {
            return int.Parse(hexString, System.Globalization.NumberStyles.HexNumber);
        }

        /// <summary>
        /// decimal value to hex string
        /// </summary>
        /// <param name="decimalValue"></param>
        /// <returns></returns>
        public static string ToHexString(this int decimalValue)
        {
          return decimalValue.ToString("X");
        }

        private const uint LOCALE_SYSTEM_DEFAULT = 0x0800;
        private const uint LCMAP_HALFWIDTH = 0x00400000;
        private const uint LCMAP_FULLWIDTH = 0x00800000;

        /// <summary>
        /// FullWidth to HalfWidth
        /// </summary>
        /// <param name="fullWidth"></param>
        /// <returns></returns>
        public static string ToHalfWidth(string fullWidth)
        {
            StringBuilder sb = new StringBuilder(256);
            LCMapString(LOCALE_SYSTEM_DEFAULT, LCMAP_HALFWIDTH, fullWidth, -1, sb, sb.Capacity);
            return sb.ToString();
        }

        /// <summary>
        /// HalfWidth to FullWidth
        /// </summary>
        /// <param name="halfWidth"></param>
        /// <returns></returns>
        public static string ToFullWidth(string halfWidth)
        {
            StringBuilder sb = new StringBuilder(256);
            LCMapString(LOCALE_SYSTEM_DEFAULT, LCMAP_FULLWIDTH, halfWidth, -1, sb, sb.Capacity);
            return sb.ToString();
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern int LCMapString(uint Locale, uint dwMapFlags, string lpSrcStr, int cchSrc, StringBuilder lpDestStr, int cchDest);
        
        /// <summary>
        /// String Is Valid Url format
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsValidUrl(this string input)
        {
            Regex regex = new Regex("/^http(s)?:\\/\\/([\\w-]+\\.)+[\\w-]+(\\/[\\w-.\\/?%&=]*)?/");
            return regex.IsMatch(input);
        }

        /// <summary>
        /// Check Is Only Kanji
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsOnlyKanji(this string input)
        {
            Regex regex = new Regex("/^[一-龥]+$/");
            return regex.IsMatch(input);
        }

        /// <summary>
        /// Check Is Only Hiragana FullWidth
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsOnlyHiraganaFullWidth(this string input)
        {
            Regex regex = new Regex("/^[ぁ-ん]+$/");
            return regex.IsMatch(input);
        }

        /// <summary>
        /// Check Is Only Katakana FullWidth
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsOnlyKatakanaFullWidth(this string input)
        {
            Regex regex = new Regex("/^([ァ-ン]|ー)+$/");
            return regex.IsMatch(input);
        }

        /// <summary>
        /// Check Is Valid Email
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsValidEmail(this string input)
        {
            Regex regex = new Regex("/^\\S+@\\S+\\.\\S+$/");
            return regex.IsMatch(input);
        }

        /// <summary>
        /// Convert to seo friendly url (slug)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static string ToSlugUrl(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                throw new ArgumentNullException("input");
            }

            var stringBuilder = new StringBuilder();
            foreach (char c in input.ToArray())
            {
                if (Char.IsLetterOrDigit(c))
                {
                    stringBuilder.Append(c);
                }
                else if (c == ' ')
                {
                    stringBuilder.Append("-");
                }
            }

            return stringBuilder.ToString().ToLower();
        }

        /// <summary>
        /// Get First Day Of Week
        /// </summary>
        /// <param name="dayInWeek"></param>
        /// <returns></returns>
        public static DateTime FirstDayOfWeek(this DateTime dayInWeek)
        {
            CultureInfo defaultCultureInfo = CultureInfo.CurrentCulture;
            return dayInWeek.FirstDayOfWeek(defaultCultureInfo);
        }

        /// <summary>
        /// Get First Day Of Week
        /// </summary>
        /// <param name="dayInWeek"></param>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        public static DateTime FirstDayOfWeek(this DateTime dayInWeek, CultureInfo cultureInfo)
        {
            DayOfWeek firstDay = cultureInfo.DateTimeFormat.FirstDayOfWeek;
            DateTime firstDayInWeek = dayInWeek.Date;

            while (firstDayInWeek.DayOfWeek != firstDay)
            {
                firstDayInWeek = firstDayInWeek.AddDays(-1);
            }

            return firstDayInWeek;
        }

        /// <summary>
        /// Get Last Day Of Week
        /// </summary>
        /// <param name="dayInWeek"></param>
        /// <returns></returns>
        public static DateTime LastDayOfWeek(this DateTime dayInWeek)
        {
            CultureInfo defaultCultureInfo = CultureInfo.CurrentCulture;
            return dayInWeek.LastDayOfWeek(defaultCultureInfo);
        }

        /// <summary>
        ///  Get Last Day Of Week
        /// </summary>
        /// <param name="dayInWeek"></param>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        public static DateTime LastDayOfWeek(this DateTime dayInWeek, CultureInfo cultureInfo)
        {
            DateTime firstDayInWeek = dayInWeek.FirstDayOfWeek(cultureInfo);
            return firstDayInWeek.AddDays(7);
        }

        /// <summary>
        /// get start date of week
        /// </summary>
        /// <param name="dayInWeek"></param>
        /// <param name="startOfWeek"></param>
        /// <returns></returns>
        public static DateTime StartOfWeek(this DateTime dayInWeek, DayOfWeek startOfWeek)
        {
            int diff = (7 + (dayInWeek.DayOfWeek - startOfWeek)) % 7;
            return dayInWeek.AddDays(-1 * diff).Date;
        }

        /// <summary>
        /// To Pascal Case
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ToPascalCase(this string input)
        {
            string result = string.Join("", input?.Select(c => Char.IsLetterOrDigit(c) ? c.ToString().ToLower() : "_").ToArray());
            var arr = result?
                .Split(new[] { '_' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => $"{s.Substring(0, 1).ToUpper()}{s.Substring(1)}");
            result = string.Join("", arr);
            return result;
        }
    }
}
