using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InventoryFrontEnd.Models
{
    public class Campuses
    {
        public string Campus_ID { get; set; }
        public string Campus_name { get; set; }
    }
    public class Items
    {
        public string ID { get; set; }
        public string Item_name { get; set; }
        public string Model { get; set; }
        public string Brand { get; set; }
        public string Serial_number { get; set; }
        public string Item_type { get; set; }
        public string Room_name { get; set; }
        public string Item_status { get; set; }
        public string Department { get; set; }
        public string Date_now { get; set; }
        public string Campus_name { get; set; }
    }
    public class Brand
    {
        public string Brand_ID { get; set; }
        public string Brand_name { get; set; }
        public string Campus_name { get; set; }
    }
    public class Employee_Account
    {
        public string ID { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Campus_name { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }
    public class Rooms
    {
        public string ID { get; set; }
        public string Room_name { get; set; }
        public string Floor_level { get; set; }
        public string Room_description { get; set; }
        public string Campus_name { get; set; }
        public string Counter { get; set; }

    }
    public class Item_logs
    {
        public string Item_name { get; set; }
        public string Serial_number { get; set; }
        public string Date_transfer { get; set; }
        public string Last_location { get; set; }
        public string Current_location { get; set; }
        public string Transfer_by { get; set; }
        public string Requested_by { get; set; }
        public string Campus_name { get; set; }
        public string Process { get; set; }
        public string Model { get; set; }
        public string Brand { get; set; }
        public string Item_type { get; set; }
        public string Item_status { get; set; }
        public string Department { get; set; }
    }
    public class Apparel_stock
    {
        public string Apparel_ID { get; set; }
        public string Grade_level { get; set; }
        public string Apparel_type { get; set; }
        public string Size { get; set; }
        public string Quantity { get; set; }
    }
    public class Grades
    {
        public string ID { get; set; }
        public string Grade_level { get; set; }
        public string Campus_name { get; set; }
        
    }
    public class Apparel_type_list
    {
        public string Apparel_ID { get; set; }
        public string Apparel_type { get; set; }
        public string Campus_name { get; set; }

    }
    public class Apparel_Size
    {
        public string Size_ID { get; set; }
        public string Size { get; set; }
        public string Campus_name { get; set; }

    }
    public class Claim_stub
    {
        public string Claim_ID { get; set; }
        public string Size { get; set; }
        public string Quantity { get; set; }
        public string Grade_level { get; set; }
        public string Apparel_name { get; set; }
        public string First_name { get; set; }
        public string Last_name { get; set; }
        public string Date_recieve { get; set; }
        public string Reciept_number { get; set; }
        public string Campus_name { get; set; }
       
    }
    public class Appareltype_list
    {
        public string Apparel_ID { get; set; }
        public string Apparel_type { get; set; }
        public string Campus_name { get; set; }

    }
    public class Apparel_logs
    {
        public string ID { get; set; }
        public string Apparel_type { get; set; }
        public string Last_name { get; set; }
        public string First_name { get; set; }
        public string Grade_level { get; set; }
        public string Transaction { get; set; }
        public string Size { get; set; }
        public string Quantity { get; set; }
        public string Transaction_date { get; set; }
        public string Campus_name { get; set; }
        public string Reciept_number { get; set; }

    }
    public class Roomscounters
    {
        public string Counter { get; set; }

    }
    public class ITassetcounters
    {
        public string Counter { get; set; }

    }
    public class Employeecounters
    {
        public string Counter { get; set; }

    }
    public class logscounters
    {
        public string Counter { get; set; }

    }
    public class Brandcounter
    {
        public string Counter { get; set; }

    }
    public class Apparelavailablecounter
    {
        public string Counter { get; set; }

    }
    public class Gradecounter
    {
        public string Counter { get; set; }

    }
    public class Sizecounter
    {
        public string Counter { get; set; }

    }
    public class clainapparelcounter
    {
        public string Counter { get; set; }

    }
    public class appareltypecounter
    {
        public string Counter { get; set; }

    }
    public class apparelrecordcounter
    {
        public string Counter { get; set; }

    }

    public class Modelall
    {
        public IEnumerable<Items> Item_list { get; set; }
        public IEnumerable<Rooms> Room_list { get; set; }
        public IEnumerable<Brand> Allbrand { get; set; }
        public IEnumerable<Item_logs> Item_logs_data { get; set; }
        public IEnumerable<Campuses> Allcampus { get; set; }
        public IEnumerable<Employee_Account> Employees { get; set; }
        public IEnumerable<Apparel_stock> Allapparel_stock { get; set; }
        public IEnumerable<Grades> Allgrades { get; set; }
        public IEnumerable<Apparel_type_list> Allapparel_type { get; set; }
        public IEnumerable<Apparel_Size> Allsize { get; set; }
        public IEnumerable<Claim_stub> Allclaimitems { get; set; }
        public IEnumerable<Appareltype_list> Allappareltype { get; set; }
        public IEnumerable<Apparel_logs> Allapparellogs_data { get; set; }
        public IEnumerable<Roomscounters> Allroomcounters { get; set; }
        public IEnumerable<ITassetcounters> AllITassetcounters { get; set; }
        public IEnumerable<Employeecounters> Allemployeecounters { get; set; }
        public IEnumerable<logscounters> Alllogscounters { get; set; }
        public IEnumerable<Brandcounter> Allbrandcounters { get; set; }
        public IEnumerable<Apparelavailablecounter> AllApparelavailablecounters { get; set; }
        public IEnumerable<Gradecounter> AllGradecounters { get; set; }
        public IEnumerable<Sizecounter> AllSizecounters { get; set; }
        public IEnumerable<clainapparelcounter> Allclaimcounters { get; set; }
        public IEnumerable<appareltypecounter> Allappareltypecounters { get; set; }
        public IEnumerable<apparelrecordcounter> Allapparelrecordcounters { get; set; }
    }
}