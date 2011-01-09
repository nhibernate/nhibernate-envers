using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Envers
{
    [AttributeUsage(AttributeTargets.Property)]
    class RevisionNumberAttribute : Attribute
    {
    }
}
