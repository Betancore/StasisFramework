using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Download;
using Google.Apis.Drive.v3;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;
using NUnit.Framework;
using ProductX.Framework.Enums;
using ProductX.Framework.Models.Google;
using Spritely.Recipes;
using Thread = System.Threading.Thread;

namespace ProductX.Framework.Helpers
{
	/// <summary>
	///     Class GoogleApiHelper.
	/// </summary>
	public static class GoogleApiHelper
	{
		private static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
		private const string AuthPath = "../../resources/";
		private const string LinkRegex = "https(.+)[0-9]{1}\"";
		private const string DataFile = "data.xlsx";
		private const string ColumnRegexPattern = @"([A-Za-z]+)";
		private const string RowRegexPattern = "([0-9]+)";
		private const string DimensionColumnType = "COLUMNS";

		static GoogleApiHelper()
		{
			Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
		}

		/// <summary>
		/// Gets values from google spreadsheet from specified range.
		/// </summary>
		/// <param name="spreadsheetId">Spreadsheet Id.</param>
		/// <param name="range">Range to receive (e.g. "Tab!A3:D6").</param>
		/// <param name="apiUser">User to establish connection.</param>
		/// <returns>IList<IList<object>>.</returns>
		public static IList<IList<object>> GetValuesRange(string spreadsheetId, string range, GoogleApiUser apiUser) =>
			ConnectToSheetsApi(apiUser).Spreadsheets.Values.Get(spreadsheetId, range).Execute().Values;

		/// <summary>
		/// Sets single value from one cell.
		/// </summary>
		/// <param name="spreadSheetId">Spreadsheet Id.</param>
		/// <param name="range">Range to set (e.g. "Tab!A3:A3").</param>
		/// <param name="apiUser">User to establish connection.</param>
		/// <param name="value">Value to set.</param>
		public static void SetSheetSingleValue(string spreadSheetId, string range, string value, GoogleApiUser apiUser)
		{
			var service = ConnectToSheetsApi(apiUser);
			var valueRange = new ValueRange {MajorDimension = DimensionColumnType };
			var valueList = new List<object> { value };
			valueRange.Values = new List<IList<object>> { valueList };
			var request = service.Spreadsheets.Values.Update(valueRange, spreadSheetId, range);
			request.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;
			request.Execute();
		}

		/// <summary>
		/// Sets cell color.
		/// </summary>
		/// <param name="range">Range to set (e.g. "Tab!A3:A3").</param>
		/// <param name="spreadSheetTabId">Spreadsheet tab Id number from spreadsheet url (e.g. 0 in ...gid=0).</param>
		/// <param name="color">Cell color.</param>
		/// <param name="spreadSheetId">Spreadsheet Id.</param>
		/// <param name="apiUser">User to establish connection.</param>
		public static void SetCellColor(string range, Color color, string spreadSheetId, GoogleApiUser apiUser, int spreadSheetTabId = 0)
		{
			var regex = new Regex(ColumnRegexPattern);
			var startColumnNumber = GetExcelColumnNumber(regex.Matches(range)[1].Value) - 1;
			var endColumnNumber = GetExcelColumnNumber(regex.Matches(range)[2].Value);
			regex = new Regex(RowRegexPattern);
			var startRowNumber = int.Parse(regex.Matches(range)[0].Value) - 1;
			var endRowNumber = int.Parse(regex.Matches(range)[1].Value);
			var service = ConnectToSheetsApi(apiUser);

			var rowData = new RowData
			{
				Values = new List<CellData>
				{
					new CellData
					{
						UserEnteredFormat = new CellFormat
						{
							BackgroundColor = color
						}
					}
				}
			};

			var requestToBody = new Request
			{
				UpdateCells = new UpdateCellsRequest
				{
					Range = new GridRange
					{
						SheetId = spreadSheetTabId,
						StartRowIndex = startRowNumber,
						EndRowIndex = endRowNumber,
						StartColumnIndex = startColumnNumber,
						EndColumnIndex = endColumnNumber
					},
					Rows = new List<RowData>
					{
						rowData
					},
					Fields = "*"
				}
			};

			var body = new BatchUpdateSpreadsheetRequest {Requests = new List<Request> {requestToBody}};
			service.Spreadsheets.BatchUpdate(body, spreadSheetId).Execute();
		}

