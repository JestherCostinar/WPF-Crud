﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace CRUD
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SqlConnection sqlConnection;
        public MainWindow()
        {
            InitializeComponent();
            string connectionString = ConfigurationManager.ConnectionStrings["CRUD.Properties.Settings.WPFCRUDConnectionString"].ConnectionString;

            sqlConnection = new SqlConnection(connectionString);
            DisplayStores();
            DisplayStoreInventory();
            DisplayAllProducts();
        }

        public void DisplayStores()
        {
            try
            {
                string query = "SELECT * FROM Store";

                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(query, sqlConnection);

                using(sqlDataAdapter)
                {
                    DataTable storeTable = new DataTable();
                    sqlDataAdapter.Fill(storeTable);
                    storeList.DisplayMemberPath = "Name";
                    storeList.SelectedValue = "Id";
                    storeList.ItemsSource = storeTable.DefaultView;
                }
            } catch(Exception ex) {
                MessageBox.Show(ex.ToString());
            }
        }

        private void DisplayStoreInventory()
        {
            try
            {
                string query = "SELECT * FROM Product p INNER JOIN StoreInventory si ON p.Id = si.ProductId WHERE si.StoreId = @StoreId";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);

                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);

                using (sqlDataAdapter)
                {
                    sqlCommand.Parameters.AddWithValue("StoreId", storeList.SelectedValuePath);
                    DataTable inventoryTable = new DataTable();
                    sqlDataAdapter.Fill(inventoryTable);
                    storeInventoryList.DisplayMemberPath = "Brand";
                    storeInventoryList.SelectedValue = "Id";
                    storeInventoryList.ItemsSource = inventoryTable.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void DisplayAllProducts()
        {
            try
            {
                string query = "SELECT * FROM Product";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(query, sqlConnection);

                using (sqlDataAdapter)
                {
                    DataTable productTable = new DataTable();
                    sqlDataAdapter.Fill(productTable);
                    productList.DisplayMemberPath = "Brand";
                    productList.SelectedValue = "Id";
                    productList.ItemsSource = productTable.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void storeList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void AddStoreClick(object sender, RoutedEventArgs e)
        {
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>() {
                new SqlParameter("@Name", SqlDbType.NVarChar) { Value =storeName.Text },
                new SqlParameter("@Street", SqlDbType.NVarChar) { Value =storeStreet.Text },
                new SqlParameter("@City", SqlDbType.NVarChar) { Value =storeCity.Text },
                new SqlParameter("@State", SqlDbType.NChar) { Value =storeState.Text },
                new SqlParameter("@Zip", SqlDbType.Int) { Value =storeZip.Text },
            };

                string query = "INSERT INTO Store (Name, Street, City, State, Zip) Values (@Name, @Street, @City, @State, @Zip)";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddRange(parameters.ToArray());
                DataTable storeTable = new DataTable();
                using (SqlDataAdapter adapter = new SqlDataAdapter(sqlCommand)) adapter.Fill(storeTable);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            } finally
            {
                sqlConnection.Close();
                DisplayStores();
            }
            
        }

        private void DeleteStoreClick(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "DELETE FROM Store WHERE id = @StoreId";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@StoreId", storeList.SelectedIndex+1);
                sqlCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                DisplayStores();
            }
        }

        private void AddInventoryClick(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "INSERT INTO StoreInventory (StoreId, ProductId) VALUES (@StoreId, @ProductId)";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@StoreId", storeList.SelectedValue);
                sqlCommand.Parameters.AddWithValue("@ProductId", productList.SelectedValue);
                sqlCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                DisplayStoreInventory();
            }
        }

        private void DeleteInventoryClick(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "DELETE FROM StoreInventory WHERE ProductId = @ProductId";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@ProductId", storeInventoryList.SelectedIndex+1);
                sqlCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                DisplayStoreInventory();
            }
        }

        private void AddProductClick(object sender, RoutedEventArgs e)
        {
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>() {
                    new SqlParameter("@Manufacturer", SqlDbType.NVarChar) { Value = productManufacturer.Text },
                    new SqlParameter("@Brand", SqlDbType.NVarChar) { Value = productName.Text },
                };

                string query = "INSERT INTO Product (Manufacturer, Brand) VALUES (@Manufacturer, @Brand)";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddRange(parameters.ToArray());
                DataTable productTable = new DataTable();
                using (SqlDataAdapter adapter = new SqlDataAdapter(sqlCommand)) adapter.Fill(productTable);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                DisplayAllProducts();
            }
        }

        private void DeleteProductClick(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "DELETE FROM Product WHERE Id = @ProductId";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@ProductId", productList.SelectedIndex+1);
                sqlCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                DisplayAllProducts();
            }
        }
    }
}
