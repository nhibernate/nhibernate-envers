using System;
using System.Collections.Generic;
using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Entities.ManyToAny
{
    [Audited]
    public class Purchase
    {
        public virtual Guid PurchaseId { get; set; }
        public virtual ISet<IPayment> Payments { get; set; }
    }
}