using Dapper;
using NikeAuto.Define;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Linq;

namespace NikeAuto.Utils
{
    public class SqliteDataAccess
    {
        private static string LoadConnectionString(string id = "Default")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }

        public static List<Task> LoadTask()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<Task>("select * from Task", new DynamicParameters());
                return output.ToList();
            }
        }

        public static List<Task> GetProxyCount_Task(int ProxyId)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<Task>("select * from Task WHERE proxy="+ ProxyId, new DynamicParameters());
                return output.ToList();
            }
        }

        public static List<Task> GetAccountCount_Task(int AccountId)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<Task>("select * from Task WHERE Account=" + AccountId, new DynamicParameters());
                return output.ToList();
            }
        }

        public static void SaveTask(Task task)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("insert into Task (sneaker, size, quantity, account, profile, shipping, proxy, status, mode) values (@Sneaker, @Size, @Quantity, @Account, @Profile, @Shipping, @Proxy, @Status, @Mode)", task);
            }
        }

        public static void DeleteAllTask()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("delete from Task");
            }
        }

        public static void DeleteTask(string deleteQuery)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute(deleteQuery);
            }
        }

        public static List<Account> LoadAccount()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<Account>("select * from Account", new DynamicParameters());
                return output.ToList();
            }
        }

        public static void SaveAccount(Account account)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("insert into Account (email, password) values (@Email, @Password)", account);
            }
        }

        public static void DeleteAllAccount()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("delete from Account");
            }
        }

        public static List<Proxy> LoadProxy()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<Proxy>("select * from Proxy", new DynamicParameters());
                return output.ToList();
            }
        }

        public static void SaveProxy(Proxy proxy)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("insert into Proxy (ip, port, userid, password) values (@Ip, @Port, @UserId, @Password)", proxy);
            }
        }

        public static void DeleteAllProxy()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("delete from Proxy");
            }
        }

        public static List<Profile> LoadProfile()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<Profile>("select * from Profile", new DynamicParameters());
                return output.ToList();
            }
        }

        public static List<Profile> LoadProfileData(int ProfileID)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<Profile>("select * from Profile where Id="+ ProfileID, new DynamicParameters());
                return output.ToList();
            }
        }

        public static void SaveProfile(Profile profile)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("insert into Profile (bfirstname, blastname, baddress, bapt, bzip, bcity, bstate, bphone, sfirstname, slastname, saddress, sapt, szip, scity, sstate, sphone, cardnumber, cardyear, cardmonth, cardsecurity) values (@BFirstName, @BLastName, @BAddress, @BAPT, @BZip, @BCity, @BState, @BPhone, @SFirstName, @SLastName, @SAddress, @SAPT, @SZip, @SCity, @SState, @SPhone, @CardNumber, @CardYear, @CardMonth, @CardSecurity)", profile);
            }
        }

        public static void UpdateProfile(Profile profile)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("UPDATE Profile SET bfirstname=@BFirstName, blastname= @BLastName, baddress=@BAddress, bapt=@BAPT, bzip=@BZip, bcity=@BCity, bstate=@BState, bphone=@BPhone, sfirstname=@SFirstName, slastname=@SLastName, saddress=@SAddress, sapt=@SAPT, szip=@SZip, scity=@SCity, sstate=@SState, sphone=@SPhone, cardnumber=@CardNumber, cardyear=@CardYear, cardmonth=@CardMonth, cardsecurity= @CardSecurity where Id=@Id", profile);
                ////////cnn.Execute("insert into Profile (bfirstname, blastname, baddress, bapt, bzip, bcity, bstate, bphone, sfirstname, slastname, saddress, sapt, szip, scity, sstate, sphone, cardnumber, cardyear, cardmonth, cardsecurity) values (@BFirstName, @BLastName, @BAddress, @BAPT, @BZip, @BCity, @BState, @BPhone, @SFirstName, @SLastName, @SAddress, @SAPT, @SZip, @SCity, @SState, @SPhone, @CardNumber, @CardYear, @CardMonth, @CardSecurity)", profile);
            }
        }

        public static void DeleteAllProfile()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("delete from Profile");
            }
        }

        public static void DeleteProfile(int ProfileID)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("delete from Profile where Id='" + ProfileID + "'");
            }
        }

        public static List<Product> LoadProduct(string upcomingDate)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                if(upcomingDate == null)
                {
                    var output = cnn.Query<Product>("select * from Product", new DynamicParameters());
                    return output.ToList();
                }
                else
                {
                    var output = cnn.Query<Product>("select * from Product where date = '" + upcomingDate + "'", new DynamicParameters());
                    return output.ToList();
                }
            }
        }

        public static List<Product> LoadProductSKU(int ProductID)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                ////////if (ProductID == null)
                ////////{
                ////////    var output = cnn.Query<Product>("select * from Product", new DynamicParameters());
                ////////    return output.ToList();
                ////////}
                ////////else
                ////////{
                ////////    var output = cnn.Query<Product>("select * from Product where date = '" + upcomingDate + "'", new DynamicParameters());
                ////////    return output.ToList();
                ////////}
                
                var output = cnn.Query<Product>("select * from Product where Id = '" + ProductID + "'", new DynamicParameters());
                return output.ToList();
            }
        }

        public static void SaveProduct(Product product)
        {
            string model = product.Model;
            string Color = product.Color;
            string SKU = product.SKU;
            int Price = product.Price;
            string Date = product.Date;
            string Time = product.Time;
            string ImageName = product.Image_Name;
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                ////////cnn.Execute("insert into Product (model, color, sku, price, date, time, image_name) values (@Model, @Color, @SKU, @Price, @Date, @Time, @ImageName)", product);
                
                cnn.Execute("insert into Product (model, color, sku, price, date, time, image_name) values ('"+ model + "', '" + Color + "', '" + SKU + "','" + Price + "', '" + Date + "', '" + Time + "', '" + ImageName + "')");
            }
        }
    }
}