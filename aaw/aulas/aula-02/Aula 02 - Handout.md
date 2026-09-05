# HANDOUT — AULA 02

## Dissecando o HTTP

*6 requisições sob o microscópio — Arquitetura de Aplicações Web*

## 🎯 MISSÃO

Vocês interceptaram 6 conversas entre um app e a API de uma biblioteca. Para CADA card:

- Descrevam o que o cliente pediu (verbo + recurso na URI)
- Expliquem o que o status code da resposta informa
- Respondam: repetindo a MESMA requisição 3 vezes seguidas, o estado do servidor muda?

Ao final, preencham juntos a TABELA-SÍNTESE dos verbos na última página.

*⏱️ Tempo: 30 minutos  |  👥 Formato: em duplas  |  Dica: o card 6 esconde uma pegadinha de quem é a culpa.*

> **Nomes:** Miguel Ribeiro   **Turma:** Quinta-feira noite  **Data:** 13 / 08 / 2026

## REQUISIÇÃO 01 — A prateleira inteira

```text
→ REQUISIÇÃO
GET /api/livros HTTP/1.1
Host: biblioteca.newton.br
Accept: application/json
```

```text
← RESPOSTA
HTTP/1.1 200 OK
Content-Type: application/json

[ { "id": 1, "titulo": "Clean Code", "autor": "Robert C. Martin" },
  { "id": 7, "titulo": "O Programador Pragmático", "autor": "Hunt & Thomas" } ]
```

**Sua análise:**

1. O que o cliente pediu (verbo + recurso)?
Ele fez request do tipo GET, recurso "/api/livros"
2. O que o status code informa? Deu certo? Culpa de quem se não deu?
200 significa sucesso. Se esse get desse errado, seria erro do servidor (5xx) ou o usuário deixou de dar alguma informação ao realizar a requisição.
3. Repetindo esta requisição 3 vezes seguidas, o estado do servidor muda? E a resposta?
Não muda, só está fazendo uma requisição de leitura.

## REQUISIÇÃO 02 — O livro fantasma

```text
→ REQUISIÇÃO
GET /api/livros/99 HTTP/1.1
Host: biblioteca.newton.br
Accept: application/json
```

```text
← RESPOSTA
HTTP/1.1 404 Not Found
Content-Type: application/problem+json

{ "title": "Not Found", "status": 404 }
```

**Sua análise:**

1. O que o cliente pediu (verbo + recurso)?
Requisição do tipo GET, "api/livros/99"
2. O que o status code informa? Deu certo? Culpa de quem se não deu?
404 Not Found, não deu certo. Usuário procurou um livro que não está cadastrado / não existem 99 livros cadastrados
3. Repetindo esta requisição 3 vezes seguidas, o estado do servidor muda? E a resposta?
Nada muda.


## REQUISIÇÃO 03 — Livro novo na estante

```text
→ REQUISIÇÃO
POST /api/livros HTTP/1.1
Host: biblioteca.newton.br
Content-Type: application/json

{ "id": 8, "titulo": "Domain-Driven Design", "autor": "Eric Evans" }
```

```text
← RESPOSTA
HTTP/1.1 201 Created
Location: /api/livros/8
Content-Type: application/json

{ "id": 8, "titulo": "Domain-Driven Design", "autor": "Eric Evans" }
```

**Sua análise:**

1. O que o cliente pediu (verbo + recurso)?
O cliente fez um POST em "/api/livros"
2. O que o status code informa? Deu certo? Culpa de quem se não deu?
201 que significa criado com sucesso. Se deu errado o usuário pode ter inserido algum parâmetro errado ao fazer o post ou não possui permissão para inserir algo na tabela livros.
3. Enviando este POST 3 vezes seguidas, o que acontece na estante? Para que serve o header Location?
A primeira vez irá inserir, já a segunda e terceira não irão permitir. Mostra em que ID foi inserida a informação, indicando onde o livro inserido pode ser visualizado.

## REQUISIÇÃO 04 — Corrigindo a ficha completa

