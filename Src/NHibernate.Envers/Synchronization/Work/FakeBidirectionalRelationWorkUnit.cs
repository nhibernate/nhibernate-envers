using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Engine;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Entities;
using Iesi.Collections.Generic;

namespace NHibernate.Envers.Synchronization.Work
{
    /**
     * A work unit that handles "fake" bidirectional one-to-many relations (mapped with {@code @OneToMany+@JoinColumn} and
     * {@code @ManyToOne+@Column(insertable=false, updatable=false)}.
     * @author Simon Duduica, port of Envers omonyme class by Adam Warski (adam at warski dot org)
     */
    public class FakeBidirectionalRelationWorkUnit: AbstractAuditWorkUnit, IAuditWorkUnit {
        private readonly IDictionary<String, FakeRelationChange> fakeRelationChanges;

        /*
         * The work unit responsible for generating the "raw" entity data to be saved.
         */
        private readonly IAuditWorkUnit nestedWorkUnit;

        public FakeBidirectionalRelationWorkUnit(ISessionImplementor sessionImplementor, String entityName,
                                                 AuditConfiguration verCfg, Object id,
                                                 String referencingPropertyName, Object owningEntity,
                                                 RelationDescription rd, RevisionType revisionType,
                                                 Object index,
                                                 IAuditWorkUnit nestedWorkUnit) 
            :base(sessionImplementor, entityName, verCfg, id)
        {
            this.nestedWorkUnit = nestedWorkUnit;

            // Adding the change for the relation.
            fakeRelationChanges = new Dictionary<String, FakeRelationChange>();
            fakeRelationChanges.Add(referencingPropertyName, new FakeRelationChange(owningEntity, rd, revisionType, index));
        }

        public FakeBidirectionalRelationWorkUnit(FakeBidirectionalRelationWorkUnit original,
                                                 IDictionary<String, FakeRelationChange> fakeRelationChanges,
                                                 IAuditWorkUnit nestedWorkUnit)
            : base(original.sessionImplementor, original.EntityName, original.verCfg, original.EntityId)
        {
            this.fakeRelationChanges = fakeRelationChanges;
            this.nestedWorkUnit = nestedWorkUnit;
        }

        public FakeBidirectionalRelationWorkUnit(FakeBidirectionalRelationWorkUnit original, IAuditWorkUnit nestedWorkUnit)
            : base(original.sessionImplementor, original.EntityName, original.verCfg, original.EntityId)
        {
            this.nestedWorkUnit = nestedWorkUnit;

            fakeRelationChanges = new Dictionary<String, FakeRelationChange>(original.getFakeRelationChanges());
        }

        public IAuditWorkUnit getNestedWorkUnit() {
            return nestedWorkUnit;
        }

        public IDictionary<String, FakeRelationChange> getFakeRelationChanges() {
            return fakeRelationChanges;
        }

        public override bool ContainsWork()
        {
            return true;
        }

        public override IDictionary<String, Object> GenerateData(Object revisionData)
        {
            // Generating data with the nested work unit. This data contains all data except the fake relation.
            // Making a defensive copy not to modify the data held by the nested work unit.
            IDictionary<String, Object> nestedData = new Dictionary<String, Object>(nestedWorkUnit.GenerateData(revisionData));

            // Now adding data for all fake relations.
            foreach (FakeRelationChange fakeRelationChange in fakeRelationChanges.Values) {
                fakeRelationChange.GenerateData(sessionImplementor, nestedData);
            }

            return nestedData;
        }

        public override IAuditWorkUnit Merge(AddWorkUnit second)
        {
            return merge(this, nestedWorkUnit, second);
        }

        public override IAuditWorkUnit Merge(ModWorkUnit second)
        {
            return merge(this, nestedWorkUnit, second);
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
            IAuditWorkUnit mergedNested = second.getNestedWorkUnit().Dispatch(nestedWorkUnit);

            // Now merging the fake relation changes from both work units.
            IDictionary<String, FakeRelationChange> secondFakeRelationChanges = second.getFakeRelationChanges();
            IDictionary<String, FakeRelationChange> mergedFakeRelationChanges = new Dictionary<String, FakeRelationChange>();
            //TODO Simon - decide if we use IESI.Collections in the end.
            ISet<String> allPropertyNames = new HashedSet<String>(fakeRelationChanges.Keys);
            allPropertyNames.AddAll(secondFakeRelationChanges.Keys);

            foreach (String propertyName in allPropertyNames) {
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

        public static IAuditWorkUnit merge(FakeBidirectionalRelationWorkUnit frwu, IAuditWorkUnit nestedFirst,
                                        IAuditWorkUnit nestedSecond) {
            IAuditWorkUnit nestedMerged = nestedSecond.Dispatch(nestedFirst);

            // Creating a new fake relation work unit with the nested merged data
            return new FakeBidirectionalRelationWorkUnit(frwu, nestedMerged);
        }

        /**
         * Describes a change to a single fake bidirectional relation.
         */
        public class FakeRelationChange {
            private readonly Object owningEntity;
            private readonly RelationDescription rd;
            public RevisionType RevisionType { get; private set; }
            private readonly Object index;

            public FakeRelationChange(Object owningEntity, RelationDescription rd, RevisionType revisionType,
                                      Object index) {
                this.owningEntity = owningEntity;
                this.rd = rd;
                this.RevisionType = revisionType;
                this.index = index;
            }

            public void GenerateData(ISessionImplementor sessionImplementor, IDictionary<String, Object> data)
            {
                // If the revision type is "DEL", it means that the object is removed from the collection. Then the
                // new owner will in fact be null.
                rd.FakeBidirectionalRelationMapper.MapToMapFromEntity(sessionImplementor, data,
                        RevisionType == RevisionType.DEL ? null : owningEntity, null);

                // Also mapping the index, if the collection is indexed.
                if (rd.FakeBidirectionalRelationIndexMapper != null) {
                    rd.FakeBidirectionalRelationIndexMapper.MapToMapFromEntity(sessionImplementor, data,
                            RevisionType == RevisionType.DEL ? null : index, null);
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
                if (first.RevisionType == RevisionType.DEL || second.RevisionType == RevisionType.ADD) {
                    return second;
                } else {
                    return first;
                }
            }
        }
    }
}
