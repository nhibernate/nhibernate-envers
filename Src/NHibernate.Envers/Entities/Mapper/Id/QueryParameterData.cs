namespace NHibernate.Envers.Entities.Mapper.Id
{
	public class QueryParameterData
	{
		public QueryParameterData(string flatEntityPropertyName, object value)
		{
			QueryParameterName = flatEntityPropertyName;
			Value = value;
		}

		public string QueryParameterName { get; private set; }
		public object Value { get; private set; }

		public string GetProperty(string prefix)
		{
			if (prefix != null)
			{
				return prefix + "." + QueryParameterName;
			}
			return QueryParameterName;
		}

		public void SetParameterValue(IQuery query)
		{
			query.SetParameter(QueryParameterName, Value);
		}

		public override bool Equals(object obj)
		{
			if (this == obj) return true;
			var casted = obj as QueryParameterData;
			if (casted == null) return false;

			return QueryParameterName == null ? 
					casted.QueryParameterName == null : 
					QueryParameterName.Equals(casted.QueryParameterName);
		}

		public override int GetHashCode()
		{
			return (QueryParameterName != null ? QueryParameterName.GetHashCode() : 0);
		}
	}
}
