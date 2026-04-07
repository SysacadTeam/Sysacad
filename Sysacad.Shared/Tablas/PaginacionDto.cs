namespace Sysacad.Shared.Tablas
{
    public class PaginacionDto
    {
        private int _pagina = 1;
        public int Pagina
        {
            get => _pagina;
            set => _pagina = value < 1 ? 1 : value;
        }

        public int Total { get; set; }

        private int _registrosPorPagina = 10;
        public int RegistrosPorPagina
        {
            get => _registrosPorPagina;
            set => _registrosPorPagina = value < 1 ? 10 : value;
        }

        public int TotalPaginas => (int)Math.Ceiling((decimal)Total / RegistrosPorPagina);
    }
}
