using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Integration.AccessType
{
	public class FieldAccessEntity
	{
		private int id;
		[Audited]
		private string data;

		public virtual int Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual void SetData(string theData)
		{
			data = theData;
		}

		public override bool Equals(object obj)
		{
			var bte = obj as FieldAccessEntity;
			if (bte == null)
				return false;
			return (bte.Id == Id && string.Equals(bte.data, data));
		}

		public override int GetHashCode()
		{
			return Id ^ data.GetHashCode();
		}
	}

	public class PropertyAccessEntity
	{
		public virtual int Id { get; set; }
		[Audited]
		public virtual string Data { get; set; }
	}

	[Audited]
	public class Country
	{
		private readonly int _code;
		private readonly string _name;

		protected Country()
		{
		}

		public Country(int code, string name)
		{
			_code = code;
			_name = name;
		}

		public virtual int Code
		{
			get { return _code; }
		}

		public virtual string Name
		{
			get { return _name; }
		}

		public override bool Equals(object obj)
		{
			var casted = obj as Country;
			if (casted == null)
				return false;

			return casted.Code == Code && casted.Name.Equals(Name);
		}

		public override int GetHashCode()
		{
			return Code ^ Name.GetHashCode();
		}
	}

	public class MixedAccessEntity
	{
		private int id;
		private string data;

		protected MixedAccessEntity() { }

		public MixedAccessEntity(string data)
		{
			this.data = data;
		}

		public MixedAccessEntity(int id, string data)
		{
			this.id = id;
			this.data = data;
		}

		[Audited]
		protected virtual string Data
		{
			get
			{
				return data;
			}
			set
			{
				IsDataSet = true;
				data = value;
			}
		}

		public virtual bool IsDataSet { get; protected set; }

		public virtual void WriteData(string data)
		{
			IsDataSet = true;
			this.data = data;
		}

		public override bool Equals(object obj)
		{
			var casted = obj as MixedAccessEntity;
			if (casted == null)
				return false;

			return casted.id == id && casted.data == data;
		}

		public override int GetHashCode()
		{
			return id ^ data.GetHashCode();
		}
	}

	public class NoSetterEntity
	{
		private string data;

		protected NoSetterEntity()
		{
		}

		public NoSetterEntity(string data)
		{
			this.data = data;
		}

		public virtual int Id { get; set; }

		[Audited]
		public virtual string Data
		{
			get { return data; }
		}

		public virtual void WriteData(string data)
		{
			this.data = data;
		}

		public override bool Equals(object obj)
		{
			var casted = obj as NoSetterEntity;
			if (casted == null)
				return false;

			return casted.Id == Id && casted.Data.Equals(Data);
		}

		public override int GetHashCode()
		{
			return Id ^ Data.GetHashCode();
		}
	}
}