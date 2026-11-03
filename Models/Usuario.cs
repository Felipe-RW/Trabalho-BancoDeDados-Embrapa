namespace embrapa_bd.Models
{
    public class Usuarios
    {
        public int Id { get; set; }
        public string Nome { get; set; } = " ";
        public string Email { get; set; } = " ";
        public string Senha { get; set; } = " ";
        public string Matricula { get; set; } = " ";
        public bool Ativo { get; set; } = true;

        public override string ToString() => $"{Id, 4} { Nome, -25} {Email}";
    }
}
