
using MySql.Data;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Relational;
using System;
using System.Diagnostics;
using System.Net.Sockets;

namespace whois
{
    internal class Program
    {
        public static bool debug = true;
        //Password for SQL: L3tM31n    
        //REMEMBER TO ENTER COMMANDS AS whois "test1 test2"
        //Without "" the commands wont work properly. Spent 2 hours trying to fix nothing.

        static void Main(string[] args)
        {
            //Declare Variables
            string LoginID = "";
            string[] values = { };
            bool viewalldata = true;
            bool lookupField = false;
            bool editdata = false;
            string column = "";
            string command = "";

            //Read in string from CMD

               // RunServer();
            
            if(args.Length > 0)
            {
                command = args[0];
            }
            //Call function for Add Records to Database
            if (command.ToUpper() == "ADD")
            {
                AddRecord();
            }
            //Call function for Delete Records from Database
            else if(command.ToUpper() == "DELETE")
            {
                Console.WriteLine("Enter LoginID of Field You Wish To Delete: ");
                string Login = Console.ReadLine();
                DeleteRecord(Login);
            }

            //Process string data, to decide whether user wants to search all data, edit data field, or lookup a specific user field.
            //User can search multiple users, or multiple fields at once, however can only edit one record at a time.

            for (int i = 0; i < command.Length; i++)
            {               

                if (command[i] == '=')
                {
                    lookupField = false;
                    viewalldata = false;
                    editdata = true;
                }
                else if (command[i] == '?')
                {
                    lookupField = true;
                    viewalldata = false;
                    editdata = false;
                }
            }

            //Split string to decide which data refers to which field.
            values = command.Split('?', ' ', '=') ;

            //Calls function to edit records if requirements are met.

                if (editdata == true)
                {
                    LoginID = values[0];
                    column = values[1];
                    string update = values[2];
                EditRecord(LoginID, column, update);
                }

                //Calls function to view all data given a loginID
            for (int i = 0; i < values.Length; i++)
            {
                if (viewalldata == true)
                {
                    LoginID = values[i];
                    ViewData(LoginID, "*");
                }
            }

            //calls function to view a single field, given a loginID and field is specified
            if(lookupField == true)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    if (i % 2 == 0)
                    {
                        LoginID = values[i];
                    }
                    else if(i % 2 == 1)
                    {
                        column = values[i];
                        ViewData(LoginID,column);
                    }

                    
                }
            }




        }

        //Function to add records
        private static void AddRecord()
        {
            string? LoginID = "!";
            string? userID = "!";
            string? Surname = "";
            string? Forenames = "";
            string? Title = "";
            string? Position = "";
            string? Phone = "";
            string? Email = "";
            string? Location = "";
            int? loginid = 0;
            int? locationid = 0;
            int? emailid = 0;
            int? positionid = 0;
            int? usernum = 0;

            Console.WriteLine("Enter Record Details Below to Add to Database: ");

            //Handles limitations and error in input data. LoginID and userID should have no special characters.
            try
            {
                while (LoginID.Any(ch => !char.IsLetterOrDigit(ch)) || LoginID == null)
                {
                    Console.WriteLine("Enter LoginID: ");
                    LoginID = Console.ReadLine();
                }
                while (userID.Any(ch => !char.IsLetterOrDigit(ch)) || userID == null || userID.Length == 0)
                {
                    Console.WriteLine("Enter userID: ");
                    userID = Console.ReadLine();
                }
                Console.WriteLine("Enter Surname (Leave Blank For No Surname): ");
                Surname = Console.ReadLine();
                Console.WriteLine("Enter Forenames");
                Forenames = Console.ReadLine();
                Console.WriteLine("Enter Title: ");
                Title = Console.ReadLine();
                Console.WriteLine("Enter Position (Add Comma Between For Multiple Titles): ");
                Position = Console.ReadLine();
                Console.WriteLine("Enter Phone: ");
                Phone = Console.ReadLine();
                Console.WriteLine("Enter Email: ");
                Email = Console.ReadLine();
                Console.WriteLine("Enter Location: ");
                Location = Console.ReadLine();
                Console.WriteLine("Enter Location ID:");
                locationid = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine("Enter Email ID:");
                emailid= Convert.ToInt32(Console.ReadLine());
                Console.WriteLine("Enter Position ID:");
                positionid = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine("Enter Login ID:");
                loginid = Convert.ToInt32(Console.ReadLine());
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine("Incorrect data type. Must be string.");
            }

            usernum = Convert.ToInt32(userID);

            //SQL script which is run in SQL to give instructions to the database.

            string AddRecord = ("(INSERT INTO `phone number` (`Phone Number`) VALUES('" + Phone + "');" + 
                "\r\nINSERT INTO userid (userid, forename, surname, title, `phone number_phone number`, location_locationid)\r\nVALUES('"
                + usernum + "', '" + Forenames + "', '" + Surname + "', '" + Title + "', '" + Phone + "', '" + locationid + "');" +
                "\r\nINSERT INTO email (EmailID, Email) VALUES ('" + emailid +
                "', '" + Email + "');" +
                "\r\nINSERT INTO location (LocationID, Location) VALUES ('" + locationid
                + "', '" + Location + "');" +
                "\r\nINSERT INTO LoginID (LoginID, `Login Number`) VALUES ('" + loginid + "', '" + LoginID + "');"
                 + "\r\nINSERT INTO positions (Position, UserID_UserID) VALUES ('" + Position + "', '" + userID + "');"
                 + "\r\nINSERT INTO userid_has_email(UserID_UserID, Email_EmailID) VALUES ('" + userID + "', '" + emailid + "');" +
                 "\r\nINSERT INTO userid_has_loginid (UserID_UserID, LoginID_LoginID) VALUES ('" + userID + "', '" + loginid + "');");

            
            //Creates Connection with SQL server
            MySqlConnection connection = new MySqlConnection("server=localhost;user=root;database=mydb;port=3306;password=L3tM31n");

            try
            {
                //Open connection to database
                Console.WriteLine("Connecting to MySQL, mydb Database.");
                connection.Open();
                // Perform database operations 
                MySqlCommand cmd = new MySqlCommand(AddRecord, connection);

                cmd.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            connection.Close(); // close the connection 
            Console.WriteLine("Record successfully added.");
        }
    
        //Function to Delete Records
        private static void DeleteRecord(string LoginID)
        {
            try
            {
                //Create and Open connection
                MySqlConnection connection = new MySqlConnection("server=localhost;user=root;database=mydb;port=3306;password=L3tM31n");
                
                connection.Open();
                MySqlCommand command = new MySqlCommand("DELETE FROM userid WHERE UserID = '" + LoginID + "'", connection);
                    
                command.ExecuteNonQuery();
                    
                    connection.Close(); //Close connection

                Console.WriteLine("Record successfully deleted.");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static void EditRecord(string LoginID, string Record, string Update)
        {
            //Create & open connection to database.

            try
            {
                MySqlConnection connection = new MySqlConnection("server=localhost;user=root;database=mydb;port=3306;password=L3tM31n");

                connection.Open();
                //SQL script to update field given parameters.
                MySqlCommand command = new MySqlCommand("UPDATE " + Record + " SET " +  Record  + " = " + "'" + Update + "'" + " WHERE userid.UserID = " + "'" + LoginID + "'", connection);

                command.ExecuteNonQuery();

                connection.Close();

                Console.WriteLine("Record successfully updated.");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static void ViewData(string LoginID, string column)
        {
            //Create and open connection
            MySqlConnection connection = new MySqlConnection("server=localhost;user=root;database=mydb;port=3306;password=L3tM31n");
            string ViewData = "";
            if (column == "*")
            {

                //string to insert into SQL for viewing data from specific column.
                ViewData = ("SELECT UserID, Forename, Surname, Title, Location, Email, `Login Number`," +
                    " `Phone Number`\r\nFROM userid, email, location, loginid, `phone number`, userid_has_loginid, " +
                    "userid_has_email\r\nWHERE userid.UserID = '" + LoginID + "'\r\nAND userid.UserID = userid_has_loginid.UserID_UserID\r\nAND userid_has_loginid.LoginID_LoginID" +
                    " = loginid.LoginID\r\nAND userid.UserID = userid_has_email.UserID_UserID\r\nAND userid_has_email.Email_EmailID = email.EmailID\r\nAND " +
                    "userid.Location_LocationID = location.LocationID\r\nAND userid.`Phone Number_Phone Number` = `phone number`.`Phone Number`;");

            }
            else if(column == "location")
            {
                ViewData = ("SELECT Location from location, userid WHERE userid.UserID = '" + LoginID + "' AND userid.Location_LocationID = location.LocationID");
            }
            try
            {
                Console.WriteLine("Connecting to MySQL, mydb Database.");
                connection.Open();
                // Perform database operations 
                MySqlCommand cmd = new MySqlCommand(ViewData, connection);

                //Searches and returns a column from a certain record.

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            Console.WriteLine(reader.GetValue(i));
                        }
                        Console.WriteLine();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            connection.Close(); // close the connection 
        }

    }


}

   
