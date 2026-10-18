# HANDOUT — AULA 09

## Projete a Matriz de Acesso

*SisAcad — sistema acadêmico fictício — Arquitetura de Aplicações Web*

## 🎯 MISSÃO

Vocês são o time de segurança contratado para definir QUEM pode fazer O QUÊ no SisAcad, o novo sistema acadêmico da faculdade. Os papéis já existem: Aluno, Professor, Coordenador e Secretaria.

- Preencham a matriz abaixo com ✓ (pode) ou ✗ (não pode) para cada papel × ação
- Justifiquem por escrito os 3 casos que geraram mais discussão na dupla
- Respondam à pergunta-armadilha da Parte 03 — ela esconde o limite do modelo

*⏱️ Tempo: 25 minutos  |  👥 Formato: em duplas  |  Regra de ouro: na dúvida, NEGUE o acesso.*

> **Nomes:** Miguel Ribeiro Muniz Morais   **Turma:** Quinta-Feira Noite   **Data:** 15 /10/2026

## PARTE 01 — A matriz papel × permissão

Marquem ✓ (pode) ou ✗ (não pode) em cada célula:

| **Ação** | **Aluno** | **Professor** | **Coordenador** | **Secretaria** |
| --- | --- | --- | --- | --- |
| 1. Ver as próprias notas | ✓ | ✓ | ✓ | ✓ |
| 2. Ver notas de toda a turma | X | ✓ | ✓ | ✓ |
| 3. Lançar nota | X | ✓ | X | X |
| 4. Alterar nota já lançada | X | ✓ | X | X |
| 5. Matricular aluno em disciplina | X | X | ✓ | X |
| 6. Trancar matrícula | X | X | ✓ | X |
| 7. Criar nova disciplina | X | X | ✓ | X |
| 8. Ver dados pessoais de aluno | ✓ | X | ✓ | ✓ |
| 9. Emitir histórico escolar | ✓ | X | X | ✓ |
| 10. Aprovar colação de grau | X | X | ✓ | X |

*Dica: pensem em consequências — quem responde perante o MEC se uma nota for alterada indevidamente? E perante a LGPD se dados pessoais vazarem?*

## PARTE 02 — Os casos polêmicos

Quais 3 células geraram mais discussão na dupla? Anotem a célula (ação + papel) e a justificativa da decisão final:

1. Caso 1: Célula 5 Matricular aluno - Pois o coordenador tem ciência das matérias disponíveis para cada aluno.

2. Caso 2: Célula 8 Ver dados pessoais de aluno - O professor é o único que não deve ter acesso, pois não é necessário os dados pessoais do aluno para nenhuma ação.

3. Caso 3: Célula 10 Aprovar colação de grau - Apenas o coordenador consegue analisar a situação do aluno para saber se pode ou não colar grau.

## PARTE 03 — A pergunta-armadilha

Vocês marcaram que Professor pode “lançar nota”. Mas… de QUALQUER turma? O professor de Cálculo pode lançar nota na turma de Arquitetura de Aplicações Web?

1. O papel “Professor” sozinho é suficiente para decidir esse acesso? Por quê?
Não, pois cada professor é responsável por uma ou mais matérias específicas.

2. Que informação ALÉM do papel o sistema precisa conhecer para decidir corretamente?
O ID do professor no sistema, fazendo com que suas turmas sejam vinculadas ao seu ID.

3. Onde vocês já viram esse mesmo problema nesta disciplina? (dica: Aula 07…)
No problema Mass Assigment, onde o usuário conseguia se dar a permissão de Admin por exemplo.

## DESAFIO

1. Chegou um pedido da coordenação: criar o papel “Monitor” — um aluno que ajuda o professor em uma disciplina. Que permissões vocês dariam ao Monitor? Que risco novo esse papel cria?
Acredito que apenas a permissão para ver a nota da turma. Para evitar risco de alterações indevidas das notas.