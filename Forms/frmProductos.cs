using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Punto.Forms
{
    public partial class frmProductos : Form
    {
        private DataAcces coneccion;
        private DataAcces dbHelper = new DataAcces();
        public frmProductos()
        {
            InitializeComponent();
        }
       
        private void CargarProductos()
        {
            MySqlConnection dbConn = dbHelper.ObtenerConexion();
            if (dbConn != null)
            {
                string consulta = "SELECT producto_id, codigo,descripcion, precio, stock, categoria FROM productos";
                MySqlDataAdapter adapter = new MySqlDataAdapter(consulta, dbConn);
                DataTable dataTable = new DataTable();
                
                adapter.Fill(dataTable); 
                dgvProductos.DataSource = dataTable; 
                dbConn.Close();
            }
       
           
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            MySqlConnection dbConn = dbHelper.ObtenerConexion();
            string codigo = txtCodigo.Text.Trim();
            string nombre = txtNombre.Text.Trim();
            string precio = txtPrecio.Text.Trim();
            string stock = txtStock.Text.Trim();
            string categorias = cmbCategorias.Text.Trim();
            if (string.IsNullOrWhiteSpace(codigo))
            {

                MessageBox.Show("El codigo del producto es obligatorio.", "Validacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCodigo.Focus();
                return;

            }
            if (string.IsNullOrWhiteSpace(nombre))
            {

                MessageBox.Show("El nombre del producto es obligatorio.", "Validacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNombre.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(precio))
            {

                MessageBox.Show("El precio del producto es obligatorio.", "Validacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPrecio.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(stock))
            {

                MessageBox.Show("El stock del producto es obligatorio.", "Validacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtStock.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(categorias))
            {

                MessageBox.Show("La categoria del producto es obligatorio.", "Validacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbCategorias.Focus();
                return;
            }

            if (!decimal.TryParse(txtPrecio.Text, out decimal precioValido))
            {
                MessageBox.Show("El formato del precio es invalido (Ej: 15.50).", "Error de Formato", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPrecio.Focus();
                return;
            }

            if (!int.TryParse(txtStock.Text, out int stockValido))
            {
                MessageBox.Show("El stock debe ser un numero entero.", "Error de Formato", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtStock.Focus();
                return;
            }

            if (dbConn != null)
            {
                try
                {
                    string consulta = "INSERT INTO productos (codigo, descripcion, precio, stock, categoria) VALUES (@codigo, @descripcion, @precio, @stock, @categoria)";
                    MySqlCommand command = new MySqlCommand(consulta, dbConn);

                    command.Parameters.AddWithValue("@codigo", codigo);
                    command.Parameters.AddWithValue("@descripcion", nombre);
                    command.Parameters.AddWithValue("@precio", precioValido);
                    command.Parameters.AddWithValue("@stock", stockValido);
                    command.Parameters.AddWithValue("@categoria", categorias);

                    int filasAfectadas = command.ExecuteNonQuery();
                    if (filasAfectadas > 0)
                    {
                        MessageBox.Show("Producto registrado exitosamente.", "Exito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LimpiarFormulario();
                        CargarProductos();
                    }
                    dbConn.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al registrar producto: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        private void dgvProductos_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow fila = dgvProductos.Rows[e.RowIndex];


                txtCodigo.Text = fila.Cells["codigo"].Value.ToString();
                txtNombre.Text = fila.Cells["descripcion"].Value.ToString();
                txtPrecio.Text = fila.Cells["precio"].Value.ToString();
                txtStock.Text = fila.Cells["stock"].Value.ToString();
                cmbCategorias.Text = fila.Cells["categoria"].Value.ToString();
                dgvProductos.Tag = fila.Cells["producto_id"].Value.ToString()   ;
            }
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (dgvProductos.Tag == null)
            {
                MessageBox.Show("Por favor, seleccione un producto de la tabla antes de continuar.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!decimal.TryParse(txtPrecio.Text, out decimal precioValido) || !int.TryParse(txtStock.Text, out int stockValido))
            {
                MessageBox.Show("Verifique que los formatos numericos de Precio y Stock sean correctos.", "Validacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            MySqlConnection dbConn = dbHelper.ObtenerConexion();
            if (dbConn != null)
            {
                try
                {
                    string consulta = "UPDATE productos SET codigo = @codigo, descripcion = @descripcion, precio = @precio, stock = @stock, categoria = @categoria WHERE producto_id = @id";
                    MySqlCommand command = new MySqlCommand(consulta, dbConn);

                    command.Parameters.AddWithValue("@codigo", txtCodigo.Text.Trim());
                    command.Parameters.AddWithValue("@descripcion", txtNombre.Text.Trim());
                    command.Parameters.AddWithValue("@precio", precioValido);
                    command.Parameters.AddWithValue("@stock", stockValido);
                    command.Parameters.AddWithValue("@categoria", cmbCategorias.Text.Trim());
                    command.Parameters.AddWithValue("@id", dgvProductos.Tag); 

                    int filasAfectadas = command.ExecuteNonQuery();
                    if (filasAfectadas > 0)
                    {
                        MessageBox.Show("Producto actualizado con éxito.", "Exito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LimpiarFormulario();
                        CargarProductos(); 
                    }
                    dbConn.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al actualizar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvProductos.Tag == null)
            {
                MessageBox.Show("Por favor, seleccione un producto de la tabla antes de continuar.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            DialogResult confirmacion = MessageBox.Show("¿Seguro que deseas eliminar este producto?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirmacion == DialogResult.Yes)
            {
                MySqlConnection dbConn = dbHelper.ObtenerConexion();
                if(dbConn != null)
                {
                    try
                    {
                        string consulta = "DELETE FROM productos WHERE producto_id = @id";
                        MySqlCommand command = new MySqlCommand(consulta, dbConn);
                        command.Parameters.AddWithValue("@id", dgvProductos.Tag);

                        int filasAfectadas = command.ExecuteNonQuery();
                        if (filasAfectadas > 0)
                        {
                            MessageBox.Show("Producto eliminado exitosamente.", "Exito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LimpiarFormulario();
                            CargarProductos();

                        }
                        dbConn.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error al eliminar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        private void LimpiarFormulario()
        {
            txtCodigo.Clear();
            txtCodigo.Clear();
            txtNombre.Clear();
            txtPrecio.Clear();
            txtStock.Clear();
            cmbCategorias.SelectedIndex = -1;
            txtCodigo.Focus();
            dgvProductos.Tag = null;
        }

        private void frmProductos_Load_1(object sender, EventArgs e)
        {
            CargarProductos();
        }

        private void txtBusqueda_TextChanged(object sender, EventArgs e)
        {
            coneccion = new DataAcces();
            MySqlConnection dbConn = dbHelper.ObtenerConexion();

            if (dbConn != null)
            {
                TextBox txt = (TextBox)sender;
                string consulta = "SELECT producto_id, codigo, descripcion, precio, stock, categoria FROM productos WHERE descripcion LIKE @busqueda OR codigo LIKE @busqueda";
                MySqlDataAdapter adapter = new MySqlDataAdapter(consulta, dbConn);
                adapter.SelectCommand.Parameters.AddWithValue("@busqueda", "%" + txt.Text + "%");

                DataTable dt = new DataTable();
                adapter.Fill(dt);

                dgvProductos.DataSource = dt;
                dbConn.Close();
            }
        }
    }
}
