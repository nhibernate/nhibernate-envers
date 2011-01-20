namespace NHibernate.Envers.Query.Criteria
{
    public interface IExtendableCriterion : IAuditCriterion
    {
        IExtendableCriterion Add(IAuditCriterion criterion);
    }
}
