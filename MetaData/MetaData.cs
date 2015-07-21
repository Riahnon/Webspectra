using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace wcm.wspectra.metadata
{
	/// <summary>
	/// Sometimes types cannot be defined at compilation time because they are generated at runtime. For example in a server-client scenario in which
	/// client receives a list of parameters that can be set in the server. This parameters may behave in a certain way and have some metaproperties 
	/// (maximum values, default values, valid values)
	/// MetaDataIDs are a way to represent that information that describes how a runtime generated type behaves.
	/// </summary>
	public class MetadataID
	{
		public readonly string Name;
		public MetadataID(string aName) { this.Name = aName; }
		public override string ToString() { return this.Name; }
		public static implicit operator string(MetadataID aMetadata) { return aMetadata.Name; }
		public static implicit operator MetadataID(string aId) { return new MetadataID(aId); }
	}

	public sealed class MetadataID<T> : MetadataID
	{
		public MetadataID(string aName) : base(aName) { }
		public MetadataID(MetadataID aMD) : base(aMD.Name) { }
		public static implicit operator MetadataID<T>(string aId) { return new MetadataID<T>(aId); }
	}

	public struct DependencyMetaData
	{
		public readonly string ParameterName;
		public readonly object ParameterValue;
		public DependencyMetaData(string aParameterName, object aParameterValue) { ParameterName = aParameterName; ParameterValue = aParameterValue; }
	}

	public class MetadataIDs
	{
		public static readonly MetadataID DEFAULT_VALUE = new MetadataID("DefaultValue");
		public static readonly MetadataID VALID_VALUES = new MetadataID("ValidValues");
		public static readonly MetadataID VALID_NAMED_VALUES = new MetadataID("ValidNamedValues");
		public static readonly MetadataID MAX_VALUE = new MetadataID("MaxValue");
		public static readonly MetadataID MIN_VALUE = new MetadataID("MinValue");
		public static readonly MetadataID<bool> READ_ONLY = new MetadataID<bool>("ReadOnly");
		public static readonly MetadataID<string> DISPLAY_NAME = new MetadataID<string>("DisplayName");
		public static readonly MetadataID<DependencyMetaData> DEPENDENCY = new MetadataID<DependencyMetaData>("Dependency");
		public static readonly MetadataID<bool> IS_FILEPATH = new MetadataID<bool>("IsFilePath");
	}

	public static class MetadataIDs<T>
	{
		public static readonly MetadataID<T> DEFAULT_VALUE = new MetadataID<T>(MetadataIDs.DEFAULT_VALUE);
		public static readonly MetadataID<IEnumerable<T>> VALID_VALUES = new MetadataID<IEnumerable<T>>(MetadataIDs.VALID_VALUES);
		public static readonly MetadataID<IEnumerable<KeyValuePair<string, T>>> VALID_NAMED_VALUES = new MetadataID<IEnumerable<KeyValuePair<string, T>>>(MetadataIDs.VALID_NAMED_VALUES);
		public static readonly MetadataID<T> MAX_VALUE = new MetadataID<T>(MetadataIDs.MAX_VALUE);
		public static readonly MetadataID<T> MIN_VALUE = new MetadataID<T>(MetadataIDs.MIN_VALUE);
	}

	public interface IMetadataHolder
	{
		IEnumerable<KeyValuePair<string, object>> MetadatasRaw { get; }

		bool HasMetadataRaw(MetadataID aId);
		bool TryGetMetadataRaw(MetadataID aId, out object MetaData);
		object GetMetaDataRaw(MetadataID aId);
		void SetMetadataRaw(MetadataID aId, object aMetaData);
		void RemoveMetadataRaw(MetadataID aId);

		bool HasMetadata<T>(MetadataID<T> aId);
		bool TryGetMetadata<T>(MetadataID<T> aId, out T aMetaData);
		T GetMetadata<T>(MetadataID<T> aId);
		void SetMetadata<T>(MetadataID<T> aId, T aMetaData);
		void RemoveMetadata<T>(MetadataID<T> aId);
	}
}
