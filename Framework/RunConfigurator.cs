using System;
using System.IO;
using System.Xml;
using ClosedXML.Excel;
using ProductX.Framework.Enums;

namespace ProductX.Framework
{
	/// <summary>
	///     Class RunConfigurator.
	/// </summary>
	public static class RunConfigurator 
	{
		private static readonly XmlDocument XmlDoc = new XmlDocument(); // Create an XML document object
		public const string ResourcePath = "../../resources/";
		public const string RunFileName = "run.xml";
		public const string DataFileName = "data.xlsx";

		static RunConfigurator()
		{
			Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
		}

		/// <summary>
		///     Gets the value.
		/// </summary>
		/// <param name="tag">The tag.</param>
		/// <returns>String.</returns>
		public static string GetValue(string tag, string fileName = RunFileName)
		{
			//if (fileName == null) fileName = RunFileName;
			XmlDoc.Load($"{ResourcePath}{fileName}"); // Load the XML document from the specified file
			var browser = XmlDoc.GetElementsByTagName(char.ToLowerInvariant(tag[0]) + tag.Substring(1));
			return browser[0].InnerText;
		}

		public static string GetValue(RunValues runValue, string fileName = RunFileName) => GetValue(runValue.ToString(), fileName);

		/// <summary>
		/// Sets the value.
		/// </summary>
		/// <param name="tag">The tag.</param>
		/// <param name="value">The value.</param>
		public static void SetValue(string tag, string value, string fileName = RunFileName)
		{
			XmlDoc.Load($"{ResourcePath}{fileName}"); // Load the XML document from the specified file
			var element = XmlDoc.GetElementsByTagName(tag);
			element[0].InnerText = value;
			XmlDoc.Save($"{ResourcePath}{fileName}");
		}

		public static string GetXlsValue(string sheetName, string cell)
		{
			return new XLWorkbook(Path.GetFullPath(ResourcePath) + DataFileName).Worksheet(sheetName).Cell(cell).GetString();
		}

		public static bool IsResourceFileExists(string fileName) => File.Exists($"{Path.GetFullPath(ResourcePath)}{fileName}");
	}
}