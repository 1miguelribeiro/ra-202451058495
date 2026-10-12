# Proposta — Trabalho Semestral · Arquitetura de Aplicações Web 2026.2

- **Aluno:** Miguel Ribeiro Muniz Morais · **Curso/Turma:** Sistemas de Informação / <turma>
- **Repositório:** https://github.com/1miguelribeiro/ra-202451058495

## 1. Domínio e problema

Controle de estoque de uma loja de jogos retrô.
Hoje o dono anota em planilha quantos cartuchos e CDs tem de cada jogo.
A aplicação registra os jogos e cada compra e venda do estoque, e não deixa vender o que não tem.

## 2. Entidades

Entidade A: **Jogo** — nome, console, ano de lançamento, preço, quantidade em estoque.

Entidade B: **Movimentacao** — jogo, tipo (entrada ou saída), quantidade, data, motivo.

Relacionamento: um jogo tem várias movimentações (1-N).
No MongoDB, jogos e movimentações ficam em coleções separadas, e cada movimentação guarda o id do jogo (referência).
Escolhi referência, pois um jogo pode ter muitas movimentações ao longo do tempo, se ficassem dentro do jogo, o documento do jogo só ia crescer.

## 3. Endpoints previstos

Jogos:
- `GET /api/jogos` — lista todos os jogos
- `GET /api/jogos/{id}` — mostra um jogo
- `POST /api/jogos` — cadastra um jogo
- `PUT /api/jogos/{id}` — altera um jogo
- `DELETE /api/jogos/{id}` — apaga um jogo

Movimentações:
- `GET /api/movimentacoes` — lista todas as movimentações
- `GET /api/movimentacoes/{id}` — mostra uma movimentação
- `POST /api/movimentacoes` — registra uma entrada ou saída e atualiza a quantidade do jogo
- `PUT /api/movimentacoes/{id}` — corrige uma movimentação
- `DELETE /api/movimentacoes/{id}` — apaga uma movimentação

Login:
- `POST /api/auth/registro` — cria um usuário
- `POST /api/auth/login` — faz login e devolve o token

## 4. Regra de negócio

- Entrada soma na quantidade do jogo; saída subtrai.
- Não pode vender mais do que tem: a saída só é aceita se a quantidade em estoque for suficiente.
- A quantidade de uma movimentação tem que ser maior que zero.

## 5. Casos de erro

- Buscar, alterar ou apagar um jogo que não existe → **404 Not Found**.
- Registrar uma saída maior que o estoque (ex.: vender 3 e só ter 1) → **400 Bad Request** com a mensagem "Estoque insuficiente".

## 6. Stack escolhida

- Backend: .NET 10 (C#)
- Banco: MongoDB, rodando no Docker
- Frontend: HTML + JavaScript (fetch)

## 7. Segurança e testes

- **JWT:** o usuário faz login e recebe um token que vale por 1 hora. Para cadastrar, alterar ou apagar, precisa mandar o token.
- **Perfis:** `admin` e `funcionario`. O funcionário consulta e registra entradas e saídas; só o admin pode alterar e apagar jogos e movimentações.
- **Testes:**
  - sucesso: entrada de 5 unidades aumenta o estoque;
  - sucesso: saída de 2 unidades diminui o estoque;
  - erro: saída maior que o estoque é recusada;
  - erro: movimentação com quantidade zero é recusada.
