using System;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Drawing;

namespace Fishing
{
    public partial class ProductForm : Form
    {
        string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Trade;Integrated Security=True";

        public ProductForm()
        {
            InitializeComponent();
        }

        private void ProductForm_Load(object sender, EventArgs e)
        {
            LoadProductData();
        }

        private void LoadProductData()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Запрос к базе данных
                string query = "SELECT [ProductArticleNumber], [ProductName], [ProductDescription], [ProductManufacturer], [ProductCost], [ProductQuantityInStock], [ProductPhoto] FROM [Trade].[dbo].[Product]";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Присвоение значений элементам управления
                            string productName = reader["ProductName"].ToString();
                            string productDescription = reader["ProductDescription"].ToString();
                            string productManufacturer = reader["ProductManufacturer"].ToString();
                            string productCost = reader["ProductCost"].ToString();
                            string productQuantity = reader["ProductQuantityInStock"].ToString();

                            // Загрузка изображения
                            Image productImage = GetProductImage(reader["ProductPhoto"]);

                            // Создание контейнера для отображения информации о продукте
                            Panel productPanel = CreateProductPanel(productName, productDescription, productManufacturer, productCost, productQuantity, productImage);

                            // Добавление контейнера на FlowLayoutPanel
                            flowLayoutPanelProducts.Controls.Add(productPanel);
                        }
                    }
                }
            }
        }

        private Image GetProductImage(object photoData)
        {
            if (photoData != DBNull.Value && photoData is byte[] imageData && imageData.Length > 0)
            {
                using (System.IO.MemoryStream ms = new System.IO.MemoryStream(imageData))
                {
                    return Image.FromStream(ms);
                }
            }
            else
            {
                // Используйте изображение-заглушку, если фото отсутствует
                return Image.FromFile("C:\\Users\\stud305\\source\\repos\\Fishing\\Resources\\placeholder.png");
            }
        }

        private Panel CreateProductPanel(string name, string description, string manufacturer, string cost, string quantity, Image image)
        {
            Panel productPanel = new Panel();

            // Настройка контейнера
            productPanel.BorderStyle = BorderStyle.FixedSingle;
            productPanel.Size = new Size(1000, 150);

            // Создание и добавление элементов управления в контейнер
            PictureBox pictureBox = new PictureBox();
            pictureBox.Image = image;
            pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox.Size = new Size(150, 150);
            productPanel.Controls.Add(pictureBox);
            pictureBox.Location = new Point(10, 5);

            Label nameLabel = new Label();
            nameLabel.Text = name;
            nameLabel.AutoSize = true;
            productPanel.Controls.Add(nameLabel);
            nameLabel.Location = new Point(400, 30);


            Label descriptionLabel = new Label();
            descriptionLabel.Text = description;
            descriptionLabel.AutoSize = true;
            productPanel.Controls.Add(descriptionLabel);
            descriptionLabel.Location = new Point(400, 50);

            Label manufacturerLabel = new Label();
            manufacturerLabel.Text = "Производитель: " + manufacturer;
            manufacturerLabel.AutoSize = true;
            productPanel.Controls.Add(manufacturerLabel);
            manufacturerLabel.Location = new Point(400, 70);

            Label costLabel = new Label();
            costLabel.Text = "Цена: " + cost;
            costLabel.AutoSize = true;
            productPanel.Controls.Add(costLabel);
            costLabel.Location = new Point(400, 90);

            Label quantityLabel = new Label();
            quantityLabel.Text = "На складе: " + quantity;
            quantityLabel.AutoSize = true;
            productPanel.Controls.Add(quantityLabel);
            quantityLabel.Location = new Point(800, 80);

            return productPanel;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AutorizationForm auth = new AutorizationForm();
            auth.Show();
            this.Close();
        }
    }
}
