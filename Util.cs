using System.Globalization;
using System.Text;

namespace DesafioBackend
{
    public class Util
    {
        public static Mensagem RemoverAcentos(string texto, out string textoSemAcento)
        {
            textoSemAcento = string.Empty;
            try
            {
                var normalized = texto.Normalize(NormalizationForm.FormD);
                var sb = new StringBuilder();
                foreach (var c in normalized)
                {
                    if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                        sb.Append(c);
                }
                textoSemAcento =  sb.ToString().Normalize(NormalizationForm.FormC);
                return new Mensagem();

            }catch (Exception ex)
            {
                return new Mensagem(ex.Message, ex);
            }
        }
    }
}
