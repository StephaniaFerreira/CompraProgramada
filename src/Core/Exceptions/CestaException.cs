namespace Core.Exceptions
{
    public class CestaException : Exception
    {
        public string Codigo { get; }

        public CestaException(string mensagem, string codigo) : base(mensagem)
        {
            Codigo = codigo;
        }

        public string Mensagem => base.Message;
    }
}
