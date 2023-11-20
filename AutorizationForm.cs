using System;
using System.Data.SqlClient;
using System.Linq;  // Добавьте эту директиву
using System.Windows.Forms;

namespace Fishing
{
    public partial class AutorizationForm : Form
    {
        string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Trade;Integrated Security=True";
        private int failedAttempts = 0;
        private string generatedCaptcha;
        private Label captchaLabel;
        private TextBox captchaTextBox;
        private Timer buttonLockTimer;
        private bool isButtonLocked = false;

        public AutorizationForm()
        {
            InitializeComponent();
            InitializeCaptchaControls(); // вызов метода для инициализации элементов капчи
            buttonLockTimer = new Timer();
            buttonLockTimer.Interval = 10000;
            buttonLockTimer.Tick += ButtonLockTimer_Tick;
        }

        private void InitializeCaptchaControls()
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string username = usernameTextBox.Text;
            string password = passwordTextBox.Text;
            string enteredCaptcha = captchaTextBox.Text;

           
            if (IsUserAuthenticated(username, password, enteredCaptcha))
            {
                ProductForm product = new ProductForm();
                product.Show();
                this.Hide();
            }
            else
            {
                failedAttempts++;
                
                if (failedAttempts >= 2)
                {
                    if (!isButtonLocked)
                    {
                        ShowCaptcha();
                        if (captchaLabel.Visible == true)
                        {
                            MessageBox.Show("Неправильный логин, пароль или Captcha. Авторизация не удалась.");
                        }
                        LockButton();
                    }
                    else
                    {
                        MessageBox.Show("Слишком много попыток, ждите 10 секунд.");
                    }
                    
                }
                else
                {
                    MessageBox.Show("Неправильный логин или пароль. Авторизация не удалась.");
                }
            }
        }

        private void LockButton()
        {
            isButtonLocked = true;
            buttonLockTimer.Start();
        }

        private bool IsUserAuthenticated(string username, string password, string enteredCaptcha)
        {
            if (failedAttempts >= 2)
            {
                return enteredCaptcha.Equals(generatedCaptcha, StringComparison.OrdinalIgnoreCase) &&
                       CheckCredentials(username, password);
            }
            else
            {
                return CheckCredentials(username, password);
            }
        }

        private bool CheckCredentials(string username, string password)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT COUNT(1) FROM [User] WHERE UserLogin = @Username AND UserPassword = @Password";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Password", password);
                    connection.Open();
                    int count = (int)command.ExecuteScalar();
                    return count > 0;
                }
            }
        }
        private void ButtonLockTimer_Tick(object sender, EventArgs e)
        {
            // Срабатывание таймера после 10 секунд
            buttonLockTimer.Stop();
            isButtonLocked = false;
        }
        private void ShowCaptcha()
        {
            GenerateCaptcha();
            captchaLabel.Visible = true;
            captchaTextBox.Visible = true;
        }

        private void GenerateCaptcha()
        {
            
            Random random = new Random();
            generatedCaptcha = new string(Enumerable.Repeat("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", 4)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            captchaLabel.Text = generatedCaptcha;

            captchaTextBox.Clear();
        }

        private void label6_Click(object sender, EventArgs e)
        {
            
        }

        private void AutorizationForm_Load(object sender, EventArgs e)
        {

        }

        private void label6_Click_1(object sender, EventArgs e)
        {
            ProductForm product = new ProductForm();
            product.Show();
            this.Hide();
        }
    }
}
