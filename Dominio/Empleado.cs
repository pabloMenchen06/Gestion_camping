namespace Gestion_camping.Dominio {
    public class Empleado {
        public string Id { get; set; }
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public string Telefono { get; set; }
        public string Correo { get; set; }
        public string Estado { get; set; } // "Activo" o "Baja"
        public string Idiomas { get; set; }
        public string Formacion { get; set; }
        public string Restricciones { get; set; }

        // Propiedad auxiliar para mostrar Nombre + Apellidos juntos en la tabla
        public string NombreCompleto {
            get { return Nombre + " " + Apellidos; }
        }
    }
}
