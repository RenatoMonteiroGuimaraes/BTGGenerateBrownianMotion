## Fundamento Matemático

O gráfico gerado representa uma simulação de **Movimento Browniano Geométrico** (também conhecido como *Geometric Brownian Motion*), 
um modelo amplamente utilizado em finanças para representar a evolução de preços de ativos ao longo do tempo.

A fórmula base utilizada é:
Sₜ = Sₜ₋₁ × exp(μ + σZ)


Onde:
- **Sₜ**: preço no tempo t
- **μ**: média de retorno (drift)
- **σ**: volatilidade do ativo
- **Z**: número aleatório de distribuição normal padrão (ruído branco)
- **exp**: função exponencial

Cada ponto da curva representa a evolução do preço após a aplicação dessa fórmula, acumulando o resultado ao longo dos "dias" (tempo simulado).

> A versão usada foi fornecida pelo BTG Pactual e adaptada para manter coerência visual no gráfico.

## Interpretação Gráfica

No gráfico:

- O **eixo X** representa o tempo (dias simulados).
- O **eixo Y** representa o valor do ativo ao longo da simulação.
- Cada **linha colorida** é uma simulação distinta, iniciando no mesmo preço inicial.
- O gráfico exibe o efeito acumulado da aleatoriedade e da tendência (drift) sobre o preço.

Esse tipo de visualização ajuda a entender a incerteza nos preços ao longo do tempo, especialmente em mercados voláteis.

