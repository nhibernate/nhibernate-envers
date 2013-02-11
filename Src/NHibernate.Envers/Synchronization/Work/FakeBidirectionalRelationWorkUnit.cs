using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Entities;

namespace NHibernate.Envers.Synchronization.Work
{
	/// <summary>
	/// A work unit that handles "fake" bidirectional one-to-many relations (mapped with {@code @OneToMany+@JoinColumn} and
	/// {@code @ManyToOne+@Column(insertable=false, updatable=false)}.
	/// </summary>
	public class FakeBidirectionalRelationWorkUnit: AbstractAuditWorkUnit
	{
		private readonly IDictionary<string, FakeRelationChange> fakeRelationChanges;

		/// <summary>
		/// The work unit responsible for generating the "raw" entity data to be saved.
		/// </summary>
		/// <param name="sessionImplementor"></param>
		/// <param name="entityName"></param>
		/// <param name="verCfg"></param>
		/// <param name="id"></param>
		/// <param name="referencingPropertyName"></param>
		/// <param name="owningEntity"></param>
		/// <param name="rd"></param>
		/// <param name="revisionType"></param>
		/// <param name="index"></param>
		/// <param name="nestedWorkUnit"></param>
		public FakeBidirectionalRelationWorkUnit(ISessionImplementor sessionImplementor, string entityName,
												 AuditConfiguration verCfg, object id,
												 string referencingPropertyName, object owningEntity,
												 RelationDescription rd, RevisionType revisionType,
												 object index,
												 IAuditWorkUnit nestedWorkUnit) 
			:base(sessionImplementor, entityName, verCfg, id, revisionType)
		{
			NestedWorkUnit = nestedWorkUnit;

			// Adding the change for the relation.
			fakeRelationChanges = new Dictionary<string, FakeRelationChange>
									{
										{
											referencingPropertyName,
											new FakeRelationChange(owningEntity, rd, revisionType, index)
										}
									};
		}

		private FakeBidirectionalRelationWorkUnit(FakeBidirectionalRelationWorkUnit original,
												 IDictionary<string, FakeRelationChange> fakeRelationChanges,
												 IAuditWorkUnit nestedWorkUnit)
			: base(original.SessionImplementor, original.EntityName, original.VerCfg, original.EntityId, original.RevisionType)
		{
			this.fakeRelationChanges = fakeRelationChanges;
			NestedWorkUnit = nestedWorkUnit;
		}

		private FakeBidirectionalRelationWorkUnit(FakeBidirectionalRelationWorkUnit original, IAuditWorkUnit nestedWorkUnit)
			: base(original.SessionImplementor, original.EntityName, original.VerCfg, original.EntityId, original.RevisionType)
		{
			NestedWorkUnit = nestedWorkUnit;
			fakeRelationChanges = new Dictionary<string, FakeRelationChange>(original.fakeRelationChanges);
		}

		public IAuditWorkUnit NestedWorkUnit { get; private set; }

		public override bool ContainsWork()
		{
			return true;
		}

		public override IDictionary<string, object> GenerateData(object revisionData)
		{
			// Generating data with the nested work unit. This data contains all data except the fake relation.
			// Making a defensive copy not to modify the data held by the nested work unit.
			var nestedData = new Dictionary<string, object>(NestedWorkUnit.GenerateData(revisionData));

			// Now adding data for all fake relations.
			foreach (var fakeRelationChange in fakeRelationChanges.Values) 
			{
				fakeRelationChange.GenerateData(SessionImplementor, nestedData);
			}

			return nestedData;
		}

		public override IAuditWorkUnit Merge(AddWorkUnit second)
		{
			return Merge(this, NestedWorkUnit, second);
		}

		public override IAuditWorkUnit Merge(ModWorkUnit second)
		{
			return Merge(this, NestedWorkUnit, second);
		}

		public override IAuditWorkUnit Merge(DelWorkUnit second)
		{
			return second;
		}

