using System;
using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Entities.ManyToAny
{
    [Audited]
    public class WirePayment : IPayment
    {
        public virtual Guid WirePaymentId { get; set; }
        public virtual bool? IsSuccessful { get; set; }
        public virtual decimal? Amount { get; set; }
        public virtual string CardNumber { get; set; }
    }
}