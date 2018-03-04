namespace Service.Common.QueryTreats
{
    public enum Order
    {
        Ascending,
        Descending
    }

    public class OrderDescriptor
    {
        public Order Order { get; set; }
        public string PropertyName { get; set; }
    }
}
