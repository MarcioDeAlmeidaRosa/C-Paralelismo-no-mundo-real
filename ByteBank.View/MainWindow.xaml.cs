using System;
using System.Linq;
using System.Windows;
using ByteBank.Core.Model;
using ByteBank.View.Utils;
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

        private void LimparView()
        {
            LstResultados.ItemsSource = null;
            TxtTempo.Text = null;
            pgsProgresso.Value = 0;
        }

        private void BtnProcessar_Click_Bloqueando(object sender, RoutedEventArgs e)
        {
            BtnProcessar.IsEnabled = false;
            BtnProcessar1.IsEnabled = false;
            BtnProcessar2.IsEnabled = false;
            BtnProcessar3.IsEnabled = false;
            var textoInicial = BtnProcessar2.Content;
            BtnProcessar2.Content = "Processando...";

            var contas = r_Repositorio.GetContaClientes();

            var resultado = new List<string>();

            LimparView();

            var inicio = DateTime.Now;

            foreach (var conta in contas)
            {
                var resultadoConta = r_Servico.ConsolidarMovimentacao(conta);
                resultado.Add(resultadoConta);
            }

            var fim = DateTime.Now;

            AtualizarView(resultado, fim - inicio);

            BtnProcessar.IsEnabled = true;
            BtnProcessar1.IsEnabled = true;
            BtnProcessar2.IsEnabled = true;
            BtnProcessar3.IsEnabled = true;
            BtnProcessar2.Content = textoInicial;
        }

        private void BtnProcessar_Click_Forma_Complicada(object sender, RoutedEventArgs e)
        {
            //Quando trabalhamos com Thread em paralelo, precisamos tomar o cuidado de bloquear o acionamento do acionador da rotina, neste caso o botão,
            //                  pois diferente de usar SingleThread, a interface não firacá bloqueada aguardando o processamento, permitindo assim o usuário
            //                  clicar diversas vezes no botão acionador desta rotina, casusando assim processamentos desnecessários podendo até causar um 
            //                  processamento excessivo da CPU/Memória
            BtnProcessar.IsEnabled = false;
            BtnProcessar1.IsEnabled = false;
            BtnProcessar2.IsEnabled = false;
            BtnProcessar3.IsEnabled = false;
            var textoInicial = BtnProcessar.Content;
            BtnProcessar.Content = "Processando...";

            //TaskScheduler --> Responsável por delegar atividade para cada núcleo da máquina que possa trabalhar na atividade do momento
            //                  Existente em todas as Threads (Principal e demais criadas pela principal ou por suas filhas)
            //TaskScheduler.FromCurrentSynchronizationContext --> método responsável por criar um (TaskScheduler) associado ao Thread corrente onde foi chamado.
            //                  Este recurso serve para quando a execução das Threads filhas criadas por este processo retornarem, nós assamos este Contexto inicial
            //                  para a continuação correta do código, assim, a continuação da nova Thread criada pelo término da execução das Threads filhas, possa 
            //                  acessar os objetos da Thread principal, assim nao será lançado exception 
            //                  (System.InvalidOperationException "O segmento de chamada não pode acessar esse objeto porque um segmento diferente é proprietário dele.")
            var threadprincipal = TaskScheduler.FromCurrentSynchronizationContext();

            var contas = r_Repositorio.GetContaClientes();

            var resultado = new List<string>();

            LimparView();

            var inicio = DateTime.Now;

            //Task.Factory --> propriedade statica da Classe Task onde cria e configura uma tarefa (System.Threading.Tasks.Task), 
            //                 retornando assim um objeto (TaskFactory) onde este é responsável por: Fornece suporte para criar e agendar objetos System.Threading.Tasks.Task
            //Ao receber o objet (TaskFactory) -> então solicitamos o seu início imediato pelo método (StartNew) onde este recebe uma 
            //                 (Action "expressão lambda (O delegate de ação para executar de forma assíncrona)") e retorna uma tarefa iniciada

            var contasTarefas = contas.Select(
                conta => Task.Factory.StartNew(() =>
                {
                    resultado.Add(r_Servico.ConsolidarMovimentacao(conta));
                })).ToArray();

            //Task.WhenAll -> Cria uma tarefa que ficará aguardando todas as tarefas passada para ela terminem, assim desbloqueia a execução da Thread (Task) onde foi chamada.
            //                Retorna uma tarefa que representa a conclusão de todas as tarefas passada em sua chamada.
            //Task.WhenAll.ContinueWith(continuationAction) --> da continuação da execução do bloco quando todas as TASKs passada para (Task.WhenAll) termina.
            //                Recebe um delegate com o código que depende da finalização das tarefas e continua sua execução.
            //Task.WhenAll.ContinueWith(,scheduler) --> Parâmetro que recebe o contexto que deverá ser considerado para continuar a execução do código.
            //                Desta forma podemos acessar objetos particular do "contexto inicial", sem lançar 
            //                (System.InvalidOperationException "O segmento de chamada não pode acessar esse objeto porque um segmento diferente é proprietário dele.")
            Task.WhenAll(contasTarefas)
            .ContinueWith(task =>
            {
                var fim = DateTime.Now;

                AtualizarView(resultado, fim - inicio);
            }, threadprincipal)
            .ContinueWith(task =>
            {
                //Como bloqueamos o acionamento do botão após clicado, precisamos libera-lo novamente após o termino da execução da sua finalidade
                BtnProcessar.IsEnabled = true;
                BtnProcessar1.IsEnabled = true;
                BtnProcessar2.IsEnabled = true;
                BtnProcessar3.IsEnabled = true;
                BtnProcessar.Content = textoInicial;
            }, threadprincipal);
        }

        private void BtnProcessar_Click_Forma_Um_Pouco_Mais_Simples(object sender, RoutedEventArgs e)
        {
            //Quando trabalhamos com Thread em paralelo, precisamos tomar o cuidado de bloquear o acionamento do acionador da rotina, neste caso o botão,
            //                  pois diferente de usar SingleThread, a interface não firacá bloqueada aguardando o processamento, permitindo assim o usuário
            //                  clicar diversas vezes no botão acionador desta rotina, casusando assim processamentos desnecessários podendo até causar um 
            //                  processamento excessivo da CPU/Memória
            BtnProcessar.IsEnabled = false;
            BtnProcessar1.IsEnabled = false;
            BtnProcessar2.IsEnabled = false;
            BtnProcessar3.IsEnabled = false;
            var textoInicial = BtnProcessar1.Content;
            BtnProcessar1.Content = "Processando...";

            //TaskScheduler --> Responsável por delegar atividade para cada núcleo da máquina que possa trabalhar na atividade do momento
            //                  Existente em todas as Threads (Principal e demais criadas pela principal ou por suas filhas)
            //TaskScheduler.FromCurrentSynchronizationContext --> método responsável por criar um (TaskScheduler) associado ao Thread corrente onde foi chamado.
            //                  Este recurso serve para quando a execução das Threads filhas criadas por este processo retornarem, nós assamos este Contexto inicial
            //                  para a continuação correta do código, assim, a continuação da nova Thread criada pelo término da execução das Threads filhas, possa 
            //                  acessar os objetos da Thread principal, assim nao será lançado exception 
            //                  (System.InvalidOperationException "O segmento de chamada não pode acessar esse objeto porque um segmento diferente é proprietário dele.")
            var threadprincipal = TaskScheduler.FromCurrentSynchronizationContext();

            IList<string> resultado = new List<string>();

            LimparView();

            var inicio = DateTime.Now;

            var progressDOTNet = new Progress<string>(str => { pgsProgresso.Value += 1; });
            //var progress = new ByteBankProgress<string>(str => pgsProgresso.Value += 1);

            ConsolidarContas(r_Repositorio.GetContaClientes(), progressDOTNet).ContinueWith(t =>
            {
                resultado = t.Result;
                var fim = DateTime.Now;
                AtualizarView(resultado, fim - inicio);
                //Como bloqueamos o acionamento do botão após clicado, precisamos libera-lo novamente após o termino da execução da sua finalidade
                BtnProcessar.IsEnabled = true;
                BtnProcessar1.IsEnabled = true;
                BtnProcessar2.IsEnabled = true;
                BtnProcessar3.IsEnabled = true;
                BtnProcessar1.Content = textoInicial;
            }, threadprincipal);
        }

        private async void BtnProcessar_Click_Com_AsyncAwait(object sender, RoutedEventArgs e)
        {
            var threadprincipal = TaskScheduler.FromCurrentSynchronizationContext();

            BtnProcessar.IsEnabled = false;
            BtnProcessar1.IsEnabled = false;
            BtnProcessar2.IsEnabled = false;
            BtnProcessar3.IsEnabled = false;
            var textoInicial = BtnProcessar3.Content;
            BtnProcessar3.Content = "Processando...";

            IList<string> resultado = new List<string>();

            LimparView();

            var inicio = DateTime.Now;

            var progressDOTNet = new Progress<string>(str => { pgsProgresso.Value += 1; });
            //var progress = new ByteBankProgress<string>(str => pgsProgresso.Value += 1);

            resultado = await ConsolidarContas(r_Repositorio.GetContaClientes(), progressDOTNet);

            var fim = DateTime.Now;
            AtualizarView(resultado, fim - inicio);
            BtnProcessar.IsEnabled = true;
            BtnProcessar1.IsEnabled = true;
            BtnProcessar2.IsEnabled = true;
            BtnProcessar3.IsEnabled = true;
            BtnProcessar3.Content = textoInicial;
        }

        private async Task<IList<string>> ConsolidarContas(IEnumerable<ContaCliente> contas, IProgress<string> reportadorDeProgresso)
        {
            pgsProgresso.Maximum = contas.Count();
            return await Task.WhenAll(contas.Select(conta => Task.Factory.StartNew(() =>
            {
                var ret = r_Servico.ConsolidarMovimentacao(conta);
                reportadorDeProgresso.Report(ret);
                return ret;
            })));
        }

        private void AtualizarView(IList<string> result, TimeSpan elapsedTime)
        {
            var tempoDecorrido = $"{ elapsedTime.Seconds }.{ elapsedTime.Milliseconds} segundos!";
            var mensagem = $"Processamento de {result.Count} clientes em {tempoDecorrido}";

            LstResultados.ItemsSource = result;
            TxtTempo.Text = mensagem;
        }
    }
}
