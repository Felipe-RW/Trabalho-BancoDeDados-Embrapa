namespace embrapa_bd.Models
{
    public class Acao
    {
        public int Id { get; set; }
        public int PesquisadorId { get; set; }
        public string Nome { get; set; } = " ";
        public string Descricao { get; set; } = " ";
        public string Tipo { get; set; } = " ";
        public DateTime Data { get; set; }

        public int Ano
        {
            get { return Data.Year; }
        }
    }
}