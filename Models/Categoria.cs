namespace embrapa_bd.Models
{
    public class Categoria
    {
        // Deve acompanhar o tamanho da coluna nome no banco de dados.
        public const int LimiteNome = 100;

        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
    }
}
