namespace API.Models
{
    public class Orders
    {
        public int OrderId { get; set; }
        public string? CustomerId { get; set; }
        public string? Employeeid { get; set; }
        public DateTime Orderdate { get; set; }
        public DateTime Requireddate { get; set; }
        public string? Shippeddate { get; set; }
        public int Shipvia { get; set; }
        public decimal Freight { get; set; }
        public string? ShipName { get; set; }
        public string? ShipAddress { get; set; }
        public string? ShipCity { get; set; }
        public string? ShipRegion { get; set; }
        public string? ShipPostalCode { get; set; }
        public string? ShipCountry { get; set; }
    }
}
