using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.Distributions;

namespace P2Simulacao
{
    class Program
    {
        // Atraso do Roteador
        static double aRoteador = 0.00015;
        // Capacidade das LANs = 100 Mbps = 100 * 1024000 * 0.8 bps
        static double capLan = 100 * 1024000 * 0.8;
        // Processamento Cliente
        static double procCliente = 0.005;
        // Velocidade Internet
        static double vInternet = 16000;
        // Overhead Protocolo
        static double overHeadProtocolo = 1.1;
        // Ritmo de chegada dos pacotes
        static double ritmoChegada = 3600.0 / 250.0;

        static List<SItem> GerarEntrada(Random rand)
        {
            // Simula a chegada das visitas
            var visitas = new List<SItem>();
            
            var chegada = 0.0;
            // Gera As visitas
            for (int i = 0; i < 30; i++)
            {
                visitas.Add(new SItem
                {
                    Nome = String.Format("Visita {0}", i + 1),
                    Inicio = chegada
                });
                chegada += (-ritmoChegada) * Math.Log(rand.NextDouble());
            }

            foreach (SItem visita in visitas.OrderBy(v => v.Inicio))
            {
                Console.WriteLine(string.Format("{0} chegou: {1}", visita.Nome, visita.Inicio));
            }

            Console.WriteLine();

            return visitas;
        }

        static double GetTriangular(Random random, double lower, double mode, double upper)
        {
            return Triangular.Sample(random, lower, upper, mode);
        }

        static double GetUniform(Random random, double lower, double upper)
        {
            return ContinuousUniform.Sample(random, lower, upper) / 1000;
        }

        static double GetTempoInternet(double tamanho)
        {
            return tamanho / vInternet;
        }

        static double GetTempoLan(double tamanho)
        {
            return tamanho / capLan;
        }

        static void Main(string[] args)
        {
            // Semente aleatória para o programa
            Random rand = new Random();

            // Tempo Servidor de Aplicações Interação 2
            Func<double> tSA2 = () =>
            {
                return GetUniform(rand, 40, 60);
            };
            // Tempo Servidor de Aplicações Interação 3
            Func<double> tSA3 = () =>
            {
                return GetUniform(rand, 60, 120);
            };

            // Tempo Servidor Web Interação 1
            Func<double> tSW1 = () =>
            {
                return GetUniform(rand, 4, 6);
            };
            // Tempo Servidor Web Interação 2-2
            Func<double> tSW22 = () =>
            {
                return GetUniform(rand, 5, 7);
            };
            // Tempo Servidor Web Interação 2-5
            Func<double> tSW25 = () =>
            {
                return GetUniform(rand, 7, 10);
            };
            // Tempo Servidor Web Interação 3
            Func<double> tSW3 = () =>
            {
                return GetUniform(rand, 9, 12);
            };

            // Tempo Banco de Dados
            Func<double> tBD = () =>
            {
                return GetUniform(rand, 15, 30) + GetUniform(rand, 50, 400);
            };

            // Funções de Calcular tamanho
            Func<double> m1 = () =>
            {
                return GetTriangular(rand, 100, 200, 250) * overHeadProtocolo;
            };
            Func<double> m2 = () =>
            {
                return GetTriangular(rand, 100, 200, 300) * overHeadProtocolo;
            };
            Func<double> m3 = () =>
            {
                return GetTriangular(rand, 250, 400, 450) * overHeadProtocolo;
            };
            Func<double> m4 = () =>
            {
                return GetTriangular(rand, 1500, 2500, 3000) * overHeadProtocolo;
            };
            Func<double> m5 = () =>
            {
                return GetTriangular(rand, 1500, 2100, 2800) * overHeadProtocolo;
            };
            Func<double> m6 = () =>
            {
                return GetTriangular(rand, 400, 550, 800) * overHeadProtocolo;
            };
            Func<double> m7 = () =>
            {
                return GetTriangular(rand, 2000, 3000, 3500) * overHeadProtocolo;
            };
            Func<double> m8 = () =>
            {
                return GetTriangular(rand, 1800, 2000, 2300) * overHeadProtocolo;
            };
            Func<double> m9 = () =>
            {
                return GetTriangular(rand, 1500, 2100, 2800) * overHeadProtocolo;
            };


            // Definição das Propriedades das Atividades
            var no1 = new SActivity { Nome = "Nó 1" };
            var no2 = new SActivity { Nome = "Nó 2" };
            var no3 = new SActivity { Nome = "Nó 3" };
            var no4 = new SActivity { Nome = "Nó 4" };
            var no5 = new SActivity { Nome = "Nó 5" };
            var no6 = new SActivity { Nome = "Nó 6" };
            var no7 = new SActivity { Nome = "Nó 7" };
            var no8 = new SActivity { Nome = "Nó 8" };
            var no9 = new SActivity { Nome = "Nó 9" };
            var no10 = new SActivity { Nome = "Nó 10" };

            // Definir as conexões entre os nós e a função de calcular o tempo de processamento em cada uma delas
            no1.AddConnection(new SConnection(no2, () => 
            {
                var tamanho = m1();
                return GetTempoLan(tamanho) + aRoteador + GetTempoInternet(tamanho) + procCliente;
            }));

            // nó 2 - 0.05 de chance de ir para o nó 3 e 0.95 de ir para o nó 4
            no2.AddConnection(new SConnection(no3, () =>
            {
                var tamanho = m2();
                return tSW1() + GetTempoLan(tamanho) + aRoteador + GetTempoInternet(tamanho) + procCliente;
            }, 0.05));
            no2.AddConnection(new SConnection(no4, () =>
            {
                var tamanho = m3();
                return tSW22() + GetTempoLan(tamanho);
            }));

            // nó 4 - 0.2 de chance de ir para o nó 5 e 0.8 de ir para o nó 7
            no4.AddConnection(new SConnection(no5, () =>
            {
                var tamanho = m4();
                return tSA2() + GetTempoLan(tamanho);
            }, 0.2));
            no4.AddConnection(new SConnection(no7, () =>
            {
                var tamanho = m6();
                return tSA2() + GetTempoLan(tamanho);
            }));

            // nó 5 para nó 6
            no5.AddConnection(new SConnection (no6, () =>
            {
                var tamanho = m5();
                return tSW25() + GetTempoLan(tamanho) + aRoteador + GetTempoInternet(tamanho) + procCliente;
            }));

            // nó 7 para nó 8
            no7.AddConnection(new SConnection(no8, () =>
            {
                var tamanho = m7();
                return tBD() + GetTempoLan(tamanho);
            }));

            // nó 8 para nó 9
            no8.AddConnection(new SConnection(no9, () =>
            {
                var tamanho = m8();
                return tSA3() + GetTempoLan(tamanho);
            }));

            // nó 9 para nó 10
            no9.AddConnection(new SConnection(no10, () =>
            {
                var tamanho = m9();
                return tSW3() + GetTempoLan(tamanho) + aRoteador + GetTempoInternet(tamanho) + procCliente;
            }));

            // Gera os pacotes que serão simulados
            List<SItem> visitas = GerarEntrada(rand);

            // Preenche a fila de chegada do no1
            no1.PreencheEntrada(visitas);

            // Roda a Simulação
            no1.Run();

            // Evita que o console feche automaticamente
            Console.ReadKey();
        }
    }
}
