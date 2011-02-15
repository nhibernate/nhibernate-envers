namespace NHibernate.Envers.Entities.Mapper.Id
{
    public class QueryParameterData
    {
        private readonly string flatEntityPropertyName;
        private readonly object value;

        public QueryParameterData(string flatEntityPropertyName, object value)
        {
            this.flatEntityPropertyName = flatEntityPropertyName;
            this.value = value;
        }

        public string GetProperty(string prefix)
        {
            if (prefix != null)
            {
                return prefix + "." + flatEntityPropertyName;
            }
            return flatEntityPropertyName;
        }

    	public object Value
    	{
    		get { return value; }
    	}

    	public void SetParameterValue(IQuery query)
        {
            query.SetParameter(flatEntityPropertyName, value);
        }

        public string GetQueryParameterName()
        {
            return flatEntityPropertyName;
        }

        public override bool Equals(object obj) 
        {
            if (this == obj) return true;
        	var casted = obj as QueryParameterData;
            if (casted==null) return false;

            if (flatEntityPropertyName != null ? !flatEntityPropertyName.Equals(casted.flatEntityPropertyName) : casted.flatEntityPropertyName != null)
                return false;

            return true;
        }

        public override int GetHashCode()
        {
            return (flatEntityPropertyName != null ? flatEntityPropertyName.GetHashCode() : 0);
        }
    }
}
