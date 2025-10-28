public enum PlanoLocacao
{
    SeteDias = 7,
    QuinzeDias = 15,
    TrintaDias = 30,
    QuarentaCincoDias = 45,
    CinquentaDias = 50
}

public static class PlanoLocacaoInfo
{
    public static decimal ObterValorDiaria(PlanoLocacao plano) => plano switch
    {
        PlanoLocacao.SeteDias => 30m,
        PlanoLocacao.QuinzeDias => 28m,
        PlanoLocacao.TrintaDias => 22m,
        PlanoLocacao.QuarentaCincoDias => 20m,
        PlanoLocacao.CinquentaDias => 18m,
        _ => throw new ArgumentOutOfRangeException(nameof(plano), "Plano inválido")
    };

    public static decimal ObterMulta(PlanoLocacao plano) => plano switch
    {
        PlanoLocacao.SeteDias => 0.2m,    // 20%
        PlanoLocacao.QuinzeDias => 0.4m,  // 40%
        _ => 0m
    };
}