namespace OrientaTEC_MVC.Models
{
    public class SesionSingleton
    {
        private static SesionSingleton _instance;
        private static readonly object _lock = new object();





        public static SesionSingleton Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new SesionSingleton();
                        }
                    }
                }
                return _instance;
            }
        }

        public DateTime FECHA_DEL_SISTEMA { get; set; }


        public IUsuario UsuarioActual { get; set; }

        private SesionSingleton() { }
    }
}
