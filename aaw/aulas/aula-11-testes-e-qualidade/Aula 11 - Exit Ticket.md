Exit Ticket

Tabela de casos (CalcularTotal)

| Subtotal | Desconto | Frete | Total esperado |
| 0,00 | 0,00 | 25,00 | 25,00 |
| 100,00 | 0,00 | 25,00 | 125,00 |
| 199,99 | 0,00 | 25,00 | 224,99 |
| 200,00 | 10,00 | 25,00 | 215,00 |
| 299,99 | 15,00 | 25,00 | 309,99 |
| 300,00 | 15,00 | 0,00 | 285,00 |
| 499,99 | 25,00 | 0,00 | 474,99 |
| 500,00 | 50,00 | 0,00 | 450,00 |
| 1000,00 | 100,00 | 0,00 | 900,00 |


O caso de subtotal = 500,00 revelou o bug. O esperado era total 450,00, mas o teste retornou 475, ou seja, só aplicou 5% de desconto.
No PedidoService.CalcularDesconto a regra diz 10% a partir de R$500, mas o código usava subtotal > 500m. Fazendo o valor exato de 500 caía em 5%.
Correção: trocar para subtotal >= 500m.

