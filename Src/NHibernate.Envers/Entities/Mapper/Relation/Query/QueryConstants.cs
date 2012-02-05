namespace NHibernate.Envers.Entities.Mapper.Relation.Query
{
	public static class QueryConstants
	{
		public static readonly string ReferencedEntityAlias = "e__";
		public static readonly string ReferencedEntityAliasDefAudStr = "e2__";

		public static readonly string IndexEntityAlias = "f__";
		public static readonly string IndexEntityAliasDefAudStr = "f2__";

		public static readonly string MiddleEntityAlias = "ee__";
		public static readonly string MiddleEntityAliasDefAudStr = "ee2__";

		public static readonly string RevisionParameter = "revision";
		public static readonly string DelRevisionTypeParameter = "delrevisiontype";

		public static readonly string RevisionAlias = "r__";
	}
}