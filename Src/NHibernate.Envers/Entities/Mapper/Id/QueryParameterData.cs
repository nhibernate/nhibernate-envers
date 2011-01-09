using System;

namespace NHibernate.Envers.Entities.Mapper.Id
{
    /**
     * @author Adam Warski (adam at warski dot org)
     */
    public class QueryParameterData
    {
        private string flatEntityPropertyName;
        private object value;

        public QueryParameterData(String flatEntityPropertyName, Object value)
        {
            this.flatEntityPropertyName = flatEntityPropertyName;
            this.value = value;
        }

        public string getProperty(String prefix)
        {
            if (prefix != null)
            {
                return prefix + "." + flatEntityPropertyName;
            }
            return flatEntityPropertyName;
        }

        public object getValue()
        {
            return value;
        }

        public void SetParameterValue(IQuery query)
        {
            query.SetParameter(flatEntityPropertyName, value);
        }

        public string GetQueryParameterName()
        {
            return flatEntityPropertyName;
        }

        public override bool Equals(Object o) 
        {
            if (this == o) return true;
            if (!(o is QueryParameterData)) return false;

            QueryParameterData that = (QueryParameterData) o;

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
