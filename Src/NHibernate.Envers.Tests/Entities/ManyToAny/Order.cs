using System;
using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Entities.ManyToAny
{
    [Audited]
    public class Order
    {
        public virtual Guid OrderId { get; set; }
        public virtual IPayment Payment { get; set; }
    }
}