using Gestion_camping;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Gestion_camping {
    public partial class MainWindow : Window {
        // Aceptamos el usuario de vuestro boceto ("admin_recepcion") y también "admin" para pruebas rápidas
        private string usuarioValido1 = "admin_recepcion";
        private string usuarioValido2 = "admin";
        private string passwordValida = "1234";

        public MainWindow() {
            InitializeComponent();
            txtUsuario.Focus();
        }

        // Si pulsa Enter en el usuario, salta directo a la contraseña
        private void txtUsuario_KeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Return) {
                passContrasena.Focus();
            }
        }

        // Si pulsa Enter en la contraseña, intenta iniciar sesión directamente
        private void passContrasena_KeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Return) {
                ValidarAcceso();
            }
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e) {
            ValidarAcceso();
        }

        private void ValidarAcceso() {
            string user = txtUsuario.Text.Trim();
            string pass = passContrasena.Password;

            bool usuarioOk = (user == usuarioValido1 || user == usuarioValido2);
            bool passOk = (pass == passwordValida);

            // Marcamos visualmente si hay error
            txtUsuario.BorderBrush = usuarioOk ? Brushes.Green : Brushes.Red;
            passContrasena.BorderBrush = passOk ? Brushes.Green : Brushes.Red;

            if (usuarioOk && passOk) {
                // Abrimos VentanaPrincipal pasando el usuario conectado
                Ventana_principal principal = new Ventana_principal();
                principal.Show();
                this.Close();
            } else {
                // Diálogo de error idéntico al boceto
                MessageBox.Show("Inicio de sesión incorrecto", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                passContrasena.Clear();
                txtUsuario.Focus();
                txtUsuario.SelectAll();
            }
        }

        private void btnSalir_Click(object sender, RoutedEventArgs e) {
            Application.Current.Shutdown();
        }
    }
}