using System;
using System.Linq;
using System.Windows;
using System.Threading;
using ByteBank.Core.Service;
using ByteBank.Core.Repository;
using System.Collections.Generic;

namespace ByteBank.View
{
    public partial class MainWindow : Window
    {
        private readonly ContaClienteRepository r_Repositorio;
        private readonly ContaClienteService r_Servico;

        public MainWindow()
        {
            InitializeComponent();

            r_Repositorio = new ContaClienteRepository();
            r_Servico = new ContaClienteService();
        }

        private void BtnProcessar_Click(object sender, RoutedEventArgs e)
        {
            var contas = r_Repositorio.GetContaClientes();

            var contasQuantidadePorThread = contas.Count() / 4;

            var contas_parte1 = contas.Take(contasQuantidadePorThread);//Recupera a metade da lista
            var contas_parte2 = contas.Skip(contasQuantidadePorThread).Take(contasQuantidadePorThread);//Pula uma quantidade de registro da lista e retorna o restante
            var contas_parte3 = contas.Skip(contasQuantidadePorThread).Skip(contasQuantidadePorThread).Take(contasQuantidadePorThread);//Pula uma quantidade de registro da lista e retorna o restante
            var contas_parte4 = contas.Skip(contasQuantidadePorThread).Skip(contasQuantidadePorThread).Skip(contasQuantidadePorThread).Take(contasQuantidadePorThread);//Pula uma quantidade de registro da lista e retorna o restante

            var resultado = new List<string>();

            AtualizarView(new List<string>(), TimeSpan.Zero);

            var inicio = DateTime.Now;

            Thread thread_parte_1 = new Thread(() =>
            {
                foreach (var conta in contas_parte1)
                {
                    var resultadoConta = r_Servico.ConsolidarMovimentacao(conta);
                    resultado.Add(resultadoConta);
                }
            });

            Thread thread_parte_2 = new Thread(() =>
            {
                foreach (var conta in contas_parte2)
                {
                    var resultadoConta = r_Servico.ConsolidarMovimentacao(conta);
                    resultado.Add(resultadoConta);
                }
            });

            Thread thread_parte_3 = new Thread(() =>
            {
                foreach (var conta in contas_parte3)
                {
                    var resultadoConta = r_Servico.ConsolidarMovimentacao(conta);
                    resultado.Add(resultadoConta);
                }
            });

            Thread thread_parte_4 = new Thread(() =>
            {
                foreach (var conta in contas_parte4)
                {
                    var resultadoConta = r_Servico.ConsolidarMovimentacao(conta);
                    resultado.Add(resultadoConta);
                }
            });

            thread_parte_1.Start();
            thread_parte_2.Start();
            thread_parte_3.Start();
            thread_parte_4.Start();

            while (thread_parte_1.IsAlive || thread_parte_2.IsAlive || thread_parte_3.IsAlive || thread_parte_4.IsAlive)
            {
                Thread.Sleep(250);
            }


            var fim = DateTime.Now;

            AtualizarView(resultado, fim - inicio);
        }

        private void AtualizarView(List<String> result, TimeSpan elapsedTime)
        {
            var tempoDecorrido = $"{ elapsedTime.Seconds }.{ elapsedTime.Milliseconds} segundos!";
            var mensagem = $"Processamento de {result.Count} clientes em {tempoDecorrido}";

            LstResultados.ItemsSource = result;
            TxtTempo.Text = mensagem;
        }
    }
}
