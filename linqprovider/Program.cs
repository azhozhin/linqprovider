using System;
using System.Data.Common;
using System.Linq;
using Mono.Data.Sqlite;

namespace linqprovider
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            using (DbConnection con = new SqliteConnection("Data Source=|DataDirectory|Chinook_Sqlite.sqlite"))
            {
                con.Open();
                var db = new Chinook(con);
                var city = "London";
                var query = db.Customers.Select(c => new
                    {
                        Name = c.FirstName,
                        Location = new {City = c.City, Country = c.Country}
                    })
                    .Where(c => c.Location.City == city);
                Console.WriteLine(query.Expression.ToString());
                Console.WriteLine($"Query:\n{query}\n");

                var list = query.ToList();
                foreach (var item in list)
                {
                    Console.WriteLine($"{item}");
                }
            }
        }
    }

    public class Customer
    {
        public long CustomerId;
        public string FirstName;
        public string LastName;
        public string Company;
        public string Address;
        public string City;
        public string State;
        public string Country;
        public string PostalCode;
        public string Phone;
        public string Fax;
        public string Email;
        public long SupportRepId;
    }

    public class Invoice
    {
        public long InvoiceId;
        public long CustomerId;
        public DateTime InvoiceDate;
        public string BillingAddress;
        public string BillingCity;
        public string BillingState;
        public string BillingCountry;
        public string BillingPostalCode;
        public decimal Total;
    }

    public class Chinook
    {
        public Query<Customer> Customers;
        public Query<Invoice> Invoices;

        public Chinook(DbConnection connection)
        {
            QueryProvider provider = new DbQueryProvider(connection);
            Customers = new Query<Customer>(provider);
            Invoices = new Query<Invoice>(provider);
        }
    }

    public class X{
        public String Name;
        public String Phone;
    }
}