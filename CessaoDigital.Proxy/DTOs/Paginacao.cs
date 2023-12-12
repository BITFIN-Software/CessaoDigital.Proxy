﻿// Copyright (c) 2023 - BITFIN Tecnologia Ltda. Todos os Direitos Reservados.
// Código exclusivo para consumo dos serviços (API's) da Plataforma Cessão Digital.

namespace CessaoDigital.Proxy.DTOs
{
    /// <summary>
    /// Dados da paginação.
    /// </summary>
    public class Paginacao
    {
        /// <summary>
        /// Quantidade de registros atingidos pela consulta.
        /// </summary>
        public int? Registros { get; set; }

        /// <summary>
        /// Total de páginas geradas.
        /// </summary>
        public int? Paginas { get; set; }

        /// <summary>
        /// Quantidade de registros por página.
        /// </summary>
        public int? RegistrosPorPagina { get; set; }

        /// <summary>
        /// Página a qual o resultado se refere.
        /// </summary>
        public int? PaginaAtual { get; set; }
    }
}