using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Punto.Forms
{
    internal class DataAcces
    {
        private readonly string cadena;

        public DataAcces()
        {
            cadena = "Server=127.0.0.1;Database=puntodb;Uid=root;Pwd=;Port=3306;SslMode=0;";
        }
        public MySqlConnection ObtenerConexion()
        {
            try
            {
                MySqlConnection con = new MySqlConnection(cadena);
                con.Open();
                return con;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error de coneccion : " + ex.ToString());
                return null;
            }
        }
    }
}
