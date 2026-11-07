const conteudo = document.getElementById("conteudo");
const caixaMensagem = document.getElementById("mensagem");

async function chamarApi(caminho, metodo = "GET", corpo = null) {
    const opcoes = { method: metodo, headers: { "Content-Type": "application/json" } };
    if (corpo) {
        opcoes.body = JSON.stringify(corpo);
    }

    const resposta = await fetch("/api" + caminho, opcoes);
    if (resposta.status === 204) {
        return null;
    }

    const dados = await resposta.json();
    if (!resposta.ok) {
        throw new Error(textoDoErro(dados));
    }
    return dados;
}

function textoDoErro(dados) {
    if (dados.mensagem) {
        return dados.mensagem;
    }
    if (dados.errors) {
        return Object.values(dados.errors).flat().join(" ");
    }
    return "Erro inesperado.";
}

function mostrarMensagem(texto, tipo) {
    caixaMensagem.textContent = texto;
    caixaMensagem.className = "mensagem " + tipo;
    window.scrollTo(0, 0);
}

function limparMensagem() {
    caixaMensagem.className = "mensagem escondido";
}

function escapar(texto) {
    const div = document.createElement("div");
    div.textContent = texto;
    return div.innerHTML;
}

function formatarPreco(valor) {
    return valor.toLocaleString("pt-BR", { style: "currency", currency: "BRL" });
}

async function mostrarLista() {
    limparMensagem();

    try {
        const jogos = await chamarApi("/jogos");

        const linhas = jogos.map(jogo => `
            <tr>
                <td>${escapar(jogo.nome)}</td>
                <td>${escapar(jogo.console)}</td>
                <td>${jogo.anoLancamento}</td>
                <td>${formatarPreco(jogo.preco)}</td>
                <td>${jogo.quantidadeEmEstoque}</td>
                <td><button onclick="mostrarDetalhe('${jogo.id}')">Ver</button></td>
            </tr>`).join("");

        conteudo.innerHTML = `
            <h2>Jogos</h2>
            <table>
                <thead>
                    <tr><th>Nome</th><th>Console</th><th>Ano</th><th>Preço</th><th>Estoque</th><th></th></tr>
                </thead>
                <tbody>${linhas || "<tr><td colspan='6'>Nenhum jogo cadastrado.</td></tr>"}</tbody>
            </table>`;
    } catch (erro) {
        mostrarMensagem(erro.message, "erro");
    }
}

async function mostrarDetalhe(id) {
    limparMensagem();

    try {
        const jogo = await chamarApi("/jogos/" + id);
        const movimentacoes = await chamarApi("/movimentacoes?jogoId=" + id);

        const linhas = movimentacoes.map(mov => `
            <tr>
                <td>${new Date(mov.data).toLocaleString("pt-BR")}</td>
                <td class="${mov.tipo === "Entrada" ? "entrada" : "saida"}">${mov.tipo}</td>
                <td>${mov.quantidade}</td>
                <td>${escapar(mov.motivo)}</td>
                <td><button class="perigo" onclick="removerMovimentacao('${mov.id}', '${jogo.id}')">Remover</button></td>
            </tr>`).join("");

        conteudo.innerHTML = `
            <h2>${escapar(jogo.nome)}</h2>
            <div class="painel">
                <p><strong>Console:</strong> ${escapar(jogo.console)}</p>
                <p><strong>Ano:</strong> ${jogo.anoLancamento}</p>
                <p><strong>Preço:</strong> ${formatarPreco(jogo.preco)}</p>
                <p><strong>Estoque:</strong> ${jogo.quantidadeEmEstoque}</p>
                <div class="acoes">
                    <button class="secundario" onclick="mostrarLista()">Voltar</button>
                    <button onclick="mostrarFormulario('${jogo.id}')">Editar</button>
                    <button class="perigo" onclick="removerJogo('${jogo.id}')">Apagar</button>
                </div>
            </div>

            <h3>Registrar entrada ou saída</h3>
            <form class="painel" novalidate onsubmit="registrarMovimentacao(event, '${jogo.id}')">
                <label for="tipo">Tipo *</label>
                <select id="tipo" required>
                    <option value="Entrada">Entrada (compra)</option>
                    <option value="Saida">Saída (venda)</option>
                </select>
                <label for="quantidade">Quantidade *</label>
                <input id="quantidade" type="number" value="1" required>
                <label for="motivo">Motivo *</label>
                <input id="motivo" type="text" placeholder="Ex.: venda no balcão" required>
                <div class="acoes"><button type="submit">Registrar</button></div>
            </form>

            <h3>Histórico</h3>
            <table>
                <thead>
                    <tr><th>Data</th><th>Tipo</th><th>Qtd.</th><th>Motivo</th><th></th></tr>
                </thead>
                <tbody>${linhas || "<tr><td colspan='5'>Nenhuma movimentação.</td></tr>"}</tbody>
            </table>`;
    } catch (erro) {
        mostrarMensagem(erro.message, "erro");
    }
}

