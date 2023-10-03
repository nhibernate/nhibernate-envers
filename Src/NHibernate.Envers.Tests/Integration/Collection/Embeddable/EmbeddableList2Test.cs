using System.Collections.Generic;
using NHibernate.Envers.Tests.Entities.Collection;
using NHibernate.Envers.Tests.Entities.Components.Relations;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.Collection.Embeddable
{
	public partial class EmbeddableList2Test : TestBase
	{
		private int eleId1;
		private StrTestNoProxyEntity entity1;
		private StrTestNoProxyEntity entity2;
		private StrTestNoProxyEntity entity3;
		private StrTestNoProxyEntity entity4;
		private StrTestNoProxyEntity entity4Copy;

		public EmbeddableList2Test(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			entity1 = new StrTestNoProxyEntity {Str = "strTestEntity1"};
			entity2 = new StrTestNoProxyEntity {Str = "strTestEntity2"};
			entity3 = new StrTestNoProxyEntity {Str = "strTestEntity3"};
			entity4 = new StrTestNoProxyEntity {Str = "strTestEntity4"};
			var manyToOneComponent1 = new ManyToOneEagerComponent{Entity = entity1, Data = "dataComponent1"};
			var manyToOneComponent2 = new ManyToOneEagerComponent{Entity = entity2, Data = "dataComponent2"};
			var manyToOneComponent4 = new ManyToOneEagerComponent{Entity = entity4, Data = "dataComponent4"};

			var ele1 = new EmbeddableListEntity2();
			
			// Revision 1 (ele1: saving a list with 1 many-to-one component)
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(entity1);
				Session.Save(entity2);
				ele1.ComponentList.Add(manyToOneComponent1);
				eleId1 = (int) Session.Save(ele1);
				tx.Commit();
			}

			// Revision 2 (ele1: changing the component)
			using (var tx = Session.BeginTransaction())
			{
				ele1.ComponentList.Clear();
				ele1.ComponentList.Add(manyToOneComponent2);
				tx.Commit();
			}

			//Revision 3 (ele1: putting back the many-to-one component to the list)
			using (var tx = Session.BeginTransaction())
			{
				ele1.ComponentList.Add(manyToOneComponent1);
				tx.Commit();
			}

			// Revision 4 (ele1: changing the component's entity)
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(entity3);
				ele1.ComponentList[ele1.ComponentList.IndexOf(manyToOneComponent2)].Entity = entity3;
				ele1.ComponentList[ele1.ComponentList.IndexOf(manyToOneComponent2)].Data = "dataComponent3";
				tx.Commit();
			}

			// Revision 5 (ele1: adding a new many-to-one component)
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(entity4);
				entity4Copy = new StrTestNoProxyEntity {Id = entity4.Id, Str = entity4.Str};
				ele1.ComponentList.Add(manyToOneComponent4);
				tx.Commit();
			}

			// Revision 6 (ele1: changing the component's entity properties)
			using (var tx = Session.BeginTransaction())
			{
				ele1.ComponentList[ele1.ComponentList.IndexOf(manyToOneComponent4)].Entity.Str = "sat4";
				tx.Commit();
			}

			// Revision 7 (ele1: removing component)
			using (var tx = Session.BeginTransaction())
			{
				ele1.ComponentList.RemoveAt(ele1.ComponentList.IndexOf(manyToOneComponent4));
				tx.Commit();
			}

			// Revision 8 (ele1: removing all)
			using (var tx = Session.BeginTransaction())
			{
				Session.Delete(ele1);
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCounts()
		{
			AuditReader().GetRevisions(typeof (EmbeddableListEntity2), eleId1).Should().Have.SameSequenceAs(1, 2, 3, 4, 5, 7, 8);
			AuditReader().GetRevisions(typeof (StrTestNoProxyEntity), entity1.Id).Should().Have.SameSequenceAs(1);
			AuditReader().GetRevisions(typeof (StrTestNoProxyEntity), entity2.Id).Should().Have.SameSequenceAs(1);
			AuditReader().GetRevisions(typeof (StrTestNoProxyEntity), entity3.Id).Should().Have.SameSequenceAs(4);
			AuditReader().GetRevisions(typeof (StrTestNoProxyEntity), entity4.Id).Should().Have.SameSequenceAs(5, 6);
		}

		[Test]
		public void VerifyManyToOneComponentList()
		{
			// Revision 1: many-to-one component1 in the list
			var rev1 = AuditReader().Find<EmbeddableListEntity2>(eleId1, 1);
			rev1.Should().Not.Be.Null();
			rev1.ComponentList.Should().Not.Be.Empty();
			rev1.ComponentList[0].Data.Should().Be.EqualTo("dataComponent1");
			rev1.ComponentList[0].Entity.Should().Be.EqualTo(entity1);
		}

		[Test]
		public void VerifyHistoryOfEle1()
		{
			// Revision 1: many-to-one component in the list
			AuditReader().Find<EmbeddableListEntity2>(eleId1, 1).ComponentList
										.Should().Have.SameSequenceAs(new ManyToOneEagerComponent {Data = "dataComponent1", Entity = entity1});

			// Revision 2: many-to-one component in the list
			AuditReader().Find<EmbeddableListEntity2>(eleId1, 2).ComponentList
										.Should().Have.SameSequenceAs(new ManyToOneEagerComponent { Data = "dataComponent2", Entity = entity2 });

			// Revision 3: two many-to-one components in the list
			AuditReader().Find<EmbeddableListEntity2>(eleId1, 3).ComponentList
										.Should().Have.SameSequenceAs(new ManyToOneEagerComponent { Data = "dataComponent2", Entity = entity2 },
																									new ManyToOneEagerComponent{Data="dataComponent1", Entity = entity1});

			// Revision 4: second component edited and first one in the list
			AuditReader().Find<EmbeddableListEntity2>(eleId1, 4).ComponentList
										.Should().Have.SameSequenceAs(new ManyToOneEagerComponent { Data = "dataComponent3", Entity = entity3 },
																									new ManyToOneEagerComponent { Data = "dataComponent1", Entity = entity1 });

			// Revision 5: fourth component added in the list
			AuditReader().Find<EmbeddableListEntity2>(eleId1, 5).ComponentList
									.Should().Have.SameSequenceAs(new ManyToOneEagerComponent { Data = "dataComponent3", Entity = entity3 },
																						new ManyToOneEagerComponent { Data = "dataComponent1", Entity = entity1 },
																						new ManyToOneEagerComponent { Data = "dataComponent4", Entity = entity4Copy });

			// Revision 6: changing fourth component property
			AuditReader().Find<EmbeddableListEntity2>(eleId1, 6).ComponentList
									.Should().Have.SameSequenceAs(new ManyToOneEagerComponent { Data = "dataComponent3", Entity = entity3 },
																			new ManyToOneEagerComponent { Data = "dataComponent1", Entity = entity1 },
																			new ManyToOneEagerComponent { Data = "dataComponent4", Entity = entity4 });

			// Revision 7: removing component number four
			AuditReader().Find<EmbeddableListEntity2>(eleId1, 7).ComponentList
									.Should().Have.SameSequenceAs(new ManyToOneEagerComponent { Data = "dataComponent3", Entity = entity3 },
																								new ManyToOneEagerComponent { Data = "dataComponent1", Entity = entity1 });

			AuditReader().Find<EmbeddableListEntity2>(eleId1, 8).Should().Be.Null();
		}

		protected override IEnumerable<string> Mappings
		{
			get { return new[] { "Entities.Collection.Mapping.hbm.xml" }; }
		}
	}
}