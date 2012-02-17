namespace NHibernate.Envers.Tests.Entities.ManyToAny
{
    public interface IPayment
    {
        bool? IsSuccessful { get; set; }
        decimal? Amount { get; set; }
        string CardNumber { get; set; }
    }
}