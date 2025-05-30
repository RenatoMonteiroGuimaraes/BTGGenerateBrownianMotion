using Microsoft.Maui.Graphics;
using System.Linq;

namespace BTGGenerateBrownianMotion.Graphics
{
    /// <summary>
    /// Classe responsável por desenhar o gráfico de movimento browniano
    /// utilizando GraphicsView e a interface IDrawable do .NET MAUI.
    /// </summary>
    public class ChartDrawable : IDrawable
    {
        /// <summary>
        /// Conjunto de simulações de preços. Cada lista representa uma simulação completa.
        /// </summary>
        public List<List<double>> Simulations { get; set; } = new();

        /// <summary>
        /// Responsável por desenhar todo o conteúdo gráfico na tela, incluindo os eixos,
        /// escalas e as curvas de simulação. Este método é executado automaticamente
        /// sempre que o componente <see cref="GraphicsView"/> precisa ser redesenhado,
        /// por exemplo, após chamar o método Invalidate().
        /// </summary>
        /// <param name="canvas">Objeto de desenho</param>
        /// <param name="dirtyRect">Área da tela a ser redesenhada</param>
        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            // Medidas totais da área do gráfico
            float width = dirtyRect.Width;
            float height = dirtyRect.Height;

            // Margens ao redor do gráfico para deixar espaço para escalas e rótulos
            float marginLeft = 70;
            float marginBottom = 30;
            float marginTop = 20;
            float marginRight = 20;

            // Área útil de desenho (sem as margens)
            float graphWidth = width - marginLeft - marginRight;
            float graphHeight = height - marginBottom - marginTop;

            // Se não houver dados suficientes para desenhar, sai do método
            if (Simulations.Count == 0 || Simulations.All(s => s.Count < 2))
                return;

            // Coleta todos os valores das simulações para calcular o intervalo global
            var allPoints = Simulations.SelectMany(s => s).ToList();
            double globalMax = allPoints.Max();
            double globalMin = allPoints.Min();
            if (globalMax == globalMin) globalMax += 1; // Evita divisão por zero

            // Cores diferentes para cada linha de simulação
            var colors = new[] { Colors.Blue, Colors.Red, Colors.Green, Colors.Orange, Colors.Purple };

            // -------------------------------
            // Desenho da escala Y (vertical)
            // -------------------------------
            int yDivisions = 5; // quantidade de linhas horizontais (divisões)
            for (int i = 0; i <= yDivisions; i++)
            {
                float y = marginTop + i * (graphHeight / yDivisions);
                double price = globalMax - (globalMax - globalMin) * i / yDivisions;

                // Linha horizontal (guia)
                canvas.StrokeColor = Colors.Black;
                canvas.StrokeSize = 1;
                canvas.DrawLine(marginLeft, y, width - marginRight, y);

                // Rótulo da escala Y
                canvas.FontColor = Colors.Black;
                canvas.DrawString(price.ToString("0.##"), 0, y - 10, marginLeft - 5, 20,
                    HorizontalAlignment.Right, VerticalAlignment.Center);
            }

            // -------------------------------
            // Desenho da escala X (horizontal)
            // -------------------------------
            int xDivisions = 5; // quantidade de linhas verticais (divisões)
            int numPoints = Simulations.First().Count;

            for (int i = 0; i <= xDivisions; i++)
            {
                float x = marginLeft + i * (graphWidth / xDivisions);
                int t = i * (numPoints / xDivisions);

                // Linha vertical (guia)
                canvas.StrokeColor = Colors.Black;
                canvas.StrokeSize = 1;
                canvas.DrawLine(x, marginTop, x, marginTop + graphHeight);

                // Rótulo da escala X (tempo)
                canvas.FontColor = Colors.Black;
                canvas.DrawString(t.ToString(), x - 10, marginTop + graphHeight + 2, 30, 20,
                    HorizontalAlignment.Center, VerticalAlignment.Top);
            }

            // -------------------------------
            // Desenho dos eixos principais
            // -------------------------------
            canvas.StrokeColor = Colors.Black;
            canvas.StrokeSize = 1.5f;

            // Eixo Y (linha vertical à esquerda)
            canvas.DrawLine(marginLeft, marginTop, marginLeft, marginTop + graphHeight);

            // Eixo X (linha horizontal na base)
            canvas.DrawLine(marginLeft, marginTop + graphHeight, width - marginRight, marginTop + graphHeight);

            // -------------------------------
            // Desenho das curvas de simulação
            // -------------------------------
            int colorIndex = 0;
            foreach (var line in Simulations)
            {
                if (line.Count < 2) continue;

                canvas.StrokeColor = colors[colorIndex % colors.Length];
                canvas.StrokeSize = 2;
                colorIndex++;

                // Distância horizontal entre os pontos
                float dx = graphWidth / (line.Count - 1);

                // Função para normalizar o valor (preço) entre 0 e altura do gráfico
                float Normalize(double val) =>
                    (float)((val - globalMin) / (globalMax - globalMin)) * graphHeight;

                // Desenha as linhas conectando os pontos da simulação
                for (int i = 0; i < line.Count - 1; i++)
                {
                    float x1 = marginLeft + i * dx;
                    float y1 = marginTop + graphHeight - Normalize(line[i]);
                    float x2 = marginLeft + (i + 1) * dx;
                    float y2 = marginTop + graphHeight - Normalize(line[i + 1]);

                    // Ignora valores inválidos (NaN ou infinito)
                    if (float.IsNaN(y1) || float.IsNaN(y2) || float.IsInfinity(y1) || float.IsInfinity(y2))
                        continue;

                    canvas.DrawLine(x1, y1, x2, y2);
                }
            }
        }
    }
}
