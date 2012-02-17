using System;
using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Entities.ManyToAny
{
    [Audited]
    public class CreditCardPayment : IPayment
    {
        public virtual Guid CreditCardPaymentId { get; set; }
        public bool? IsSuccessful { get; set; }
        public decimal? Amount { get; set; }
        public string CardNumber { get; set; }
    }
}