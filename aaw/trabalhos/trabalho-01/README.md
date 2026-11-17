# Loja Retrô Controle de Estoque

**Aluno:** Miguel Ribeiro Muniz Morais
**Curso:** Sistemas de Informação
**Turma:** Quinta-feira noite
**Disciplina:** Arquitetura de Aplicações Web

## Sobre o projeto

Sistema de controle de estoque de uma loja de jogos retrô.
A loja cadastra os jogos e registra cada entrada e cada venda.
A quantidade em estoque é atualizada automaticamente, e o sistema não deixa vender mais do que existe.

São duas entidades com relação 1-N:

Jogo: nome, console, ano de lançamento, preço e quantidade em estoque.
Movimentacao: jogo, tipo (Entrada ou Saida), quantidade, data e motivo.

No MongoDB, jogos e movimentações ficam em coleções separadas e cada movimentação guarda o id do jogo.
Escolhi referência porque um jogo acumula muitas movimentações com o tempo, se ficassem dentro do jogo, o documento só ia crescer.

### Regras de negócio

Entrada soma no estoque, saída subtrai.
Saída maior que o estoque é recusada com "Estoque insuficiente".
A quantidade tem que ser maior que zero.
Corrigir ou remover uma movimentação recalcula o estoque, e o estoque não pode ficar negativo.
Um jogo que tem movimentações não pode ser apagado, preservando histórico.

## Tecnologias

Backend: ASP.NET Core Web API (.NET 10, C#)
Banco: MongoDB 8 rodando no Docker (docker compose)
Frontend: HTML, CSS e JavaScript
Segurança: JWT + perfis, senha guardada com hash
Documentação: Swagger
Testes: xUnit, Moq e FluentAssertions

## Pré-requisitos

.NET SDK 10
Docker Desktop
Git

## Como executar

Todos os comandos são rodados dentro da pasta aaw/trabalhos/trabalho-01.

**1. Clonar o repositório e entrar na pasta do projeto**

```bash
git clone https://github.com/1miguelribeiro/ra-202451058495.git
cd ra-202451058495/aaw/trabalhos/trabalho-01
```

**2. Criar o arquivo .env a partir do modelo**

```bash
cp .env.example .env
```

PowerShell: copy .env.example .env

Abra o .env e troque os valores de exemplo (senha do banco, chave do JWT e senha do admin).
A senha em MONGO_PASSWORD precisa ser a mesma que aparece dentro de MONGODB__CONNECTIONSTRING.

**3. Subir o MongoDB**

```bash
docker compose up -d
```

**4. Rodar a API**

```bash
dotnet run --project LojaRetro.Api --launch-profile http
```

**5. Abrir no navegador**

Página web: http://localhost:5073
Swagger: http://localhost:5073/swagger

Na primeira vez que a API sobe, ela cria o usuário admin com o e-mail e a senha definidos no .env.

## Variáveis de ambiente

Ficam no arquivo .env. O modelo está em .env.example.

MONGO_USER | Usuário root do MongoDB (usado pelo docker compose) 
MONGO_PASSWORD | Senha do MongoDB (usada pelo docker compose)
MONGODB__CONNECTIONSTRING | Endereço do banco usado pela API
MONGODB__DATABASE | Nome do banco 
JWT__KEY | Chave que assina os tokens
ADMIN__EMAIL | E-mail do admin criado automaticamente 
ADMIN__SENHA | Senha do admin criado automaticamente 

Emissor, público e validade do token ficam em LojaRetro.Api/appsettings.json.

## Swagger

http://localhost:5073/swagger. Todos os endpoints têm descrição, parâmetros, exemplos e os códigos de resposta.

Para testar os endpoints protegidos:

Em POST /api/auth/login, entre com o e-mail e a senha do admin (do .env).
Copie o valor de token da resposta.
Clique em Authorize, cole o token e confirme.

## Página web

Em http://localhost:5073 a página consome a API com fetch, sem recarregar o navegador.

Lista de jogos com o estoque de cada um.
Detalhe do jogo com o histórico de entradas e saídas.
Formulário para registrar entrada ou saída.
Cadastro e edição de jogos (somente admin).
Login e criação de conta.

## Perfis e permissões

Quem cria conta pela página ou por POST /api/auth/registro recebe sempre o perfil funcionario.
O único admin é o criado pelo .env. A permissão é conferida na API, pelo perfil gravado dentro do token.

| Ação | Sem login | funcionario | admin |
| Consultar jogos e movimentações | ✓ | ✓ | ✓ |
| Registrar entrada ou saída | 401 | ✓ | ✓ |
| Cadastrar, alterar ou apagar jogo | 401 | 403 | ✓ |
| Corrigir ou remover movimentação | 401 | 403 | ✓ |

## Endpoints


POST /api/auth/registro: Cria um usuário funcionario 
POST /api/auth/login: Faz login e devolve o token JWT 
GET /api/jogos: Lista os jogos 
GET /api/jogos/{id}: Mostra um jogo 
POST /api/jogos: Cadastra um jogo
PUT /api/jogos/{id}: Altera um jogo
DELETE /api/jogos/{id}: Apaga um jogo sem movimentações
GET /api/movimentacoes?jogoId=: Lista as movimentações
GET api/movimentacoes/{id}: Mostra uma movimentação
POST /api/movimentacoes: Registra entrada ou saída e atualiza o estoque
PUT /api/movimentacoes/{id}: Corrige uma movimentação e recalcula o estoque
DELETE /api/movimentacoes/{id}: Remove uma movimentação e desfaz o efeito no estoque

### Códigos de resposta

| Código | Quando |

| 200 / 201 / 204 | Sucesso |
| 400 | Dados inválidos, id em formato errado ou estoque insuficiente |
| 401 | Sem token, token inválido ou expirado, ou login errado |
| 403 | O perfil não tem permissão |
| 404 | Jogo ou movimentação não encontrado |
| 409 | E-mail já cadastrado, ou jogo com movimentações |
| 500 | Erro inesperado: a API devolve uma mensagem genérica, sem detalhes internos |

## Testes

Os testes unitários ficam em LojaRetro.Tests e cobrem as regras do EstoqueService.
O banco é simulado com Moq.

```bash
dotnet test
```

Cenários testados:

Sucesso: entrada de 5 unidades aumenta o estoque.
Sucesso: saída de 2 unidades diminui o estoque.
Sucesso: saída igual ao estoque deixa o estoque em zero.
Erro: saída maior que o estoque é recusada e nada é gravado.
Erro: quantidade zero ou negativa é recusada.
Erro: movimentação de jogo inexistente é recusada.
Erro: remover uma entrada já vendida é recusado (o estoque ficaria negativo).

## Limitações conhecidas

Registrar uma movimentação faz duas gravações separadas no MongoDB (atualiza o jogo e cria a movimentação), sem transação. Se o banco cair entre as duas, os dados podem ficar inconsistentes.

## Uso de IA

Ferramentas usadas: Claude.
Onde ajudou: montou o roteiro do trabalho, críticas, gerou a primeira versão dos controllers, do EstoqueService, do login com JWT, da página web e dos testes, e explicou cada parte (ObjectId, injeção de dependência, estorno do estoque, mocks).
O que eu revisei/reescrevi: simplifiquei a proposta e o visual da página, validação dos campos, correção de mensagens de erro presas entre telas, ajuste de textos e mensagens da API.
