namespace embrapa_bd.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nome { get; set; } = "";
        public string Email { get; set; } = "";
        public string Senha { get; set; } = "";
        public int Matricula { get; set; }
        public bool Ativo { get; set; }
    }
}