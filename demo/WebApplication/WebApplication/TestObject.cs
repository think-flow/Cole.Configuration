#nullable disable
namespace Model;

public class Address
{
    public string street { get; set; }
    public string city { get; set; }
    public string state { get; set; }
}

public class Receipt
{
    public string receipt { get; set; }
    public DateTime Date { get; set; }
    public Customer customer { get; set; }
    public Item[] items { get; set; }
    public Address bill_to { get; set; }
    public Address ship_to { get; set; }
    public string specialDelivery { get; set; }
}

public class Customer
{
    public string given { get; set; }
    public string family { get; set; }
}

public class Item
{
    public string part_no { get; set; }
    public string descrip { get; set; }
    public decimal price { get; set; }
    public int quantity { get; set; }
}