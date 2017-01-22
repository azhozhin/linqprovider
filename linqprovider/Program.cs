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
                var query = db.Customers.Where(c => c.City == city)
                    .Select(c => new {Name = c.FirstName, Phone = c.Phone});
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
        public string Phone;
        public string City;
        public string Country;
    }


    public class Invoice
    {
        public long OrderId;
        public long CustomerId;
        public DateTime InvoiceDate;
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