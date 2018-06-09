using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace P2Simulacao
{

    /// <summary>
    /// Representa uma atividade e as informações referentes ao processamento dos itens da mesma.
    /// </summary>
    class SActivity
    {
        Random rand;

        public bool Print { get; set; }

        public List<SItem> FilaEntrada { get; set; }

        public List<SItem> Resultado { get; set; }

        public List<SConnection> Connections { get; set; }

        public double TempoEspera { get; set; }

        public double TempoOcioso { get; set; }

        public string Nome { get; set; }

        // Inicializa Valores Padrão para a Atividade
        public SActivity()
        {
            Connections = new List<SConnection>();
            FilaEntrada = new List<SItem>();
            Resultado = new List<SItem>();
            rand = new Random();
            Print = true;
        }

        // Preenche a Lista de entrada com uma Lista de itens
        public void PreencheEntrada(List<SItem> itens)
        {
            foreach(SItem item in itens)
            {
                FilaEntrada.Add(item);
            }
        }

        // Adiciona uma Conexão para a ativdade
        public void AddConnection(SConnection conn)
        {
            Connections.Add(conn);
            Connections = Connections.OrderBy(x => x.Probability).ToList();
        }

        SItem ProcessItem(SItem newItem, double tempo)
        {
            var ultimoItem = Resultado.OrderByDescending(x => x.Fim).FirstOrDefault();
            if (ultimoItem == null)
            {
                newItem.Fim = newItem.Inicio + tempo;
                return newItem;
            }
            if (ultimoItem.Fim < newItem.Inicio)
            {
                TempoOcioso += newItem.Inicio - ultimoItem.Fim;
            }
            else if (ultimoItem.Fim > newItem.Inicio)
            {
                TempoEspera += ultimoItem.Fim - newItem.Inicio;
                newItem.Inicio = ultimoItem.Fim;
            }
            newItem.Fim = newItem.Inicio + tempo;
            return newItem;
        }

        void PrintAtividadesEntrada()
        {
            Console.WriteLine("{0}:", Nome);
            foreach (SItem i in FilaEntrada)
            {
                Console.WriteLine("{0} - Chegada {1}", i.Nome, i.Inicio);
            }
            Console.WriteLine();
        }

        void PrintAtividadesResultado()
        {
            Console.WriteLine("{0}:", Nome);
            foreach (SItem i in Resultado)
            {
                Console.WriteLine("{0} - Chegada {1}, Fim {2}", i.Nome, i.Inicio, i.Fim);
            }
            Console.WriteLine("Tempo Espera {0} - Tempo Ocioso {1}\n", TempoEspera, TempoOcioso);
        }

        void RunConnectedActivities()
        {
            foreach (SConnection conn in Connections)
            {
                conn.Connection.Run();
            }
        }

        SConnection GetConnectionByProbability(double prob)
        {
            foreach(SConnection conn in Connections)
            {
                if (prob <= conn.Probability)
                    return conn; 
            }
            return null;
        }

        public void Run()
        {
            // Não roda caso o Nó seja uma "folha", ou seja, não tenha conexões
            if (!Connections.Any())
            {
                if (Print)
                    PrintAtividadesEntrada();
                return;
            }

            // Ordena a fila de entrada pela ordem de entrada na atividade
            FilaEntrada = FilaEntrada.OrderBy(x => x.Inicio).ToList();
            
            foreach (SItem item in FilaEntrada)
            {
                // Seleciona a próxima atividade (conexão) de acordo com um número aleatório gerado
                SConnection target = GetConnectionByProbability(rand.NextDouble());

                // Pega o tempo de processamento desse item e atualiza o mesmo
                var tempoProc = target.CalcTS();
                var newItem = ProcessItem(new SItem { Nome = item.Nome, Inicio = item.Inicio }, tempoProc);

                // Adiciona o item na lista de resultados
                Resultado.Add(newItem);

                // Adiciona o item na fila de entrada da próxima atividade
                target.Connection.FilaEntrada.Add(new SItem { Inicio = newItem.Fim, Nome = newItem.Nome });
            }

            // Faz o Display das informações obtidas com a simulação da atividade
            if (Print)
                PrintAtividadesResultado();

            // Processa as connexões
            RunConnectedActivities();
        }
    }
}
