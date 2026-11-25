using Gestion_camping;
using Gestion_camping.Dominio;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Gestion_camping {
    public partial class Ventana_principal : Window {
        private ObservableCollection<Empleado> listaEmpleados = new ObservableCollection<Empleado>();
        private bool esNuevoEmpleado = false;

        public Ventana_principal() {
            InitializeComponent();
            CargarResumenHoy();
            CargarEmpleadosIniciales();
        }

        public Ventana_principal(string nombreUsuario) : this() {
            txtUsuarioConectado.Text = nombreUsuario;
            txtFechaAcceso.Text = "Último acceso: " + DateTime.Now.ToString("dd/MM/yyyy - HH:mm") + "h";
        }

        // ================= NAVEGACIÓN ENTRE INICIO Y PESTAÑAS =================
        private void IrAPestana(int indicePestana) {
            pnlVistaInicio.Visibility = Visibility.Collapsed;
            tcPestanas.Visibility = Visibility.Visible;
            btnInicio.Visibility = Visibility.Visible;
            tcPestanas.SelectedIndex = indicePestana;
        }

        private void MenuInicio_Alojamientos_Click(object sender, RoutedEventArgs e) { IrAPestana(0); }
        private void MenuInicio_Reservas_Click(object sender, RoutedEventArgs e) { IrAPestana(1); }
        private void MenuInicio_Empleados_Click(object sender, RoutedEventArgs e) { IrAPestana(2); }
        private void MenuInicio_Actividades_Click(object sender, RoutedEventArgs e) { IrAPestana(3); }

        private void VolverAInicio_Click(object sender, RoutedEventArgs e) {
            tcPestanas.Visibility = Visibility.Collapsed;
            btnInicio.Visibility = Visibility.Collapsed;
            pnlVistaInicio.Visibility = Visibility.Visible;
        }

        private void LogoInicio_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            VolverAInicio_Click(sender, e);
        }

        // ================= DATOS DEL PANEL DE RESUMEN (INICIO) =================
        private void CargarResumenHoy() {
            dgResumenLlegadas.ItemsSource = new List<object>
            {
                new { Cliente = "Urko Pérez", Alojamiento = "Bungalow 3", Hora = "16:00" },
                new { Cliente = "María Gómez", Alojamiento = "Parcela A12", Hora = "18:30" },
                new { Cliente = "Luis Sánchez", Alojamiento = "Bungalow 1", Hora = "19:00" }
            };

            dgResumenActividades.ItemsSource = new List<object>
            {
                new { Hora = "10:00", Actividad = "Ruta Senderismo", Publico = "Adultos", Monitor = "Marcos P.", Plazas = "15/20", Lugar = "Recepción" },
                new { Hora = "12:00", Actividad = "Taller Naturaleza", Publico = "Niños", Monitor = "Ana M.", Plazas = "10/10 (Lleno)", Lugar = "Sala Multiusos" },
                new { Hora = "18:00", Actividad = "Torneo Fútbol Sala", Publico = "Todos", Monitor = "Pablo M.", Plazas = "12/20", Lugar = "Pista Deportiva" }
            };
        }

        // ================= CABECERA (AYUDA Y SALIR) =================
        private void btnAyuda_Click(object sender, RoutedEventArgs e) {
            MessageBox.Show(
                "Camping Las Lagunas - Panel de Gestión\n\n" +
                "• Usa el menú lateral en Inicio o las pestañas superiores para navegar.\n" +
                "• Pulsa '🏠 Inicio' o el logo superior izquierdo para volver al resumen diario.\n" +
                "• Selecciona un registro en la lista izquierda para consultar o modificar sus datos.\n" +
                "• Los campos marcados con (*) son obligatorios.",
                "Ayuda del Sistema", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnSalir_Click(object sender, RoutedEventArgs e) {
            if (MessageBox.Show("¿Seguro que deseas cerrar sesión?", "Salir", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes) {
                MainWindow login = new MainWindow();
                login.Show();
                this.Close();
            }
        }

        // ================= GESTIÓN DE EMPLEADOS (PESTAÑA 3) =================
        private void CargarEmpleadosIniciales() {
            string rutaFichero = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Datos", "empleados.txt");

            if (File.Exists(rutaFichero)) {
                foreach (string linea in File.ReadAllLines(rutaFichero)) {
                    if (string.IsNullOrWhiteSpace(linea)) continue;
                    string[] p = linea.Split(';');
                    if (p.Length >= 9) {
                        listaEmpleados.Add(new Empleado {
                            Id = p[0], Nombre = p[1], Apellidos = p[2],
                            Telefono = p[3], Correo = p[4], Estado = p[5],
                            Idiomas = p[6].Replace("|", "\n"),
                            Formacion = p[7].Replace("|", "\n"),
                            Restricciones = p[8].Replace("|", "\n")
                        });
                    }
                }
            }

            dgEmpleados.ItemsSource = listaEmpleados;
            if (listaEmpleados.Count > 0) dgEmpleados.SelectedIndex = 0;
        }

        private void dgEmpleados_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            Empleado emp = dgEmpleados.SelectedItem as Empleado;
            if (emp != null) {
                esNuevoEmpleado = false;
                gbFichaEmpleado.Header = "Ficha de Empleado Seleccionado: " + emp.Id;
                btnEliminarEmpleado.Content = "🗑️ Eliminar " + emp.Id;
                btnEliminarEmpleado.IsEnabled = true;

                txtEmpNombre.Text = emp.Nombre;
                txtEmpApellidos.Text = emp.Apellidos;
                txtEmpTelefono.Text = emp.Telefono;
                txtEmpCorreo.Text = emp.Correo;
                cbEmpEstado.SelectedIndex = (emp.Estado == "Baja") ? 1 : 0;
                txtEmpIdiomas.Text = emp.Idiomas;
                txtEmpFormacion.Text = emp.Formacion;
                txtEmpRestricciones.Text = emp.Restricciones;
            }
        }

        private void FiltroEmpleados_Changed(object sender, EventArgs e) {
            if (dgEmpleados == null) return;

            string texto = txtBuscarEmpleado.Text.Trim().ToLower();
            string estadoSeleccionado = (cbFiltroEstadoEmpleado.SelectedItem as ComboBoxItem)?.Content.ToString();

            var filtrados = listaEmpleados.Where(emp =>
                (string.IsNullOrEmpty(texto) || emp.NombreCompleto.ToLower().Contains(texto) || emp.Id.ToLower().Contains(texto)) &&
                (estadoSeleccionado == "Estado: Todos" || emp.Estado == estadoSeleccionado)
            ).ToList();

            dgEmpleados.ItemsSource = filtrados;
        }

        private void btnNuevoEmpleado_Click(object sender, RoutedEventArgs e) {
            esNuevoEmpleado = true;
            dgEmpleados.SelectedItem = null;
            gbFichaEmpleado.Header = "Nuevo Empleado";
            btnEliminarEmpleado.IsEnabled = false;

            txtEmpNombre.Clear();
            txtEmpApellidos.Clear();
            txtEmpTelefono.Clear();
            txtEmpCorreo.Clear();
            cbEmpEstado.SelectedIndex = 0;
            txtEmpIdiomas.Clear();
            txtEmpFormacion.Clear();
            txtEmpRestricciones.Clear();
            txtEmpNombre.Focus();
        }

        private void btnGuardarEmpleado_Click(object sender, RoutedEventArgs e) {
            if (string.IsNullOrWhiteSpace(txtEmpNombre.Text) ||
                string.IsNullOrWhiteSpace(txtEmpApellidos.Text) ||
                string.IsNullOrWhiteSpace(txtEmpTelefono.Text)) {
                MessageBox.Show("Por favor, rellena todos los campos obligatorios (*).", "Campos incompletos", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string estado = (cbEmpEstado.SelectedItem as ComboBoxItem).Content.ToString();

            if (esNuevoEmpleado) {
                string nuevoId = "E-" + (listaEmpleados.Count + 1).ToString("D2");
                Empleado nuevo = new Empleado {
                    Id = nuevoId,
                    Nombre = txtEmpNombre.Text.Trim(),
                    Apellidos = txtEmpApellidos.Text.Trim(),
                    Telefono = txtEmpTelefono.Text.Trim(),
                    Correo = txtEmpCorreo.Text.Trim(),
                    Estado = estado,
                    Idiomas = txtEmpIdiomas.Text,
                    Formacion = txtEmpFormacion.Text,
                    Restricciones = txtEmpRestricciones.Text
                };
                listaEmpleados.Add(nuevo);
                dgEmpleados.ItemsSource = null;
                dgEmpleados.ItemsSource = listaEmpleados;
                dgEmpleados.SelectedItem = nuevo;
                MessageBox.Show("Empleado " + nuevoId + " creado correctamente.", "Guardado", MessageBoxButton.OK, MessageBoxImage.Information);
            } else {
                Empleado actual = dgEmpleados.SelectedItem as Empleado;
                if (actual != null) {
                    actual.Nombre = txtEmpNombre.Text.Trim();
                    actual.Apellidos = txtEmpApellidos.Text.Trim();
                    actual.Telefono = txtEmpTelefono.Text.Trim();
                    actual.Correo = txtEmpCorreo.Text.Trim();
                    actual.Estado = estado;
                    actual.Idiomas = txtEmpIdiomas.Text;
                    actual.Formacion = txtEmpFormacion.Text;
                    actual.Restricciones = txtEmpRestricciones.Text;

                    dgEmpleados.Items.Refresh();
                    MessageBox.Show("Cambios guardados en " + actual.Id + ".", "Guardado", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void btnDescartarEmpleado_Click(object sender, RoutedEventArgs e) {
            if (dgEmpleados.SelectedItem != null) {
                dgEmpleados_SelectionChanged(null, null);
            } else if (listaEmpleados.Count > 0) {
                dgEmpleados.SelectedIndex = 0;
            }
        }

        private void btnEliminarEmpleado_Click(object sender, RoutedEventArgs e) {
            Empleado actual = dgEmpleados.SelectedItem as Empleado;
            if (actual == null) return;

            MessageBoxResult res = MessageBox.Show(
                "¿Está seguro de que desea eliminar este registro (" + actual.Id + " - " + actual.NombreCompleto + ")?\nEsta acción no se puede deshacer.",
                "Confirmación de borrado",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (res == MessageBoxResult.Yes) {
                listaEmpleados.Remove(actual);
                dgEmpleados.ItemsSource = null;
                dgEmpleados.ItemsSource = listaEmpleados;
                if (listaEmpleados.Count > 0) dgEmpleados.SelectedIndex = 0;
            }
        }
    }
}