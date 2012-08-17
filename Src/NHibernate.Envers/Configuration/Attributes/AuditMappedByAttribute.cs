using System;
using NHibernate.Envers.Configuration.Store;

namespace NHibernate.Envers.Configuration.Attributes
{
	/// <summary>
	/// Annotation to specify a "fake" bi-directional relation. Such a relation uses {@code @OneToMany} +
	/// {@code @JoinColumn} on the one side, and {@code @ManyToOne} + {@code @Column(insertable=false, updatable=false)} on
	/// the many side. Then, Envers won't use a join table to audit this relation, but will store changes as in a normal
	/// bi-directional relation.
	/// <remarks>
	/// In NHibernate.Envers this attribute is made internal. 
	/// It'll created in <see cref="MetaDataStore"/> if it's a biref collection with column insertable=false and updateable=false.
	/// </remarks>
	/// </summary>
	internal sealed class AuditMappedByAttribute:Attribute 
	{
		/// <summary>
		/// Name of the property in the related entity which maps back to this entity. The property should be
		/// mapped with {@code @ManyToOne} and {@code @Column(insertable=false, updatable=false)}.
		/// </summary>
		public string MappedBy { get; set; }

		/// <summary>
		/// Name of the property in the related entity which maps to the position column. Should be specified only
		/// for indexed collection, when @{@link org.hibernate.annotations.IndexColumn} is used on the collection.
		/// The property should be mapped with {@code @Column(insertable=false, updatable=false)}.
		/// </summary>
		public string PositionMappedBy { get; set; }

		public bool ForceInsertOverride { get; set; }
	}
}
