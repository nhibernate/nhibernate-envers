namespace NHibernate.Envers
{
    public class DefaultAuditTableAttribute : AuditTableAttribute
    {
    	public DefaultAuditTableAttribute(string value) : base(value)
    	{
    	}

    	public System.Type AttributeType()
        {
            return typeof(DefaultAuditTableAttribute);
        }
    }
}