		/// <summary>
		/// Gets last or next column Id.
		/// </summary>
		/// <param name="spreadSheetId">Spreadsheet Id.</param>
		/// <param name="sheet">Sheet name.</param>
		/// <param name="isNext">Return the last column name or the next to the last.</param>
		/// <param name="apiUser">User to establish connection.</param>
		/// <returns>Column Id</returns>
		public static string GetLastColumnId(string spreadSheetId, string sheet, GoogleApiUser apiUser, bool isNext = true)
		{
			var service = ConnectToSheetsApi(apiUser);
			var request = service.Spreadsheets.Values.Get(spreadSheetId, sheet);
			request.MajorDimension = SpreadsheetsResource.ValuesResource.GetRequest.MajorDimensionEnum.COLUMNS;
			var response = request.Execute();

			return isNext ? GetExcelColumnName(response.Values.Count + 1) : GetExcelColumnName(response.Values.Count);
		}

		/// <summary>
		/// Add new empty columns.
		/// </summary>
		/// <param name="count">Count of columns to add.</param>
		/// <param name="spreadSheetTabId">Spreadsheet tab Id number from spreadsheet url (e.g. 0 in ...gid=0).</param>
		/// <param name="spreadSheetId">Spreadsheet Id.</param>
		/// <param name="apiUser">User to establish connection.</param>
		public static void AppendColumns(int count, string spreadSheetId, GoogleApiUser apiUser, int spreadSheetTabId = 0)
		{
			var service = ConnectToSheetsApi(apiUser);
			var body = new BatchUpdateSpreadsheetRequest();

			var requestToBody = new Request
			{
				AppendDimension = new AppendDimensionRequest
				{
					Dimension = DimensionColumnType,
					Length = count,
					SheetId = spreadSheetTabId
				}
			};

			body.Requests = new List<Request> {requestToBody};
			var request = service.Spreadsheets.BatchUpdate(body, spreadSheetId);
			request.Execute();
		}

		/// <summary>
		/// Store range of values.
		/// </summary>
		/// <param name="range">Range to set (e.g. "Tab!A3:D3").</param>
		/// <param name="spreadsheetId">Spreadsheet Id.</param>
		/// <param name="values">Values to set.</param>
		/// <param name="apiUser">User to establish connection.</param>
		public static void StoreRange(string range, string spreadsheetId, IList<IList<object>> values, GoogleApiUser apiUser = null)
		{
			var valuesRange = new ValueRange
			{
				MajorDimension = DimensionColumnType,
				Values = values
			};

			var request = ConnectToSheetsApi(apiUser).Spreadsheets.Values.Update(valuesRange, spreadsheetId, range);
			request.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;
			request.Execute();
		}

		/// <summary>
		/// Checks the email is received.
		/// </summary>
		/// <param name="apiUser">User to establish connection.</param>
		/// <param name="emailSubject">Email subject.</param>
		public static void CheckEmailIsReceived(GoogleApiUser apiUser, string emailSubject)
		{
			Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
			var counter = 0;

			while (counter < 4)
			{
				Thread.Sleep(30000);
				Browser.GetDriver().Navigate().Refresh();
				counter++;
			}

			var service = ConnectToGmailApi(apiUser.SecretFile, apiUser.SecretFileReworked, apiUser.ApiName);
			var result = new List<Message>();
			var request = service.Users.Messages.List(apiUser.Email);
			var response = request.Execute();
			result.AddRange(response.Messages);
			var message = service.Users.Messages.Get(apiUser.Email, result[0].Id).Execute();
			var subject = message.Payload.Headers[19].Value;
			var date = message.Payload.Headers[18].Value.Substring(0, message.Payload.Headers[17].Value.Length - 14);
			var currentDate = DateTime.Now;
			var strCurrentDate = $"{currentDate:ddd, d MMM yyyy}";
			Assert.IsTrue(DateTimeOffset.Parse(date.ToLower().Trim()).UtcDateTime == DateTimeOffset.Parse(strCurrentDate.ToLower().Trim()).UtcDateTime,
				"Email date is incorrect. Should be: " + strCurrentDate + " but: " + date);
			Assert.IsTrue(subject.ToLower().Trim() == emailSubject.ToLower().Trim());
		}

		/// <summary>
		/// Gets link send in email.
		/// </summary>
		/// <param name="subject">Email subject.</param>
		/// <param name="apiUser">User to establish connection.</param>
		/// <param name="useCurrentDate">Use current date to filter other emails with same subjects.</param>
		/// <returns>Link.</returns>
		public static string GetLink(string subject, GoogleApiUser apiUser, bool useCurrentDate = false)
		{
			var service = ConnectToGmailApi(apiUser.SecretFile, apiUser.SecretFileReworked, apiUser.ApiName);
			var dataFromEmail = GetDataFromEmail(service, apiUser.Email, LinkRegex, subject, useCurrentDate);

			return dataFromEmail == string.Empty ? string.Empty : dataFromEmail.Substring(0, dataFromEmail.Length - 1);
		}

