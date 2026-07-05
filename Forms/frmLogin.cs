using Google.Protobuf.WellKnownTypes;
using MySql.Data.MySqlClient;
using System.Windows.Forms;
using System;

namespace Punto.Forms
{
    public partial class frmLogin : Form
    {
        private DataAcces Conexion;
        public frmLogin()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, System.EventArgs e)
        {
            string Usuario = txtUser.Text.Trim();
            string pass = txtPassword.Text.Trim();

            if(string.IsNullOrEmpty(Usuario) || string.IsNullOrEmpty(pass))
            {
                MessageBox.Show("Por favor, llena todos los campos.", "Campos vacíos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataAcces conClase = new DataAcces();
            MySqlConnection dbConn = conClase.ObtenerConexion();

           

            if (dbConn != null)
            {

                try
                {
                    string consulta = "SELECT nombre_completo FROM usuarios WHERE username = @user AND PASSWORD = @pass";

                    MySqlCommand command= new MySqlCommand(consulta, dbConn);
                    command.Parameters.AddWithValue("@user", Usuario);
                    command.Parameters.AddWithValue("@pass", pass);

                    object resultado = command.ExecuteScalar();
                    if (resultado != null) // Si encontró al estudiante...
                    {
                        string nombreEstudiante = resultado.ToString();
                        MessageBox.Show("¡Bienvenido " + nombreEstudiante + "!");
                        frmPrincipal principal = new frmPrincipal();
                        this.Hide();
                        principal.Show();
                        // Aquí se abriría el siguiente formulario y se ocultaría este
                    }
                    else
                    {
                        MessageBox.Show("Usuario o contraseña incorrectos. Verifique sus datos.","Mensaje Preventivo",MessageBoxButtons.OK, MessageBoxIcon.Stop);

                        txtPassword.Clear();
                        txtPassword.Focus();
                    }
                    dbConn.Close();

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al consultar: " + ex.Message);
                }

                
            }


            
        }
    }
}
