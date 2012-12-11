using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Envers.Event;

namespace NHibernate.Envers.Configuration.Metadata
{
	[Serializable]
	public class DefaultEnversPostInstantiationListener : IPostInstantiationListener
	{
		/// <summary>
		/// noop
		/// </summary>
		public void PostInstantiate(object entity)
		{
		}
	}
}