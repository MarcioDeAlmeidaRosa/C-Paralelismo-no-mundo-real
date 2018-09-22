using System;
using System.Threading;
using System.Threading.Tasks;

namespace ExemplosParalelos
{
    class Program
    {
        static void Main(string[] args)
        {
            //A primeira solução é menos performática.
            LoopComThread();

            Console.WriteLine("Aperte qualquer tecla para ir para ao próximo exmplo.");
            Console.ReadLine();


            //A solução em 2. é mais performática, porque passamos a responsabilidade de provisionamento e gerenciamento 
            //das Threads disponíveis para o TaskScheduler default usado pela Task.Factory, que possui uma inteligência
            LoopComTask();



            Console.WriteLine("Aperte qualquer tecla para ir para ao próximo exmplo.");
            Console.ReadLine();
            //Testando InvalidOperationException, pois há a tentativa de se acessar um objeto da Thread da interface gráfica a partir de outra Thread
            Console.WriteLine("Testando InvalidOperationException, pois há a tentativa de se acessar um objeto da Thread da interface gráfica a partir de outra Thread");
            LancandoException();

            Console.ReadLine();
        }

        static void LoopComThread()
        {
            Console.WriteLine("Executando por Thread");
            for (int i = 0; i < 100; i++)
            {
                var msg = "Thread número " + i + 1;
                var thread = new Thread(() => Console.WriteLine(msg));
                thread.Start();
            }
            Console.WriteLine("************************************");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("");
        }

        static void LoopComTask()
        {
            Console.WriteLine("Executando por Thread");
            for (int i = 0; i < 100; i++)
            {
                var msg = "Task número " + i + 1;
                Task.Factory.StartNew(() => Console.WriteLine(msg));
            }
            Console.WriteLine("************************************");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("");
        }

        static void LancandoException()
        {
            try
            {
                System.Windows.Controls.Button btnOk = new System.Windows.Controls.Button();
                new TaskFactory().StartNew(() => ExecutarProcessamento()).ContinueWith((task) => btnOk.IsEnabled = true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exercício 06 Problemas ao acessar a interface gráfica da forma errada --> ({ex.ToString()})");
            }
        }

        private static void ExecutarProcessamento()
        {
            Console.Write("Executando --> ExecutarProcessamento");
        }

        private static void OutroExercicio()
        {
            try
            {
                System.Windows.Controls.Button btnCalcular = new System.Windows.Controls.Button();
                btnCalcular.IsEnabled = false;

                Task.Factory.StartNew(() =>
                {
                    //Apesar de capturarmos o contexto na linha 3, o contexto capturado é diferente do contexto da thread principal, então uma exceção do 
                    //tipo InvalidOperationException será lançada na linha 5.
                    //O contexto capturado na linha 3 é diferente do contexto da thread principal, uma vez que estamos capturando o contexto da Task criada na linha 1
                    var context = TaskScheduler.FromCurrentSynchronizationContext();
                    Task.Factory.StartNew(() => CalculaRaiz(100)).ContinueWith(t => btnCalcular.IsEnabled = true, context);
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exercício 03 SyncronizationContext --> ({ex.ToString()})");
            }
        }

        private static void CalculaRaiz(int value)
        {
            Console.Write($"Executando --> CalculaRaiz - {value}");
        }
    }
}
