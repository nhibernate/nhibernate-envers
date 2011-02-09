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

        public override bool Equals(object o) 
        {
            if (this == o) return true;
            if (!(o is QueryParameterData)) return false;

            var that = (QueryParameterData) o;

            if (flatEntityPropertyName != null ? !flatEntityPropertyName.Equals(that.flatEntityPropertyName) : that.flatEntityPropertyName != null)
                return false;

            return true;
        }

        public override int GetHashCode()
        {
            return (flatEntityPropertyName != null ? flatEntityPropertyName.GetHashCode() : 0);
        }
    }
}
