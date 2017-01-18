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
            Console.Out.WriteLine("Hello");
            using (DbConnection con = new SqliteConnection("Data Source=|DataDirectory|db.sqlite"))
            {
                con.Open();
                var db = new Northwind(con);
                var query = db.Customers.Where(c => c.City == "London");
                Console.WriteLine($"Query:\n{query}\n");

                var list = query.ToList();
                foreach (var item in list)
                {
                    Console.WriteLine($"Name: {item.ContactName}");
                }
                Console.ReadLine();
            }
        }
    }

    public class Customers
    {
        public string CustomerID;
        public string ContactName;
        public string Phone;
        public string City;
        public string Country;
    }


    public class Orders
    {
        public int OrderID;
        public string CustomerID;
        public DateTime OrderDate;
    }


    public class Northwind
    {
        public Query<Customers> Customers;
        public Query<Orders> Orders;

        public Northwind(DbConnection connection)
        {
            QueryProvider provider = new DbQueryProvider(connection);
            Customers = new Query<Customers>(provider);
            Orders = new Query<Orders>(provider);
        }
    }
}