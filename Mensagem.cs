namespace DesafioBackend
{
    public class Mensagem
    {
        public bool Sucesso = true;
        public string Descricao = string.Empty;
        public Exception Excecao;
        public Mensagem()
        {

        }
        public Mensagem(string mensagem)
        {
            Sucesso = false;
            Descricao = mensagem;
        }
        public Mensagem(string mensagem, Exception ex)
        {
            Sucesso = false;
            Descricao = mensagem;
            Excecao = ex;
        }
    }
}
