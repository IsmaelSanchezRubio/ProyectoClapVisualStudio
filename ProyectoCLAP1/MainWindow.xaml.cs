using System;
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

namespace ProyectoCLAP1
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        String vacio = "";
        SqlConnection sqlConnection;
        public MainWindow()
        {
            InitializeComponent();
            //Conexion
            string connectionString = ConfigurationManager.ConnectionStrings["ProyectoCLAP1.Properties.Settings.ProyectoCLAPdbConnectionString"].ConnectionString;

            sqlConnection = new SqlConnection(connectionString);

            MuestraZonas();
            MuestraPresas();
            MuestraVias();
            ActualizarTablaTiempos();

        }
        
        //METODOS
        private void MuestraZonas()
        {
            try
            {  
                string consulta = "select * from Zona";
                //Adaptar C# a SQL
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(consulta, sqlConnection);
                using (sqlDataAdapter)
                {
                    //Almacenamos el contenido de la tabla en un objeto
                    DataTable zonaTabla = new DataTable();
                    sqlDataAdapter.Fill(zonaTabla);
                    //Indicamos lo que queremos ver
                    ListaZonas.DisplayMemberPath = "Nombre";
                    //Como lo encuentra
                    ListaZonas.SelectedValuePath = "Id";
                    //Formato
                    ListaZonas.ItemsSource = zonaTabla.DefaultView;
                }
            }
            catch (Exception e)
            {
                //Manejo de errores
                MessageBox.Show(e.ToString());
            }
        }
        private void MuestraPresas()
        {
            try
            {
                string consulta = "select * from Presa";
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(consulta, sqlConnection);
                using (sqlDataAdapter)
                {
                    DataTable presaTabla = new DataTable();
                    sqlDataAdapter.Fill(presaTabla);
                    ListaPresasTotales.DisplayMemberPath = "Nombre";
                    ListaPresasTotales.SelectedValuePath = "Id";
                    ListaPresasTotales.ItemsSource = presaTabla.DefaultView;
                }
            }
            catch (Exception e)
            {
                //Manejo de errores
                MessageBox.Show(e.ToString());
            }
        }
        private void MuestraVias()
        {
            try
            {
                string consulta = "select * from Via";
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(consulta, sqlConnection);
                using (sqlDataAdapter)
                {
                    DataTable viaTabla = new DataTable();
                    sqlDataAdapter.Fill(viaTabla);
                    ListaViasTotales.DisplayMemberPath = "Nombre";
                    ListaViasTotales.SelectedValuePath = "Id";
                    ListaViasTotales.ItemsSource = viaTabla.DefaultView;
                }
            }
            catch (Exception e)
            {
                //Manejo de errores
                MessageBox.Show(e.ToString());
            }
        }
        private void MuestraViasAsociadas()
        {
            try
            {
                string consulta = "select * from Via a Inner Join ViaZona b " +
                    "on a.Id = b.ViaId where b.ZonaId = @ZonaId";
                SqlCommand sqlCommand = new SqlCommand(consulta, sqlConnection);
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                using (sqlDataAdapter)
                {
                    sqlCommand.Parameters.AddWithValue("@ZonaId", ListaZonas.SelectedValue);

                    DataTable viaTabla = new DataTable();
                    sqlDataAdapter.Fill(viaTabla);
                    ListaVias.DisplayMemberPath = "Nombre";
                    ListaVias.SelectedValuePath = "Id";
                    ListaVias.ItemsSource = viaTabla.DefaultView;
                    //reset listapresas
                    ListaPresas.ItemsSource = vacio;
                }
            }
            catch (Exception e)
            {
                //MessageBox.Show(e.ToString());
            }
        }
        private void MuestraPresasAsociadas()
        {
            try
            {
                string consulta = "select * from Presa a Inner Join PresaVia b " +
                    "on a.Id = b.PresaId where b.ViaId = @ViaId";
                SqlCommand sqlCommand = new SqlCommand(consulta, sqlConnection);
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                using (sqlDataAdapter)
                {
                    sqlCommand.Parameters.AddWithValue("@ViaId", ListaVias.SelectedValue);

                    DataTable PresaTabla = new DataTable();
                    sqlDataAdapter.Fill(PresaTabla);
                    ListaPresas.DisplayMemberPath = "Nombre";
                    ListaPresas.SelectedValuePath = "Id";
                    ListaPresas.ItemsSource = PresaTabla.DefaultView;
                }
            }
            catch (Exception e)
            {
                //MessageBox.Show(e.ToString());
            }
        }
        private void MuestraPresasAsociadas2()
        {
            try
            {
                string consulta = "select * from Presa a Inner Join PresaVia b " +
                    "on a.Id = b.PresaId where b.ViaId = @ViaId";
                SqlCommand sqlCommand = new SqlCommand(consulta, sqlConnection);
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                using (sqlDataAdapter)
                {
                    sqlCommand.Parameters.AddWithValue("@ViaId", ListaViasTotales.SelectedValue);

                    DataTable PresaTabla = new DataTable();
                    sqlDataAdapter.Fill(PresaTabla);
                    ListaPresas.DisplayMemberPath = "Nombre";
                    ListaPresas.SelectedValuePath = "Id";
                    ListaPresas.ItemsSource = PresaTabla.DefaultView;
                }
            }
            catch (Exception e)
            {
                //MessageBox.Show(e.ToString());
            }
        }

        //SELECCION LISTAS
        private void ListaZonas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MuestraViasAsociadas();
        }
        private void ListaVias_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MuestraPresasAsociadas();
        }
        private void ListaViasTotales_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MuestraPresasAsociadas2();
        }
        
        //BOTONES

        //AÑADIR-ELIMINAR
        private void EliminarVia_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string consulta = "Delete from Via where Id = @viaId";
                SqlCommand sqlCommand = new SqlCommand(consulta, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@viaId", ListaViasTotales.SelectedValue);
                sqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                MuestraVias();
                //reset listapresas
                ListaPresas.ItemsSource = vacio;
            }
        }
        private void AgregarVia_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string consulta = "Insert into via (Nombre) values (@Nombre)";
                SqlCommand sqlCommand = new SqlCommand(consulta, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@Nombre", MiTexBox.Text);
                sqlCommand.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                MuestraVias();
                MiTexBox.Text = null;
            }
        }
        private void EliminarPresa_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string consulta = "Delete from Presa where Id = @PresaId";
                SqlCommand sqlCommand = new SqlCommand(consulta, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@PresaId", ListaPresasTotales.SelectedValue);
                sqlCommand.ExecuteNonQuery(); 
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                MuestraPresas();
                ListaPresas.ItemsSource = vacio;
            }
        }
        private void AgregarPresa_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string consulta = "Insert into Presa (Nombre) values (@Nombre)";
                SqlCommand sqlCommand = new SqlCommand(consulta, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@Nombre", MiTexBox.Text);
                sqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                MuestraPresas();
                MiTexBox.Text = null;
            }
        }
        private void EliminarZona_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string consulta = "Delete from Zona where Id = @ZonaId";
                SqlCommand sqlCommand = new SqlCommand(consulta, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@ZonaId", ListaZonas.SelectedValue);
                sqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                MuestraZonas();
                //reset listaVias
                ListaVias.ItemsSource = vacio;
                ListaPresas.ItemsSource = vacio;
            }
        }
        private void AgregarZona_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string consulta = "Insert into Zona (Nombre) values (@Nombre)";
                SqlCommand sqlCommand = new SqlCommand(consulta, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@Nombre", MiTexBox.Text);
                sqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                MuestraZonas();
                MiTexBox.Text = null;
            }
        }

        //MODIFICAR
        private void AñadirPresaVia_Click(object sender, RoutedEventArgs e)
        {
            
            try
            {
                string consulta = "Insert into PresaVia values (@ViaId, @PresaId)";
                SqlCommand sqlCommand = new SqlCommand(consulta, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@PresaId", ListaPresasTotales.SelectedValue);
                sqlCommand.Parameters.AddWithValue("@ViaId", ListaViasTotales.SelectedValue);
                sqlCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                MuestraPresasAsociadas2();
            }
            
        }
        private void SacarPresaVia_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                string consulta = "delete from PresaVia where PresaId = @PresaId and ViaId = @ViaId";
                SqlCommand sqlCommand = new SqlCommand(consulta, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@PresaId", ListaPresas.SelectedValue);
                sqlCommand.Parameters.AddWithValue("@ViaId", ListaViasTotales.SelectedValue);
                sqlCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                MuestraPresasAsociadas2();
            }

        }
        private void AñadirViaZona_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string consulta = "Insert into ViaZona values (@ZonaId, @ViaId)";
                SqlCommand sqlCommand = new SqlCommand(consulta, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@ZonaId", ListaZonas.SelectedValue);
                sqlCommand.Parameters.AddWithValue("@ViaId", ListaViasTotales.SelectedValue);
                sqlCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                MuestraViasAsociadas();
            }
        }
        private void SacarViaZona_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string consulta = "delete from ViaZona where ViaId = @ViaId and ZonaId = @ZonaId";
                SqlCommand sqlCommand = new SqlCommand(consulta, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@ViaId", ListaVias.SelectedValue);
                sqlCommand.Parameters.AddWithValue("@ZonaId", ListaZonas.SelectedValue);
                sqlCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                MuestraViasAsociadas();
            }
        }

        //BARRA TIEMPOS
        
        private void MuestraUsuariosB()
        {
            try
            {  
                string consulta = "SELECT Usuarios.Usuario FROM Tiempos JOIN Usuarios ON Tiempos.UsuarioId = Usuarios.Id";
                //Adaptar C# a SQL
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(consulta, sqlConnection);
                using (sqlDataAdapter)
                {
                    //Almacenamos el contenido de la tabla en un objeto
                    DataTable nombresBTabla = new DataTable();
                    sqlDataAdapter.Fill(nombresBTabla);
                    //Indicamos lo que queremos ver
                    TbUsuario.DisplayMemberPath = "Usuario";
                    //Como lo encuentra
                    TbUsuario.SelectedValuePath = "Id";
                    //Formato
                    TbUsuario.ItemsSource = nombresBTabla.DefaultView;
                }
            }
            catch (Exception e)
            {
                //Manejo de errores
                MessageBox.Show(e.ToString());
            }
        }
        private void MuestraViasB()
        {
            try
            {
                string consulta = "SELECT via.Nombre FROM Tiempos JOIN via ON Tiempos.ViaId = via.Id";
                //Adaptar C# a SQL
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(consulta, sqlConnection);
                using (sqlDataAdapter)
                {
                    //Almacenamos el contenido de la tabla en un objeto
                    DataTable viasBTabla = new DataTable();
                    sqlDataAdapter.Fill(viasBTabla);
                    //Indicamos lo que queremos ver
                    TbVia.DisplayMemberPath = "Nombre";
                    //Como lo encuentra
                    TbVia.SelectedValuePath = "Id";
                    //Formato
                    TbVia.ItemsSource = viasBTabla.DefaultView;
                }
            }
            catch (Exception e)
            {
                //Manejo de errores
                MessageBox.Show(e.ToString());
            }
        }
        private void MuestraTiemposB()
        {
            try
            {
                string consulta = "SELECT Tiempo FROM Tiempos";
                //Adaptar C# a SQL
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(consulta, sqlConnection);
                using (sqlDataAdapter)
                {
                    //Almacenamos el contenido de la tabla en un objeto
                    DataTable tiemposBTabla = new DataTable();
                    sqlDataAdapter.Fill(tiemposBTabla);
                    //Indicamos lo que queremos ver
                    TbTiempo.DisplayMemberPath = "Tiempo";
                    //Como lo encuentra
                    TbTiempo.SelectedValuePath = "Id";
                    //Formato
                    TbTiempo.ItemsSource = tiemposBTabla.DefaultView;
                }
            }
            catch (Exception e)
            {
                //Manejo de errores
                MessageBox.Show(e.ToString());
            }
        }
        private void ActualizarTablaTiempos()
        {
            MuestraUsuariosB();
            MuestraViasB();
            MuestraTiemposB();
        }
        private void Actualizar_Click(object sender, RoutedEventArgs e)
        {
            ActualizarTablaTiempos();
        }
    }
}
