using System;
using System.Collections.Generic;
using System.Text;

namespace P2Simulacao
{
    /// <summary>
    /// Item a ser executado por uma atividade.
    /// </summary>
    class SItem
    {
        // Nome do Item
        public string Nome { get; set; }

        // Tempo de Inicio do Processamento
        public double Inicio { get; set; }

        // Tempo de Fim do Processamento
        public double Fim { get; set; }
    }
}
