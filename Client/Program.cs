using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello! Change what do u want:\n" +
                    "1 - add new line\n" +
                    "2 - add new station\n" +
                    "3 - delete last station\n" +
                    "4 - list of station last line\n" +
                    "5 - full list of lines\n" +
                    "0 - exit");

            new Task(() => GetAnsver()).Start();

            RunClient();
        }

        static public void RunClient()
        {
            while (true)
            {
                if (!MessageQueue.Exists(".\\private$\\srv.q"))
                    using (var queue = MessageQueue.Create(".\\private$\\srv.q"))
                    {
                        queue.Label = "queries";
                    }

                var q = new MessageQueue(".\\private$\\srv.q");

                var ansver = Console.ReadLine();

                q.Send(ansver);
            }
        }

        static public void GetAnsver()
        {
            if (!MessageQueue.Exists(".\\private$\\cl.q"))
                using (var queue = MessageQueue.Create(".\\private$\\cl.q"))
                {
                    queue.Label = "queries";
                }

            var q = new MessageQueue(".\\private$\\cl.q");

            q.Formatter = new XmlMessageFormatter(new Type[] { typeof(string) });

            while (true)
            {
                var message = q.Receive();

                Console.WriteLine(message.Body);
            }
        }
    }
}
