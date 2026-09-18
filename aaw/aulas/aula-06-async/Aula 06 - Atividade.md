# Atividade — AULA 06

## Síncrono ou Assíncrono?

*Análise de fluxos de comunicação entre serviços — Arquitetura de Aplicações Web*

## 🎯 MISSÃO

Vocês são os arquitetos dos 4 fluxos abaixo. Para CADA cenário:

- Decidam o estilo de comunicação: síncrono (request/response), assíncrono (fila/evento) ou API Gateway/BFF
- Desenhem o fluxo com caixas (serviços) e setas (chamadas/mensagens) no espaço indicado
- Justifiquem com pelo menos 2 fatores (urgência da resposta, tolerância a atraso, picos, falhas...)
- Apontem o principal risco da escolha de vocês

*⏱️ Tempo: 25 minutos  |  👥 Formato: em duplas  |  Não existe resposta única — o que vale é a justificativa.*

> **Nomes:** Miguel Ribeiro Muniz Morais   **Turma:** Quinta-feira noite   **Data:** 10/ 09 / 2026

## CENÁRIO 01 — PagFácil — aprovar ou negar AGORA

No checkout do PagFácil, ao clicar em “Pagar”, o serviço de Pagamentos precisa consultar o saldo/limite do cliente no serviço de Contas — e a resposta define se a venda acontece neste exato momento.

- O cliente está na tela, esperando o resultado da compra
- Sem a resposta de Contas, não há decisão possível: aprovar às cegas é proibido
- Tempo de resposta do serviço de Contas: ~80 ms em condições normais

**Sua análise:**

1. Estilo recomendado:   [X] Síncrono      ☐ Assíncrono (fila/evento)      ☐ API Gateway/BFF

2. Desenhe o fluxo (caixas = serviços, setas = chamadas/mensagens):

Cliente -> Pagamentos -> consulta saldo/limite -> consulta responde -> Pagamentos aprova/nega 

A aplicação aguarda o retorno do saldo para continuar qualquer processo (request/response).

3. Justificativa (mínimo 2 fatores):

Urgência da resposta: o cliente está na tela e a decisão de aprovar/negar depende do saldo neste momento.
Correção obrigatória: aprovar às cegas é proibido, então não dá para seguir sem a resposta definitiva.

4. Principal risco da escolha:

Se o serviço de Contas cair ou ficar lento, o Pagamentos trava junto. O usuário fica preso na tela até a resposta chegar.


## CENÁRIO 02 — CadastraJá — o e-mail de boas-vindas

Após criar a conta no CadastraJá, o sistema envia um e-mail de boas-vindas. O provedor de e-mail às vezes demora 8 segundos para responder e falha em 2% das tentativas.

- O usuário quer começar a usar o app imediatamente após o cadastro
- O e-mail chegar 1 minuto depois não incomoda ninguém
- Se o provedor falhar, o envio deve ser tentado de novo — sem o usuário perceber

**Sua análise:**

1. Estilo recomendado:   ☐ Síncrono      [X] Assíncrono (fila/evento)      ☐ API Gateway/BFF

2. Desenhe o fluxo (caixas = serviços, setas = chamadas/mensagens):

Usuário -> Cadastro (cria conta) -> responde na hora
                                 -> evento "UsuárioCriado" na fila
fila -> Provedor de E-mail (se falhar -> tenta de novo)

3. Justificativa (mínimo 2 fatores):

O e-mail chegar 1 minuto depois não atrapalha.
Na fila, se falhar tenta de novo sem o usuário perceber e sem travar o cadastro. Se fosse síncrono, o usuário esperaria os 8 segundos e o cadastro poderia falhar por causa do e-mail.

4. Principal risco da escolha:

O usuário já está usando o app mas o e-mail ainda não chegou. Se a fila for só em memória, some as mensagens se cair.

## CENÁRIO 03 — MegaMarket — baixa de estoque nos picos

No marketplace MegaMarket, cada venda gera uma baixa no serviço de Estoque. Nas grandes promoções o tráfego sobe 10x e o Estoque não dá conta de responder na velocidade das vendas.

- Atraso de alguns segundos na baixa é aceitável
- PERDER uma baixa de estoque não é aceitável (gera venda sem produto)
- O checkout não pode ficar lento nem cair porque o Estoque está sobrecarregado

**Sua análise:**

1. Estilo recomendado:   ☐ Síncrono      [X] Assíncrono (fila/evento)      ☐ API Gateway/BFF

2. Desenhe o fluxo (caixas = serviços, setas = chamadas/mensagens):

Venda -> Checkout -> evento "VendaRealizada" na fila -> resposta (não espera o Estoque)
fila -> Serviço Estoque -> baixa o estoque no seu tempo

3. Justificativa (mínimo 2 fatores):

O checkout coloca na fila e o Estoque processa, sem derrubar a venda.
O broker persiste a mensagem. Se o Estoque cair, as mensagens esperam na fila.

4. Principal risco da escolha:

O estoque exibido pode estar atrasado.

## CENÁRIO 04 — AppBanco — uma tela, cinco serviços

A tela inicial do AppBanco mostra saldo, fatura do cartão, investimentos, empréstimos e cashback — dados de 5 serviços diferentes. O time mobile reclama: são 5 chamadas, 5 formatos de resposta e 5 pontos de falha em cada abertura do app.

- A tela precisa abrir rápido, inclusive em redes móveis ruins
- Cada serviço tem equipe, formato e autenticação próprios
- Amanhã nasce a versão web, que precisa de MAIS dados que a mobile

**Sua análise:**

1. Estilo recomendado:   ☐ Síncrono      ☐ Assíncrono (fila/evento)      [X] API Gateway/BFF

2. Desenhe o fluxo (caixas = serviços, setas = chamadas/mensagens):

App mobile -> API Gateway -> Saldo
App web -> API Gateway -> Cartão
                             -> Empréstimos
                             -> Cashback
                             -> Investimentos
O Gateway conversa com os 5 serviços, junta tudo e devolve em um formato.

3. Justificativa (mínimo 2 fatores):

em vez de 5 chamadas, 5 formatos e 5 pontos de falha no mobile, o Gateway centraliza.

4. Principal risco da escolha:

O Gateway vira ponto único de falha / gargalo: se ele cai, a tela inteira cai.

## DESAFIO

1. Escolha um cenário em que vocês indicaram ASSÍNCRONO. Os brokers de mensagens costumam garantir entrega “pelo menos uma vez” — ou seja, a MESMA mensagem pode chegar duas vezes. O que aconteceria no seu fluxo? Como o consumidor deveria se proteger?

Cenário escolhido: 03 — MegaMarket (baixa de estoque).

Se o evento "VendaRealizada" chega duas vezes, o serviço de Estoque baixaria o estoque duas vezes para uma única venda. O estoque fica errado. No cenário 02, o efeito seria o cliente receber dois e-mails de boas-vindas.
O consumidor se protege com idempotência: processar a mesma mensagem 1x ou 5x tem que dar o mesmo resultado.
