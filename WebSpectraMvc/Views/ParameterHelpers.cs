using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Web;
using wcm.wspectra.decoders;
using wcm.wspectra.metadata;

namespace WebSpectraMvc.Views
{
	public static class ParameterHelpers
	{
		public static string GetDisplayName(this IParameter aParameter)
		{
			string lDisplayName = string.Empty;
			if (aParameter.TryGetMetadata(MetadataIDs.DISPLAY_NAME, out lDisplayName))
			{
				return lDisplayName;
			}
			return aParameter.Name;
		}

		public static string GetEnumValueDescription(this object aValue)
		{
			string lDescription = string.Empty;
			try
			{
				lDescription = aValue.ToString();
				var lText = aValue.ToString();
				FieldInfo lFi = aValue.GetType().GetField(aValue.ToString());
				if (lFi != null)
				{
					DescriptionAttribute[] lAttributes = (DescriptionAttribute[])lFi.GetCustomAttributes(typeof(DescriptionAttribute), false);
					if (lAttributes != null && lAttributes.Length > 0)
					{
						lDescription = lAttributes[0].Description;
					}
				}
			}
			catch { }
			return lDescription;
		}

		public static bool HasValidValues(this IParameter aParameter)
		{
			object lValidValuesMetadata;
			return aParameter.TryGetMetadataRaw(MetadataIDs.VALID_VALUES, out lValidValuesMetadata) && lValidValuesMetadata is IEnumerable;
		}

		public static bool HasValidNamedValues(this IParameter aParameter)
		{
			object lValidNamedValuesMetadata;
			return aParameter.TryGetMetadataRaw(MetadataIDs.VALID_NAMED_VALUES, out lValidNamedValuesMetadata) && lValidNamedValuesMetadata is IEnumerable;
		}

		public static IEnumerable<object> GetValidValues(this IParameter aParameter)
		{
			object lValidValuesMetadata;
			if (!aParameter.TryGetMetadataRaw(MetadataIDs.VALID_VALUES, out lValidValuesMetadata) || !(lValidValuesMetadata is IEnumerable))
				yield break;

			var lValidValues = (IEnumerable)lValidValuesMetadata;
			foreach (var lValidValue in lValidValues)
			{
				yield return lValidValue;
			}
		}

		public static IEnumerable<KeyValuePair<object, object>> GetValidNamedValues(this IParameter aParameter)
		{
			object lValidNamedValuesMetadata;
			if (!aParameter.TryGetMetadataRaw(MetadataIDs.VALID_NAMED_VALUES, out lValidNamedValuesMetadata) || !(lValidNamedValuesMetadata is IEnumerable))
				yield break;

			var lValidNamedValues = (IEnumerable)lValidNamedValuesMetadata;
			foreach (var lValidNamedValue in lValidNamedValues)
			{
				var lType = lValidNamedValue.GetType();
				var lKeyProp = lType.GetProperty("Key");
				var lValueProp = lType.GetProperty("Value");
				var lKey = lKeyProp.GetValue(lValidNamedValue, null);
				var lValue = lValueProp.GetValue(lValidNamedValue, null);
				yield return new KeyValuePair<object, object>(lKey, lValue);
			}
		}
	}
}