using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Envers
{
    [AttributeUsage(AttributeTargets.Property)]
    class RevisionEntityAttribute : Attribute
    {
        //TODO Simon - find out what it needs here
        //   ORIG: Class<? extends RevisionListener> value() default RevisionListener.class;
        public System.Type value = typeof(EventHandler);
    }
}
