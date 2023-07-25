﻿// Copyright (c) 2023 - BITFIN Software Ltda. Todos os Direitos Reservados.
// Código exclusivo para consumo dos serviços (API's) da Plataforma Cessão Digital.

using CessaoDigital.Proxy.Utilitarios;
using System.Net;
using System.Net.Http.Headers;

namespace CessaoDigital.Proxy.Comunicacao
{
    /// <summary>
    /// Classe base que define os recursos necessários para comunicação com a API remota.
    /// </summary>
    public abstract class API
    {
        /// <summary>
        /// Proxy HTTP pré-configurado para comunicação com o serviço.
        /// </summary>
        protected readonly HttpClient proxy;

        /// <summary>
        /// Inicializa a API.
        /// </summary>
        /// <param name="proxy">Instância da classe <see cref="HttpClient"/> gerada pelo proxy.</param>
        public API(HttpClient proxy)
        {
            this.proxy = proxy;
            this.MimeType = new MediaTypeWithQualityHeaderValue(Protocolo.MimeType);
        }

        /// <summary>
        /// Configura, envia e captura o retorno da requisição para um determinado serviço.
        /// </summary>
        /// <param name="requisicao">Mensagem de requisição para o serviço.</param>
        /// <param name="cancellationToken">Instrução para eventual cancelamento da requisição.</param>
        /// <exception cref="ErroNaRequisicao">Exceção disparada se alguma falha ocorrer durante a requisição ou em seu processamento.</exception>
        protected virtual async Task Executar(HttpRequestMessage requisicao, CancellationToken cancellationToken = default)
        {
            requisicao.Headers.Accept.Add(this.MimeType);

            using (var resposta = await this.proxy.SendAsync(requisicao, cancellationToken))
            {
                try
                {
                    resposta.EnsureSuccessStatusCode();
                }
                catch (HttpRequestException ex) when (ex.StatusCode is HttpStatusCode.BadRequest)
                {
                    throw new ErroNaRequisicao(ex, await resposta.Content.ReadAs<DTOs.Falha>(cancellationToken));
                }
            }
        }

        /// <summary>
        /// Configura, envia e captura o retorno da requisição para um determinado serviço.
        /// </summary>
        /// <typeparam name="T">Especifica o tipo em que o conteúdo de retorno deverá ser deserializado.</typeparam>
        /// <param name="requisicao">Mensagem de requisição para o serviço.</param>
        /// <param name="analiseDeRetorno">Função que analisa o retorno e constrói o objeto do tipo <typeparamref name="T"/></param>.
        /// <param name="cancellationToken">Instrução para eventual cancelamento da requisição.</param>
        /// <exception cref="ErroNaRequisicao">Exceção disparada se alguma falha ocorrer durante a requisição ou em seu processamento.</exception>
        /// <returns>Retorna o objeto do tipo <typeparamref name="T"/> pronto para utilização.</returns>
        protected virtual async Task<T> Executar<T>(HttpRequestMessage requisicao, Func<HttpResponseMessage, Task<T>> analiseDeRetorno, CancellationToken cancellationToken = default)
        {
            requisicao.Headers.Accept.Add(this.MimeType);

            using (var resposta = await this.proxy.SendAsync(requisicao, cancellationToken))
            {
                try
                {
                    resposta.EnsureSuccessStatusCode();

                    return await analiseDeRetorno(resposta);
                }
                catch (HttpRequestException ex) when (ex.StatusCode is HttpStatusCode.BadRequest)
                {
                    throw new ErroNaRequisicao(ex, await resposta.Content.ReadAs<DTOs.Falha>(cancellationToken));
                }
            }
        }

        /// <summary>
        /// Mime type padrão.
        /// </summary>
        protected MediaTypeWithQualityHeaderValue MimeType { get; init; }
    }
}