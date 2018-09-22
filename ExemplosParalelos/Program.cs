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

    }
}
