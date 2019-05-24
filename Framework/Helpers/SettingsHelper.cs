using System;
using System.IO;
using System.Xml;
using ClosedXML.Excel;
using ProductX.Framework.Enums;

namespace ProductX.Framework.Helpers
{
	/// <summary>
	/// Class SettingsHelper.
	/// </summary>
	public static class SettingsHelper
	{
		private static readonly XmlDocument XmlDoc = new XmlDocument();
		public const string ResourcePath = "../../resources/";
		public const string SettingsFileName = "settings.xml";
		public const string DataFileName = "data.xlsx";

		static SettingsHelper()
		{
			Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
		}

		/// <summary>
		/// Gets value from xlsx data file.
		/// </summary>
		/// <param name="sheetName">Sheet name.</param>
		/// <param name="cell">Cell id.</param>
		/// <returns></returns>
		public static string GetXlsValue(string sheetName, string cell) => new XLWorkbook(Path.GetFullPath(ResourcePath) + DataFileName).Worksheet(sheetName).Cell(cell).GetString();

		/// <summary>
		/// Gets xml the value by tag.
		/// </summary>
		/// <param name="settingsValue">Tag name.</param>
		/// <param name="fileName">Settings file name.</param>
		/// <returns>String.</returns>
		public static string GetValue(SettingsValues settingsValue, string fileName = SettingsFileName) => GetValue(settingsValue.ToString(), fileName);

		/// <summary>
		/// Gets xml the value by tag.
		/// </summary>
		/// <param name="tag">The tag.</param>
		/// <param name="fileName">Settings file name.</param>
		/// <returns>String.</returns>
		public static string GetValue(string tag, string fileName = SettingsFileName)
		{
			XmlDoc.Load($"{ResourcePath}{fileName}");

			return XmlDoc.GetElementsByTagName(char.ToLowerInvariant(tag[0]) + tag.Substring(1))[0].InnerText;
		}

		/// <summary>
		/// Sets the value.
		/// </summary>
		/// <param name="tag">The tag.</param>
		/// <param name="value">The value.</param>
		public static void SetValue(string tag, string value, string fileName = SettingsFileName)
		{
			XmlDoc.Load($"{ResourcePath}{fileName}");
			var element = XmlDoc.GetElementsByTagName(tag);
			element[0].InnerText = value;
			XmlDoc.Save($"{ResourcePath}{fileName}");
		}
	}
}