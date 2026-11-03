namespace embrapa_bd.Models
{
    public class Acao
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = " ";
        public string Descricao { get; set; } = " ";
        public string Status { get; set; } = " ";
        public string CategoriaId { get; set; } = " ";
        public DateTime DataRealizacao { get; set; }

    }
}