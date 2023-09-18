using BlazorMonaco.Editor;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PanoramicData.JsonMagic.Web.Interfaces;
using PanoramicData.SheetMagic;

namespace PanoramicData.JsonMagic.Web.Pages;

public partial class Index
{
	private StandaloneCodeEditor? _editor;
	private string _sheetName = "JSON Data";
	private string _fileName = "JSON Data";

	[Inject]
	private IJSRuntime JS { get; set; } = null!;

	[Inject]
	private IToastService ToastService { get; set; } = null!;

	private static StandaloneEditorConstructionOptions EditorConstructionOptions(StandaloneCodeEditor _) => new()
	{
		AutomaticLayout = true,
		Language = "json",
		Value = """
		{
			"Woo": "Yay",
			"Houpla": null,
			"Thing": [
				{
					"A": "b",
					"C": 2
				},
				{
					"A": "x",
					"C": 200
				}
			]
		}
		"""
	};

	private async Task ButtonClickAsync()
	{
		if (_editor == null)
		{
			return;
		}

		var fileName = $"{TidyString(_fileName, "JSON Data", 30)} - {DateTime.UtcNow:yyyy-MM-ddTHHmmss}.xlsx";
		var sheetName = TidyString(_sheetName, "JSONData", 15);
		List<JObject> items = new();

		try
		{
			var @object = JsonConvert.DeserializeObject(await _editor.GetValue());
			if (@object is JArray jArray)
			{
				UpdateItems(items, jArray, new JObject());
			}
			else if (@object is JObject jObject)
			{
				var properties = jObject
						.Properties();
				var firstIterator = properties
						.FirstOrDefault(p => p?.Value is JArray);
				if (firstIterator?.Value is not JArray firstIteratorValueAsJArray)
				{
					ToastService.Info("Nothing to do");
					return;
				}

				// Find non-iterator properties and store them on a master object
				var commonPropertiesJobject = (JObject)jObject.DeepClone();
				UpdateItems(items, firstIteratorValueAsJArray, commonPropertiesJobject);
			}
			else
			{
				ToastService.Error("Unexpected JToken", $"Could not process {@object?.GetType()}");
			}
		}
		catch (Exception ex)
		{
			// TODO - something better
			var message = ex.ToString();
			ToastService.Error(ex.Message, $"Failed to determine list of objects");
		}

		await DownloadAsync(
			fileName,
			sheetName,
			items);
	}

	private async Task DownloadAsync(string fileName, string sheetName, List<JObject> items)
	{
		using var memoryStream = new MemoryStream();
		using var magicSpreadsheet = new MagicSpreadsheet(memoryStream, new Options { });
		try
		{
			var objectItems = items
				.Select(jObject => new Extended<object>(new(), jObject.ToObject<Dictionary<string, object?>>()))
				.ToList();
			magicSpreadsheet.AddSheet(objectItems, sheetName);
			magicSpreadsheet.Save();
		}
		catch (Exception ex)
		{
			// TODO - something better
			var message = ex.ToString();
			ToastService.Error($"Failed to save spreadsheet: {message}", ex.Message);
			return;
		}

		try
		{
			using var streamRef = new DotNetStreamReference(stream: memoryStream);
			await JS.InvokeVoidAsync("downloadFileFromStream", fileName, streamRef);
		}
		catch (Exception ex)
		{
			// TODO - something better
			ToastService.Error($"Failed to download spreadsheet", ex.Message);
		}
	}

	/// <summary>
	/// Returns a usable string, ensuring it has contents and truncating at a max length
	/// </summary>
	/// <param name="value"></param>
	/// <param name="valueIfNullOrWhiteSpace"></param>
	/// <returns></returns>
	private static string TidyString(
		string? value,
		string valueIfNullOrWhiteSpace,
		int maxLength)
		=> new((string.IsNullOrEmpty(value) ? valueIfNullOrWhiteSpace : value).Take(maxLength).ToArray());

	private static void UpdateItems(
		List<JObject> items,
		JArray jArray,
		JObject commonPropertiesJobject)
	{
		// Now iterate over all array objects and use them to update a clone of the common object
		foreach (var jProperty in jArray)
		{
			var thisJObject = JObject.FromObject(commonPropertiesJobject);
			foreach (var jProperty2 in jProperty)
			{
				thisJObject.Add(jProperty2);
			}

			items.Add(thisJObject);
		}
	}
}