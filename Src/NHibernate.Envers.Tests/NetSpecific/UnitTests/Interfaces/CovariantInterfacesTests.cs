using NHibernate.Envers.Query;
using NHibernate.Envers.Query.Impl;

namespace NHibernate.Envers.Tests.NetSpecific.UnitTests.Interfaces
{
	class CovariantInterfacesTests
	{
		private IRevisionEntityInfo<object, object> This_test_will_only_compile_if_the_typeparameter_of_the_IRevisionEntityInfo_interface_are_defined_as_covariant()
		{
			return new RevisionEntityInfo<CovariantToObject, CovariantToObject>(new CovariantToObject(), new CovariantToObject(), RevisionType.Added);
		}

		class CovariantToObject { }
	}
}
