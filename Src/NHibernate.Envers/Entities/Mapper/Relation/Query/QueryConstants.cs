namespace NHibernate.Envers.Entities.Mapper.Relation.Query
{
	public static class QueryConstants
	{
		public const string ReferencedEntityAlias = "e__";
		public const string ReferencedEntityAliasDefAudStr = "e2__";

		public const string IndexEntityAlias = "f__";
		public const string IndexEntityAliasDefAudStr = "f2__";

		public const string MiddleEntityAlias = "ee__";
		public const string MiddleEntityAliasDefAudStr = "ee2__";

		public const string RevisionParameter = "revision";
		public const string DelRevisionTypeParameter = "delrevisiontype";

		public const string RevisionAlias = "r__";
	}
}