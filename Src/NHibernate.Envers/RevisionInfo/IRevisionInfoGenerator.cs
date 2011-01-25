namespace NHibernate.Envers.RevisionInfo
{
    public interface IRevisionInfoGenerator
    {
        void SaveRevisionData(ISession session, object revisionData);
        object Generate();
    }
}