```text
→ REQUISIÇÃO
PUT /api/livros/7 HTTP/1.1
Host: biblioteca.newton.br
Content-Type: application/json

{ "id": 7, "titulo": "O Programador Pragmático", "autor": "D. Hunt; D. Thomas" }
```

```text
← RESPOSTA
HTTP/1.1 200 OK
Content-Type: application/json

{ "id": 7, "titulo": "O Programador Pragmático", "autor": "D. Hunt; D. Thomas" }
```

**Sua análise:**

1. O que o cliente pediu (verbo + recurso)?
Ele fez um PUT em "/api/livros/7" (Um update no id 7).
2. O que o status code informa? Deu certo? Culpa de quem se não deu?
200, sucesso. Se deu errado o usuário pode ter inserido algum parâmetro errado ao fazer o put ou não possui permissão para mudar algo na tabela dos livros.
3. Repetindo esta requisição 3 vezes seguidas, o estado do servidor muda? E a resposta?
Nada muda, está enviando as mesmas informações.
## REQUISIÇÃO 05 — Fora do catálogo

```text
→ REQUISIÇÃO
DELETE /api/livros/7 HTTP/1.1
Host: biblioteca.newton.br
```

```text
← RESPOSTA
HTTP/1.1 204 No Content
```

**Sua análise:**

1. O que o cliente pediu (verbo + recurso)?
Ele fez um DELETE em "/api/livros/7" (Deletou uma linha da tabela/removeu um livro).
2. O que o status code informa? Deu certo? Culpa de quem se não deu?
Status 204, Delete feito com sucesso. Se deu errado o usuário pode ter inserido algum parâmetro errado ao fazer a requisição ou não possui permissão para deletar algo na tabela dos livros.
3. Repetindo o DELETE, o estado do servidor muda? Que resposta você ESPERA na segunda vez?
Não muda, pois deletou o ID 7. 404 Not Found.

## REQUISIÇÃO 06 — O cadastro capenga

```text
→ REQUISIÇÃO
POST /api/livros HTTP/1.1
Host: biblioteca.newton.br
Content-Type: application/json

{ "autor": "Anônimo" }
```

```text
← RESPOSTA
HTTP/1.1 400 Bad Request
Content-Type: application/problem+json

{ "title": "Bad Request", "status": 400,
  "errors": { "Titulo": [ "O campo Titulo é obrigatório" ] } }
```

**Sua análise:**

1. O que o cliente pediu (verbo + recurso)?
Ele tentou fazer um POST em "api/livros".
2. O que o status code informa? Deu certo? Culpa de quem se não deu?
400 Bad Request, deu errado. Usuário não informou todos os parâmetros necessários para executar o POST.
3. Repetindo esta requisição 3 vezes seguidas, o estado do servidor muda? E a resposta?
Não, irá continuar alertando que o request está errado.

## TABELA-SÍNTESE — Os verbos do HTTP

*Preencham com base nos 6 cards. “Seguro” = não altera nada no servidor. “Idempotente” = repetir N vezes deixa o servidor no mesmo estado que 1 vez.*

| **Verbo** | **Para que serve** | **Seguro?** | **Idempotente?** | **Status típicos** |

| **`GET`** |  Consulta informações sem alterar nada no servidor | Sim | Sim | 200 / 404 |
| **`POST`** | Cria um novo recurso com os dados enviados pelo cliente | Nâo | Não | 201 / 400 |
| **`PUT`**  | Substitui um recurso existente ou pode criar | Não | Sim | 200 / 201 |
| **`PATCH`** | Atualiza dados especificos de um recurso que já existe | Não | Não | 200 / 404 |
| **`DELETE`** | Remove um recurso existente | Não | Sim | 204 / 404 |

## DESAFIO

1. O verbo PATCH não apareceu em nenhum card. Qual a diferença entre PATCH e PUT? Um app de banco quer alterar SÓ o apelido do usuário, entre dezenas de campos do perfil — qual dos dois você usaria e por quê?

Patch você pode alterar apenas um atributo específico sem precisar enviar todos os outros atributos para o request funcionar.
Put você precisa enviar todos os atributos como parâmetros para o request funcionar.
Utilizaria PATCH nesse caso, pois quero alterar apenas o apelido do usuário.