		/// <summary>
		/// Gets test data from google spreadsheet directly or from exported locally in .xlsx file.
		/// </summary>
		/// <param name="range">Range of data to get (e.g. "Tab!A3:A3").</param>
		/// <param name="spreadSheetId">Spreadsheet Id.</param>
		/// <param name="apiUser">User to establish connection.</param>
		/// <returns>Data as string.</returns>
		public static string GetData(string range, string spreadSheetId, GoogleApiUser apiUser)
		{
			var firstSymbol = range.IndexOf("!", StringComparison.Ordinal) + 1;
			var lastSymbol = range.IndexOf(":", StringComparison.Ordinal) - firstSymbol;
			var cell = range.Substring(firstSymbol, lastSymbol);
			var sheetName = range.Substring(0, range.IndexOf("!", StringComparison.Ordinal));

			return SettingsHelper.GetValue(SettingsValues.UseLocalData) == "yes"
				? SettingsHelper.GetXlsValue(sheetName, cell)
				: GetDataDirectly(range, spreadSheetId, apiUser);
		}

		/// <summary>
		/// Gets data directly from google spreadsheet
		/// </summary>
		/// <param name="range">Range of data to get (e.g. "Tab!A3:A3").</param>
		/// <param name="spreadSheetId">Spreadsheet Id.</param>
		/// <param name="apiUser">User to establish connection.</param>
		/// <returns>Data as string.</returns>
		public static string GetDataDirectly(string range, string spreadSheetId, GoogleApiUser apiUser = null)
		{
			var service = ConnectToSheetsApi(apiUser);

			return service.Spreadsheets.Values.Get(spreadSheetId, range).Execute().Values[0][0].ToString();
		}

