using System;
using System.Linq;
using System.Windows;
using ByteBank.Core.Service;
using System.Threading.Tasks;
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

            var resultado = new List<string>();

            AtualizarView(new List<string>(), TimeSpan.Zero);

            var inicio = DateTime.Now;

            //Task.Factory --> propriedade statica da Classe Task onde cria e configura uma tarefa (System.Threading.Tasks.Task), 
            //                 retornando assim um objeto (TaskFactory) onde este é responsável por: Fornece suporte para criar e agendar objetos System.Threading.Tasks.Task
            //Ao receber o objet (TaskFactory) -> então solicitamos o seu início imediato pelo método (StartNew) onde este recebe uma 
            //                 (Action "expressão lambda (O delegate de ação para executar de forma assíncrona)") e retorna uma tarefa iniciada
            //TaskScheduler --> Responsável por delegar atividade para cada núcleo da máquina que possa trabalhar na atividade do momento
            var contasTarefas = contas.Select(
                conta => Task.Factory.StartNew(() =>
                {
                    resultado.Add(r_Servico.ConsolidarMovimentacao(conta));
                })).ToArray();

            //Task.WhenAll -> Cria uma tarefa que ficará aguardando todas as tarefas passada para ela terminem, assim desbloqueia a execução da Thread (Task) onde foi chamada.
            //                Retorna uma tarefa que representa a conclusão de todas as tarefas passada em sua chamada.
            //Task.WhenAll.ContinueWith --> da continuação da execução do bloco quando todas as TASKs passada para (Task.WhenAll) termina.
            //                Recebe um delegate com o código que depende da finalização das tarefas e continua sua execução.
            Task.WhenAll(contasTarefas).ContinueWith(task =>
            {
                var fim = DateTime.Now;

                AtualizarView(resultado, fim - inicio);
            });
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