async function mostrarFormulario(id = null) {
    limparMensagem();
    let jogo = { nome: "", console: "", anoLancamento: "", preco: "" };

    try {
        if (id) {
            jogo = await chamarApi("/jogos/" + id);
        }
    } catch (erro) {
        mostrarMensagem(erro.message, "erro");
        return;
    }

    conteudo.innerHTML = `
        <h2>${id ? "Editar jogo" : "Novo jogo"}</h2>
        <form class="painel" novalidate onsubmit="salvarJogo(event, ${id ? `'${id}'` : "null"})">
            <label for="nome">Nome *</label>
            <input id="nome" type="text" value="${escapar(jogo.nome)}" required>
            <label for="console">Console *</label>
            <input id="console" type="text" value="${escapar(jogo.console)}" required>
            <label for="ano">Ano de lançamento *</label>
            <input id="ano" type="number" value="${jogo.anoLancamento}" required>
            <label for="preco">Preço *</label>
            <input id="preco" type="number" step="0.01" value="${jogo.preco}" required>
            <div class="acoes">
                <button type="button" class="secundario" onclick="${id ? `mostrarDetalhe('${id}')` : "mostrarLista()"}">Cancelar</button>
                <button type="submit">Salvar</button>
            </div>
        </form>`;
}

async function salvarJogo(evento, id) {
    evento.preventDefault();

    if (!evento.target.checkValidity()) {
        mostrarMensagem("Preencha os campos obrigatórios.", "erro");
        return;
    }

    const dados = {
        nome: document.getElementById("nome").value,
        console: document.getElementById("console").value,
        anoLancamento: Number(document.getElementById("ano").value),
        preco: Number(document.getElementById("preco").value)
    };

    try {
        const jogo = id
            ? await chamarApi("/jogos/" + id, "PUT", dados)
            : await chamarApi("/jogos", "POST", dados);

        await mostrarDetalhe(jogo.id);
        mostrarMensagem("Jogo salvo.", "sucesso");
    } catch (erro) {
        mostrarMensagem(erro.message, "erro");
    }
}

async function removerJogo(id) {
    if (!confirm("Apagar este jogo?")) {
        return;
    }

    try {
        await chamarApi("/jogos/" + id, "DELETE");
        await mostrarLista();
        mostrarMensagem("Jogo apagado.", "sucesso");
    } catch (erro) {
        mostrarMensagem(erro.message, "erro");
    }
}

async function registrarMovimentacao(evento, jogoId) {
    evento.preventDefault();

    if (!evento.target.checkValidity()) {
        mostrarMensagem("Preencha os campos obrigatórios.", "erro");
        return;
    }

    const dados = {
        jogoId: jogoId,
        tipo: document.getElementById("tipo").value,
        quantidade: Number(document.getElementById("quantidade").value),
        motivo: document.getElementById("motivo").value
    };

    try {
        await chamarApi("/movimentacoes", "POST", dados);
        await mostrarDetalhe(jogoId);
        mostrarMensagem("Movimentação registrada.", "sucesso");
    } catch (erro) {
        mostrarMensagem(erro.message, "erro");
    }
}

async function removerMovimentacao(id, jogoId) {
    if (!confirm("Remover esta movimentação? O estoque será recalculado.")) {
        return;
    }

    try {
        await chamarApi("/movimentacoes/" + id, "DELETE");
        await mostrarDetalhe(jogoId);
        mostrarMensagem("Movimentação removida.", "sucesso");
    } catch (erro) {
        mostrarMensagem(erro.message, "erro");
    }
}

mostrarLista();
