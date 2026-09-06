# HANDOUT — AULA 05

## Escolha o Banco

*Persistência em arquiteturas distribuídas — Arquitetura de Aplicações Web*

## 🎯 MISSÃO

Vocês são o time de arquitetura de dados contratado pelas 4 empresas abaixo. Para CADA cenário:

- Escolham o modelo de banco: relacional, documento, chave-valor ou grafo
- Justifiquem com pelo menos 2 fatores do contexto (estrutura dos dados, padrão de acesso, escala, consistência...)
- Apontem o principal risco da escolha de vocês

*⏱️ Tempo: 25 minutos  |  👥 Formato: em duplas  |  Não existe resposta única — o que vale é a justificativa.*

> **Nomes:** Miguel Ribeiro Muniz Morais   **Turma:** Quinta-feira noite   **Data:** 03/09/2026

## CENÁRIO 01 — TechStore — o catálogo camaleão

E-commerce com 80 mil produtos. Cada categoria tem atributos completamente diferentes: livro tem autor e número de páginas; notebook tem RAM e CPU; camiseta tem tamanho e cor.

- A cada categoria nova, o time faz ALTER TABLE e a tabela produtos já tem 92 colunas (a maioria NULL)
- O produto é quase sempre lido INTEIRO, de uma vez, para montar a página
- Novos atributos surgem toda semana — o marketing não espera o DBA
- Relatórios cruzando categorias são raros

**Sua análise:**

1. Modelo recomendado:   ☐ Relacional     [X] Documento     ☐ Chave-valor     ☐ Grafo

2. Justificativa (mínimo 2 fatores do contexto):
Flexibilidade para inserção ou alteração de dados sem precisar interromper o funcionamento da aplicação para fazer tal ação. Outro fator seria para evitar a alta latência, caso seja um sistema que atende várias regiões do mundo, para se comunicar com outro servidor.

3. Principal risco da escolha:
Observando o BASE pode se considerar o "E": Eventually Consistent. Você pode precisar consultar um dado que ainda não está 100% atualizado, obtendo uma resposta inconsistente naquele momento.

## CENÁRIO 02 — MegaCart — o carrinho da Black Friday

Serviço de carrinho de compras de um varejista gigante. Na Black Friday são milhões de leituras e escritas por minuto.

- O acesso é SEMPRE pela chave: “carrinho do cliente 12345” — nunca por busca ou filtro
- Todo carrinho expira automaticamente em 48h (TTL)
- Latência precisa ser de poucos milissegundos
- Perder um carrinho é chato, mas NÃO é tragédia — o cliente remonta

**Sua análise:**

1. Modelo recomendado:   ☐ Relacional     [] Documento     [X] Chave-valor     ☐ Grafo

2. Justificativa (mínimo 2 fatores do contexto):
O sistema precisa ter baixa latência (acesso por chave em milissegundo); Carrinho com as informações temporárias. 

3. Principal risco da escolha:
Não ter os dados do carrinho salvos diretamente no banco após sua expiração.

## CENÁRIO 03 — PayBank — dinheiro não pode evaporar

Módulo de transferências de um banco. Uma transferência debita uma conta e credita outra — as duas operações têm que acontecer JUNTAS ou nenhuma acontece.

- Consistência forte exigida por lei — saldo errado é multa do Banco Central
- Auditoria cruza contas, clientes, agências e transações em relatórios complexos (joins)
- O esquema dos dados é estável há 10 anos
- Volume alto, mas previsível

**Sua análise:**

1. Modelo recomendado:   [X] Relacional     ☐ Documento     ☐ Chave-valor     ☐ Grafo

2. Justificativa (mínimo 2 fatores do contexto):
Consistência dos dados "É isso ou não é". Poder consultar toda e qualquer transação sem mudanças em seus estados.
3. Principal risco da escolha:
Sistema mais "pesado"/lento pela quantidade de tráfego de dados.

## CENÁRIO 04 — FriendLink — amigos dos seus amigos

Rede social profissional em que o produto principal é a indicação: “pessoas que você talvez conheça” e “quem pode te apresentar à empresa X”.

- As consultas dominantes percorrem RELACIONAMENTOS: amigos dos amigos, caminhos de indicação com até 6 níveis
- Em banco relacional, cada nível vira um self-join — com 6 níveis a consulta já não responde
- Os dados de perfil são simples; o valor está nas CONEXÕES
- O grafo cresce milhões de arestas por dia

**Sua análise:**

1. Modelo recomendado:   ☐ Relacional     ☐ Documento     ☐ Chave-valor     [X] Grafo

2. Justificativa (mínimo 2 fatores do contexto):
Justamente por percorrer relacionamentos entre usuários em comum. Devido ao grafo crescer conforme usuário se conecta com mais pessoas, gerando mais possibilidades de conexões.

3. Principal risco da escolha:
Consulta superficial dos dados.

## DESAFIO

1. Escolha um dos cenários e responda: se a rede particionar (metade dos servidores não enxerga a outra metade), o que o sistema deve fazer — parar de responder para não errar, ou continuar respondendo mesmo arriscando dados desatualizados? Qual letra do CAP vocês sacrificariam e por quê?
Sacrificaria o "P", pois é melhor que o sistema fique fora do ar por um momento de instabilidade, fazendo com que os usuários não fiquem sujeitos a enviarem dados que ficarão inconsistentes no sistema.