		public override IAuditWorkUnit Merge(CollectionChangeWorkUnit second)
		{
			return this;
		}

		public override IAuditWorkUnit Merge(FakeBidirectionalRelationWorkUnit second)
		{
			// First merging the nested work units.
			var mergedNested = second.NestedWorkUnit.Dispatch(NestedWorkUnit);

			// Now merging the fake relation changes from both work units.
			var secondFakeRelationChanges = second.fakeRelationChanges;
			var mergedFakeRelationChanges = new Dictionary<string, FakeRelationChange>();
			var allPropertyNames = new HashSet<string>(fakeRelationChanges.Keys);
			allPropertyNames.UnionWith(secondFakeRelationChanges.Keys);
			
			foreach (var propertyName in allPropertyNames) 
			{
				mergedFakeRelationChanges.Add(propertyName,
						FakeRelationChange.Merge(
								fakeRelationChanges[propertyName],
								secondFakeRelationChanges[propertyName]));
			}

			return new FakeBidirectionalRelationWorkUnit(this, mergedFakeRelationChanges, mergedNested);
		}

		public override IAuditWorkUnit Dispatch(IWorkUnitMergeVisitor first)
		{
			return first.Merge(this);
		}

		public static IAuditWorkUnit Merge(FakeBidirectionalRelationWorkUnit frwu, IAuditWorkUnit nestedFirst,
										IAuditWorkUnit nestedSecond) 
		{
			var nestedMerged = nestedSecond.Dispatch(nestedFirst);

			// Creating a new fake relation work unit with the nested merged data
			return new FakeBidirectionalRelationWorkUnit(frwu, nestedMerged);
		}

		/// <summary>
		/// Describes a change to a single fake bidirectional relation.
		/// </summary>
		private class FakeRelationChange 
		{
			private readonly object owningEntity;
			private readonly RelationDescription rd;
			private readonly object index;
			private readonly RevisionType revisionType;

			public FakeRelationChange(object owningEntity, RelationDescription rd, RevisionType revisionType,
									  object index)
			{
				this.owningEntity = owningEntity;
				this.rd = rd;
				this.revisionType = revisionType;
				this.index = index;
			}

			public void GenerateData(ISessionImplementor sessionImplementor, IDictionary<string, object> data)
			{
				// If the revision type is "DEL", it means that the object is removed from the collection. Then the
				// new owner will in fact be null.
				rd.FakeBidirectionalRelationMapper.MapToMapFromEntity(sessionImplementor, data,
						revisionType == RevisionType.Deleted ? null : owningEntity, null);
				rd.FakeBidirectionalRelationMapper.MapModifiedFlagsToMapFromEntity(sessionImplementor, 
																										data,
				                                                                  revisionType == RevisionType.Deleted ? null : owningEntity, 
																										null);

				// Also mapping the index, if the collection is indexed.
				if (rd.FakeBidirectionalRelationIndexMapper != null) {
					rd.FakeBidirectionalRelationIndexMapper.MapToMapFromEntity(sessionImplementor, data,
							revisionType == RevisionType.Deleted ? null : index, null);
					rd.FakeBidirectionalRelationIndexMapper.MapModifiedFlagsToMapFromEntity(sessionImplementor, 
																													data,
					                                                                        revisionType == RevisionType.Deleted ? null : index, 
																													null);
				}
			}

			public static FakeRelationChange Merge(FakeRelationChange first, FakeRelationChange second)
			{
				if (first == null) { return second; }
				if (second == null) { return first; }

				/*
				 * The merging rules are the following (revision types of the first and second changes):
				 * - DEL, DEL - return any (the work units are the same)
				 * - DEL, ADD - return ADD (points to new owner)
				 * - ADD, DEL - return ADD (points to new owner)
				 * - ADD, ADD - return second (points to newer owner)
				 */
				if (first.revisionType == RevisionType.Deleted || second.revisionType == RevisionType.Added) 
				{
					return second;
				}
				return first;
			}
		}
	}
}
