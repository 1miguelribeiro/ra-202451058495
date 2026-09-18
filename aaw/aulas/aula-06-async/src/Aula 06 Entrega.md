Aula 06 

Por que o pagamento é síncrono nos dois fluxos? Poderia ser assíncrono?

Porque a decisão depende da resposta na hora. Não poderia ser assíncrono, pois aprovar a compra às cegas e só verificar o pagamento depois permitiria vender sem garantir o pagamento.

O que acontece se a fila perder um evento?

Se a aplicação reiniciar, tudo que estava na fila é perdido, ou seja, o pedido foi aceito, mas a notificação nunca é enviada.

Em produção, o que substituiria o Channel<T>?

RabbitMQ ou Kafka. Se o consumidor cair, as mensagens ficam guardadas e são processadas quando voltar.

Se a NotificacaoApi falhar, como o consumidor da fila deveria reagir?

Retry with backoff: tentar de novo, com espera crescente entre as tentativas.
Dead-letter queue: se continuar falhando, mover a mensagem para uma fila de "mortas", em vez de perder.

Síncrono: 4028.7247ms, resposta Imediata, mas pode falhar
Assíncrono: 1024.1243ms, resposta em background, não é garantida