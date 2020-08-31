using Lab5;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            RunServer();
        }

        static public void RunServer()
        {
            Model model = new Model();

            if (!MessageQueue.Exists(".\\private$\\srv.q"))
                using (var queue = MessageQueue.Create(".\\private$\\srv.q"))
                {
                    queue.Label = "queries";
                }

            var q = new MessageQueue(".\\private$\\srv.q");

            q.Formatter = new XmlMessageFormatter(new Type[] { typeof(string) });

            if (!MessageQueue.Exists(".\\private$\\cl.q"))
                using (var queue = MessageQueue.Create(".\\private$\\cl.q"))
                {
                    queue.Label = "ansvers";
                }

            var a = new MessageQueue(".\\private$\\cl.q");

            a.Formatter = new XmlMessageFormatter(new Type[] { typeof(string) });

            while (true)
            {
                Message message = q.Receive();

                switch (message.Body)
                {
                    case "1":
                        model.UndergroundLines.Add(new UndergroundLine("newItem", new List<Station>() { new Station() }, 1));
                        model.SaveChanges();

                        //handler1.Send(Encoding.Unicode.GetBytes($"New line added!"));
                        a.Send("New line added!");

                        break;
                    case "2":
                        model.Stations.Add(new Station("newItem", 1, 1));
                        model.SaveChanges();

                        //handler1.Send(Encoding.Unicode.GetBytes($"New station added!"));
                        a.Send("New station added!");

                        break;
                    case "3":
                        if (model.Stations.Count() > 0)
                        {
                            var index = model.Stations.Count() - 1;
                            var remItem = model.Stations.ToList()[index];

                            model.Stations.Remove(remItem);

                            model.SaveChanges();

                            //handler1.Send(Encoding.Unicode.GetBytes($"Last station deleted!"));
                            a.Send("Last station deleted!");
                        }
                        else
                            //handler1.Send(Encoding.Unicode.GetBytes($"Stations empty!"));
                            a.Send("Stations empty!");

                        break;
                    case "4":
                        if (model.UndergroundLines.Count() > 0)
                        {
                            var index = model.UndergroundLines.Count() - 1;
                            var line = model.UndergroundLines.ToList()[index];

                            string stations = null;

                            foreach (var item in line.Stations)
                            {
                                stations += item.ToString();
                            }

                            //handler1.Send(Encoding.Unicode.GetBytes(stations));
                            a.Send(stations);
                        }
                        else
                            //handler1.Send(Encoding.Unicode.GetBytes($"Lines empty!"));
                            a.Send("Lines empty!");

                        break;
                    case "5":
                        if (model.UndergroundLines.Count() > 0)
                        {
                            string lines = null;

                            foreach (var item in model.UndergroundLines)
                            {
                                lines += item.ToString();
                            }

                            //handler1.Send(Encoding.Unicode.GetBytes(lines));
                            a.Send(lines);
                        }
                        else
                            //handler1.Send(Encoding.Unicode.GetBytes($"Lines empty!"));
                            a.Send("Lines empty!");

                        break;
                    default:
                        //handler1.Send(Encoding.Unicode.GetBytes($"Invalid operation!"));
                        a.Send("Invalid operation!");
                        break;
                }

                Console.WriteLine(message.Body);
            }
        }
    }
}
