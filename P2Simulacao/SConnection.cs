using System;
using System.Collections.Generic;
using System.Text;

namespace P2Simulacao
{
    /// <summary>
    /// Representa  uma conexão de uma atividade com sua probabilidade de ocorrer, classe de destino e função que representa
    /// o tempo de processamento da mesma.
    /// </summary>
    class SConnection
    {
        // Atividade a qual o Item será mandado
        public SActivity Connection { get; set; }

        // Probabilidade da Conexão acontecer 0.0 <= Probability <= 1.0
        public double Probability { get; set; }

        // Função que retorna a quantidade de tempo para o Item ser processado e passar por esta conexão
        public Func<double> CalcTS { get; set; }

        public SConnection(SActivity conn, Func<double> calc, double prob = 1.0)
        {
            Connection = conn;
            CalcTS = calc;
            Probability = prob;
        }
    }
}
