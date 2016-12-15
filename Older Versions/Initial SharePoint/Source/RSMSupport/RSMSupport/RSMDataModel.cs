using System;
using System.Configuration;
using System.Runtime.Serialization;
using RSM.Artifacts.Interfaces;
using RSM.Artifacts;

namespace RSM.Support
{
	partial class RSMDataModelDataContext
	{
		public RSMDataModelDataContext() :
			base(ConfigurationManager.ConnectionStrings[Constants.ConnectionStringName].ConnectionString, mappingSource)
		{
			OnCreated();
		}
		

		partial void OnCreated()
		{
			Connection.ConnectionString = ConfigurationManager.ConnectionStrings[Constants.ConnectionStringName].ConnectionString;
		}
	}

	public enum ExternalSystemDirection
	{
		None = 0,
		Incoming = 1,
		Outgoing = 2,
	}

	[Flags]
	public enum BatchOutcome
	{
		[EnumMember]
		Success = 2,

		[EnumMember]
		Warning = 4,

		[EnumMember]
		BusinessError = 8,

		[EnumMember]
		DataError = 16,

		[EnumMember]
		ValidationError = 32,

		[EnumMember]
		TechnicalError = 64,
	}

	public enum ReaderDirection
	{
		In = 0,
		Out = 1,
		Neutral = 2
	}

	public partial class BatchHistory : IEntity
	{
		public BatchOutcome BatchOutcome
		{
			get { return (BatchOutcome) Outcome; }
		}
	}

	public partial class Setting : IEntity
	{
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof (Setting)) return false;
			return Equals((Setting) obj);
		}

		public bool Equals(Setting other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return other._Id == _Id && other._Viewable.Equals(_Viewable) && Equals(other._InputType, _InputType) && other._SystemId == _SystemId && Equals(other._Name, _Name) && Equals(other._Label, _Label) && Equals(other._Value, _Value) && other._OrderBy == _OrderBy;
		}
	}

	public partial class ExternalSystem : IEntity
	{
		public string DirectionLabel
		{
			get
			{
				switch (Direction)
				{
					case (int)ExternalSystemDirection.Incoming:
						return "Incoming";

					case (int)ExternalSystemDirection.Outgoing:
						return "Outgoing";

					default:
						return "None";
				}
			}
			set {}
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof (ExternalSystem)) return false;
			return Equals((ExternalSystem) obj);
		}

		public bool Equals(ExternalSystem other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return other._Id == _Id && Equals(other._Name, _Name) && other._Direction.Equals(_Direction);
		}
	}

	public partial class Reader : IEntity
	{
		public string DirectionLabel
		{
			get
			{
				switch (Direction)
				{
					case (int)ReaderDirection.In:
						return "Incoming";

					case (int)ReaderDirection.Out:
						return "Outgoing";

					default:
						return "Neutral";
				}
			}
		}
 
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(Reader)) return false;
			return Equals((Reader)obj);
		}

		public bool Equals(Reader other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return other.Id == _Id && Equals(other.Name, _Name) && other._Direction.Equals(_Direction);
		}
	}

	public partial class Portal : IEntity
	{ }

	public partial class Location : IEntity
	{
		public int Id
		{
			get
			{
				return _LocationID;
			}
			set
			{
				_LocationID = value;
			}
		}
	}

	public partial class AccessHistory : IEntity
	{ }

	public partial class Person : IEntity
	{
		public int Id
		{
			get
			{
				return _PersonID;
			}
			set
			{
				_PersonID = value;
			}
		}
	}

}
