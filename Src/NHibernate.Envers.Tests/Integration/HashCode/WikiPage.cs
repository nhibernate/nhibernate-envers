using Iesi.Collections.Generic;

namespace NHibernate.Envers.Tests.Integration.HashCode
{
	[Audited]
	public class WikiPage
	{
		public WikiPage()
		{
			Images = new HashedSet<WikiImage>();
		}
		public virtual int Id { get; set; }
		public virtual string Title { get; set; }
		public virtual string Content { get; set; }
		public virtual ISet<WikiImage> Images { get; set; }


		public override bool Equals(object obj)
		{
			var other = obj as WikiPage;
			if (other == null)
				return false;
			if (Content != null ? !Content.Equals(other.Content) : other.Content != null) return false;
			if (Title != null ? !Title.Equals(other.Title) : other.Title != null) return false;

			return true;
		}

		public override int GetHashCode()
		{
			var result = Title != null ? Title.GetHashCode() : 0;
			result = 31 * result + (Content != null ? Content.GetHashCode() : 0);
			return result;
		}
	}
}