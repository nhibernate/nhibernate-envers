namespace NHibernate.Envers.Entities.Mapper.Id
{
	public class QueryParameterData
	{
		public QueryParameterData(string flatEntityPropertyName, object value)
		{
			QueryParameterName = flatEntityPropertyName;
			Value = value;
		}

		public string QueryParameterName { get; }
		public object Value { get; }

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

			return QueryParameterName?.Equals(casted.QueryParameterName) ?? casted.QueryParameterName == null;
		}

		public override int GetHashCode()
		{
			return QueryParameterName?.GetHashCode() ?? 0;
		}
	}
}
