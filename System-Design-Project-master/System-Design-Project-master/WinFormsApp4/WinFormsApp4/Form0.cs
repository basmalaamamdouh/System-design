using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic.ApplicationServices;

namespace WinFormsApp4
{
    public partial class Form0 : Form
    {
        public Form0()
        {
            InitializeComponent();
        }

        private void Form0_Load(object sender, EventArgs e)
        {

        }
        // Method to check admin login
        private bool LoginAdmin(string username, string password)
        {
            string connectionString = "Data Source=DESKTOP-E3IT50S\\SQLEXPRESS01;Initial Catalog=ComplaintsSystem;Integrated Security=True;TrustServerCertificate=True";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT COUNT(1) FROM Admins WHERE Username = @Username AND Password = @Password";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Password", password);
                    connection.Open();
                    int result = Convert.ToInt32(command.ExecuteScalar());
                    return result == 1; // If result is 1, admin exists
                }
            }
        }
        private bool LoginUser(string username, string password, out int userID)
        {
            //  int userID = -1; // Default: user not found
            string connectionString = "Data Source=DESKTOP-E3IT50S\\SQLEXPRESS01;Initial Catalog=ComplaintsSystem;Integrated Security=True;TrustServerCertificate=True";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT UserID FROM Users WHERE Username = @Username AND Password = @Password";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Password", password);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        userID = Convert.ToInt32(reader["UserID"]);
                        return true; // User found and logged in
                    }
                    else
                    {
                        userID = 0;
                        return false; // Invalid username or password
                    }
                }
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string username = textBox1.Text;
            string password = textBox2.Text;

            int userID = 0;  // Variable to hold the user ID

            // Check if the login is for an admin
            if (LoginAdmin(username, password)) // Check if the admin exists in the Admins table
            {
                MessageBox.Show("Admin Login Successful!");
                // Open Admin Dashboard (Form4)
                Form4 adminDashboardForm = new Form4();
                adminDashboardForm.Show();
                this.Hide(); // Hide the Login Form
            }
            // If not admin, check if it's a user login
            else if (LoginUser(username, password, out userID)) // Check if the user exists in the Users table
            {
                MessageBox.Show("User Login Successful!");
                // Open Complaint Submission Form (Form1) and pass the UserID
                Form1 complaintForm = new Form1(userID);  // Pass the UserID to Form1
                complaintForm.Show();
                this.Hide(); // Hide the Login Form
            }
            else
            {
                MessageBox.Show("Invalid Username or Password. Please try again.");
            }
        }

        // Register button click event (You can implement registration logic later)

        private bool UserExists(string username)
        {
            string connectionString = "Data Source=DESKTOP-E3IT50S\\SQLEXPRESS01;Initial Catalog=ComplaintsSystem;Integrated Security=True;TrustServerCertificate=True";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT COUNT(1) FROM Users WHERE Username = @Username";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    connection.Open();
                    int result = Convert.ToInt32(command.ExecuteScalar());
                    return result > 0;  // If result is greater than 0, the user exists
                }
            }
        }
        private int RegisterUser(string username, string password)
        {
            string connectionString = "Data Source=DESKTOP-E3IT50S\\SQLEXPRESS01;Initial Catalog=ComplaintsSystem;Integrated Security=True;TrustServerCertificate=True";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO Users (UserID, Username, Password) VALUES (@UserID, @Username, @Password)";

                connection.Open(); // Open the connection only once
                int newUserID = GetNextUserID(connection);

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", newUserID);
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Password", password);

                    command.ExecuteNonQuery();
                    return newUserID; // Return the UserID
                }
            } // The connection is automatically closed when exiting the using block
        }


        // Helper method to get the next UserID
        private int GetNextUserID(SqlConnection connection)
        {
            string query = "SELECT ISNULL(MAX(UserID), 0) + 1 FROM Users";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                // Do NOT open the connection here; assume it's already open.
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }


        private void RegisterButton_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click_1(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            string username = textBox1.Text;
            string password = textBox2.Text;



            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter a valid username and password.");
                return;
            }

            if (UserExists(username))
            {
                MessageBox.Show("Username already exists. Please choose a different username.");
                return;
            }

            try
            {
                // Register the user

                int userID = RegisterUser(username, password);
                MessageBox.Show("User Registered Successfully!");

                // Open Complaint Submission Form (Form1)
                Form1 complaintForm = new Form1(userID); // Pass the user ID to Form1
                complaintForm.Show();
                this.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Registration failed. Error: {ex.Message}");
            }

        }
    }

}