		/// <summary>
		/// Exports spreadsheet with test data from google spreadsheet.
		/// </summary>
		/// <param name="apiUser">User to establish connection.</param>
		/// <param name="spreadSheetId">Spreadsheet Id.</param>
		public static void ExportTestDataTable(GoogleApiUser apiUser, string spreadSheetId)
		{
			var driveService = ConnectToDriveApi(apiUser);
			var request = driveService.Files.Export(spreadSheetId, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
			var stream = new MemoryStream();
			// Add a handler which will be notified on progress changes.
			// It will notify on each chunk download and when the
			// download is completed or failed.
			request.MediaDownloader.ProgressChanged +=
				(progress) =>
				{
					switch (progress.Status)
					{
						case DownloadStatus.Downloading:
						{
							Console.WriteLine(progress.BytesDownloaded);
							break;
						}
						case DownloadStatus.Completed:
						{
							Console.WriteLine("Download complete.");
							break;
						}
						case DownloadStatus.Failed:
						{
							Console.WriteLine("Download failed.");
							break;
						}
					}
				};

			request.Download(stream);
			var filePath = $"{SettingsHelper.ResourcePath}{DataFile}";

			if (File.Exists(filePath))
			{
				File.Delete(filePath);
			}

			using (var fileStream = File.Create(filePath))
			{
				stream.Seek(0, SeekOrigin.Begin);
				stream.CopyTo(fileStream);
			}

			for (var i = 0; i < 30; i++)
			{
				if (Directory.EnumerateFiles(filePath).Any())
				{
					break;
				}

				Thread.Sleep(1000);
			}
		}

		private static SheetsService ConnectToSheetsApi(GoogleApiUser apiUser)
		{
			UserCredential credential;

			using (var stream = new FileStream(Path.GetFullPath(AuthPath + apiUser.SecretFile), FileMode.Open, FileAccess.Read))
			{
				var credPath = Path.GetFullPath(AuthPath + apiUser.SecretFileReworked);
				credential = GoogleWebAuthorizationBroker.AuthorizeAsync(GoogleClientSecrets.Load(stream).Secrets, Scopes, "user", CancellationToken.None, new FileDataStore(credPath, true)).Result;
			}

			var service = new SheetsService(new BaseClientService.Initializer { HttpClientInitializer = credential, ApplicationName = apiUser.ApiName });

			return service;
		}

		private static DriveService ConnectToDriveApi(GoogleApiUser apiUser)
		{
			UserCredential credential;
			string[] driveScopes = { DriveService.Scope.Drive, DriveService.Scope.DriveFile, DriveService.Scope.DriveReadonly, SheetsService.Scope.Spreadsheets };

			using (var stream = new FileStream(Path.GetFullPath(AuthPath + apiUser.SecretFile), FileMode.Open, FileAccess.Read))
			{
				var credPath = Path.GetFullPath(AuthPath + apiUser.SecretFileReworked);
				credential = GoogleWebAuthorizationBroker.AuthorizeAsync(GoogleClientSecrets.Load(stream).Secrets, driveScopes, "user", CancellationToken.None, new FileDataStore(credPath, true)).Result;
			}

			var service = new DriveService(new BaseClientService.Initializer
			{
				HttpClientInitializer = credential,
				ApplicationName = apiUser.ApiName
			});

			return service;
		}

		private static GmailService ConnectToGmailApi(string jsonPath1, string jsonPath2, string appName)
		{
			UserCredential credential;
			using (var stream = new FileStream(Path.GetFullPath(AuthPath + jsonPath1), FileMode.Open, FileAccess.Read))
			{
				var credPath = Path.GetFullPath(AuthPath + jsonPath2);
				credential = GoogleWebAuthorizationBroker.AuthorizeAsync(GoogleClientSecrets.Load(stream).Secrets,
						new[] { GmailService.Scope.GmailLabels, GmailService.Scope.MailGoogleCom, GmailService.Scope.GmailModify, GmailService.Scope.GmailCompose }, "user", CancellationToken.None,
						new FileDataStore(credPath, true)).
					Result;
			}

			// Create Google Mail API service.
			var service = new GmailService(new BaseClientService.Initializer { HttpClientInitializer = credential, ApplicationName = appName });
			return service;
		}

		private static string GetExcelColumnName(int columnNumber)
		{
			var dividend = columnNumber;
			var columnName = string.Empty;
			while (dividend > 0)
			{
				var modulo = (dividend - 1) % 26;
				columnName = Convert.ToChar(65 + modulo) + columnName;
				dividend = (dividend - modulo) / 26;
			}

			return columnName;
		}

		private static string GetDataFromEmail(GmailService service, string email, string pattern, string subject = null, bool useCurrentDate = false)
		{
			var result = new List<Message>();
			var request = service.Users.Messages.List(email);
			request.MaxResults = 10;
			var response = request.Execute();
			result.AddRange(response.Messages);
			var messagesList = new List<Message>();
			Message message;

			if (subject != null)
			{
				foreach (var res in result)
				{
					var getRequest = service.Users.Messages.Get(email, res.Id);
					messagesList.Add(getRequest.Execute());
				}

				message = useCurrentDate
					? messagesList.FirstOrDefault(x =>
						x.Payload.Headers.FirstOrDefault(h => h.Name == "Subject")
							?.Value
							.Replace("+", "")
						== subject.Replace("+", "") && DateTime.Parse(x.Payload
							.Headers.FirstOrDefault(h => h.Name == "Date")
							?.Value) > DateTime.UtcNow.AddMinutes(-5))
					: messagesList.FirstOrDefault(x => x.Payload.Headers.FirstOrDefault(h => h.Name == "Subject")
															?.Value.Replace("+", "") == subject.Replace("+", ""));
			}
			else
			{
				message = service.Users.Messages.Get(email, result[0].Id).Execute();
			}

			if (message?.Id == null) return string.Empty;

			var bodyData = message.Payload.Body.Data;
			var decodedData = new List<byte[]>();

			if (bodyData == null)
			{
				decodedData.AddRange(message.Payload.Parts.Select(part => part.Body.Data.DecodeBase64Url()));
			}
			else
			{
				decodedData.Add(bodyData.DecodeBase64Url());
			}

			var decodedString = "";

			foreach (var part in decodedData)
			{
				decodedString += Encoding.UTF8.GetString(part);
			}

			var regex = new Regex(pattern, RegexOptions.Multiline);

			return regex.Match(decodedString).Value;
		}

		private static int GetExcelColumnNumber(string columnName)
		{
			if (string.IsNullOrEmpty(columnName)) throw new ArgumentNullException(nameof(columnName));

			columnName = columnName.ToUpperInvariant();

			var sum = 0;

			foreach (var t in columnName)
			{
				sum *= 26;
				sum += (t - 'A' + 1);
			}

			return sum;
		}
	}
}