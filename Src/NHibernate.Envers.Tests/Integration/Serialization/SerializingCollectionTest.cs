using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using NHibernate.Envers.Tests.Entities.OneToMany;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.Serialization
{
	public partial class SerializingCollectionTest : TestBase
	{
		private const int ed1_id = 15;
		private const int ing1_id =21;

		public SerializingCollectionTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override IEnumerable<string> Mappings
		{
			get
			{
				return new[] { "Entities.OneToMany.Mapping.hbm.xml", "Entities.Mapping.hbm.xml" };
			}
		}

		protected override void Initialize()
		{
			var ed1 = new CollectionRefEdEntity {Id = ed1_id, Data = "data_ed_1"};
			var ing1 = new CollectionRefIngEntity {Id = ing1_id, Data = "data_ing_1", Reference = ed1};
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(ed1);
				Session.Save(ing1);
				tx.Commit();
			}
		}

		[Test]
		public void VerifyDetached()
		{
			var ing1 = Session.Get<CollectionRefIngEntity>(ing1_id);
			var rev1 = AuditReader().Find<CollectionRefEdEntity>(ed1_id, 1);

			// First forcing loading of the collection
			Assert.AreEqual(1, rev1.Reffering.Count);

			// Now serializing and de-serializing the object
			rev1 = serializeObject(rev1);

			// And checking the collection again
			CollectionAssert.AreEqual(new[]{ing1}, rev1.Reffering);
		}

		private static T serializeObject<T>(T objectToSerialize)
		{
			using (var memStr = new MemoryStream())
			{
				var bf = new BinaryFormatter();
				bf.Serialize(memStr, objectToSerialize);
				memStr.Position = 0;
				return (T)bf.Deserialize(memStr);
			}
		} 
	}
}