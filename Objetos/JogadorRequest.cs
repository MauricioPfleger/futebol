namespace futebol.Objetos
{
    public class JogadorRequest
    {
        public string Nome { get; set; }
        public string? Numero { get; set; }
        public int? idClube { get; set; }
        public decimal? Salario {get; set; }
    }
}