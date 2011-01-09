using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Spring.Objects.Factory;

namespace Envers.Net.Model.Util
{
        public class ExecutingAssemblyFactoryObject : IFactoryObject 
        {
            #region IFactoryObject Members

            public object GetObject()
            {
 	            return Assembly.GetExecutingAssembly();
            }

            public bool IsSingleton
            {
	            get{return true;}
            }

            public Type ObjectType
            {
                get { return typeof(Assembly); }
            }

            #endregion
    }
}
