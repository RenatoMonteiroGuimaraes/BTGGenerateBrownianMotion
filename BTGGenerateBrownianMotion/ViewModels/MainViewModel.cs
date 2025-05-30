using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BTGGenerateBrownianMotion.Models;
using BTGGenerateBrownianMotion.Graphics;

namespace BTGGenerateBrownianMotion.ViewModels
{
    /// <summary>
    /// ViewModel principal da aplicação, responsável por:
    /// - Armazenar os parâmetros de entrada do usuário.
    /// - Executar a simulação do movimento browniano.
    /// - Atualizar o gráfico com os dados gerados.
    /// Implementa o padrão MVVM utilizando CommunityToolkit.Mvvm.
    /// </summary>
    public partial class MainViewModel : ObservableObject
    {
        // ----------------------------
        // Campos temporários de entrada como texto
        // ----------------------------

        [ObservableProperty] private string initialPriceText;
        [ObservableProperty] private string volatilityText;
        [ObservableProperty] private string driftText;
        [ObservableProperty] private string timeText;
        [ObservableProperty] private string simulationsText;

        // ----------------------------
        // Dados da simulação e gráfico
        // ----------------------------

        public ObservableCollection<List<double>> ResultData { get; set; } = new();

        public ChartDrawable ChartDrawable { get; } = new ChartDrawable();

        private readonly Action refreshChartCallback;

        /// <summary>
        /// Construtor que recebe a ação de atualização da interface.
        /// </summary>
        /// <param name="refreshChartCallback">Método a ser chamado após gerar os dados para redesenhar o gráfico</param>
        public MainViewModel(Action refreshChartCallback)
        {
            this.refreshChartCallback = refreshChartCallback;
        }

        /// <summary>
        /// Comando vinculado ao botão "Gerar Simulação".
        /// Gera os dados com base nos parâmetros e atualiza o gráfico.
        /// </summary>
        [RelayCommand]
        private async void Generate()
        {
            // Tenta converter os campos de texto
            bool validInitialPrice = double.TryParse(initialPriceText, out double parsedInitialPrice);
            bool validVolatility = double.TryParse(volatilityText, out double parsedVolatility);
            bool validDrift = double.TryParse(driftText, out double parsedDrift);
            bool validTime = int.TryParse(timeText, out int parsedTime);
            bool validSimulations = int.TryParse(simulationsText, out int parsedSimulations);

            // Verifica se os valores são válidos
            bool valid =
                validInitialPrice && parsedInitialPrice > 0 &&
                validVolatility && parsedVolatility >= 0 &&
                validDrift && // Drift pode ser negativo
                validTime && parsedTime > 0 &&
                validSimulations && parsedSimulations > 0;

            if (!valid)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Erro",
                    "Verifique se todos os campos foram preenchidos corretamente com valores numéricos válidos.",
                    "OK");
                return;
            }

            ResultData.Clear();

            for (int i = 0; i < parsedSimulations; i++)
            {
                var result = GenerateBrownianMotion(parsedVolatility, parsedDrift, parsedInitialPrice, parsedTime).ToList();
                ResultData.Add(result);
            }

            ChartDrawable.Simulations = ResultData.ToList();
            refreshChartCallback?.Invoke();
        }

        /// <summary>
        /// Gera uma simulação de preços com base no modelo de movimento browniano geométrico.
        /// Fórmula usada:
        /// Sₜ = Sₜ₋₁ × exp(μ + σ × Z)
        /// </summary>
        /// <param name="sigma">Volatilidade (em %)</param>
        /// <param name="mean">Média de retorno (em %)</param>
        /// <param name="initialPrice">Valor inicial do ativo</param>
        /// <param name="numDays">Número de passos da simulação</param>
        /// <returns>Array de preços simulados</returns>
        private double[] GenerateBrownianMotion(double sigma, double mean, double initialPrice, int numDays)
        {
            Random rand = new();
            double[] prices = new double[numDays];
            prices[0] = initialPrice;

            for (int i = 1; i < numDays; i++)
            {
                double u1 = 1.0 - rand.NextDouble();
                double u2 = 1.0 - rand.NextDouble();
                double z = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Cos(2.0 * Math.PI * u2);

                double retornoDiario = (mean / 100) + (sigma / 100) * z;

                prices[i] = prices[i - 1] * Math.Exp(retornoDiario);
            }

            return prices;
        }
    }
}
