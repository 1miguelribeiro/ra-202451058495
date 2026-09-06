**API:** PetShopApi (http://localhost:5302)

Redesenho dos endpoints:

GET /api/v1/pets
Devolve só 50 pets num limite fixo escondido no código, sem paginação.
Correção: GET /api/v1/pets?page=1&size=20

DELETE /api/v1/pet/999999
Está no singular e retorna 200 mesmo quando o pet não existe. Devia dar 404.
Correção: DELETE /api/v1/pets/{id} -> 204 se apagou, 404 se não existe.

GET /api/v1/pet/1
Funciona, mas a rota está no singular "pet".
Correção: GET /api/v1/pets/{id}.

GET /api/v1/tutores-vip
O "vip" seria filtro, não um recurso.
Correção: GET /api/v1/tutores?vip=true

POST /api/v1/pets
Cria o pet mas responde 200 e sem header Location.
Correção: 201 + header Location: /api/v1/pets/{id}.

GET /api/v1/pets/999999
Pet que não existe volta 200 com "Pet nao encontrado". Erro com
status de sucesso.
Correção: 404 Not Found.

GET /api/v1/petshops/1/clientes/1/pets/1/consultas/1/exames/1
Aninhamento extenso, sendo que exame já tem id próprio.
Correção: GET /api/v1/exames/{id}

GET /api/v1/consultas
Devolve as 6.000 consultas de uma vez, sem paginação.
Correção: GET /api/v1/consultas?page=1&size=20

PUT /api/v1/pets/1/vacinas
Usa PUT pra adicionar vacina, e cada chamada cria uma nova. PUT devia ser
idempotente. Adicionar item precisa ser POST.
Correção: POST /api/v1/pets/{id}/vacinas e retorna 201
