using RabbitMQ.Client;
using System;
using System.Linq;
using System.Text;

namespace SampleRabbitMQ.Publisher
{
    public enum LogNames
    {
        critical = 1,
        error = 2,
        warning = 3,
        info = 4
    }
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                #region [QueueDeclare Parameters Properties]
                //durable :    True : Kuyruklar fiziksek olarak kaydedilir, False : Memory'de tutulur ,restart atılırsa gider.
                //exclusive :  True :Bu kuyruğa publisher da oluşturduğum kanal üzerinden bağlanabilirim. False : Kuyruğa göndermiş olduğum mesajı farklı kanaldan bağlanabilirim.
                //autoDelete : True :Kuyruğa bağlı olan Son Subscriber down olursa kuyruk silinir. False : Son Subscriber yanlışlıkla giderse kuyruk silinmesin.
                #endregion

                #region [Not Exchange]
                //channel.QueueDeclare("hello-queue", true, false, false);  exchange olmadan direk kuyruk oluşturmak için


                //Enumerable.Range(1, 50).ToList().ForEach(X =>
                //{
                //    //string message = $"log {X}";
                //    //var messageBody = Encoding.UTF8.GetBytes(message);

                //    //#region [Not Exchange]
                //    // channel.BasicPublish(string.Empty, "hello-queue", null, messageBody); 
                //    ////exchange yoksa Empty : Default Exchange, routeKey'e : kuyruk ismi olan hello-queue ismini vermek durumundayız.
                //    //#endregion
                //    //Console.WriteLine($"Mesaj Gönderilmiştir. {message}");
                //});
                #endregion

                #region [Fanout Exchange]
                //channel.ExchangeDeclare("logs-fanout", durable: true, type: ExchangeType.Fanout);

                //Enumerable.Range(1, 50).ToList().ForEach(X =>
                //{
                //    string message = $"log {X}";
                //    var messageBody = Encoding.UTF8.GetBytes(message);
                //    channel.BasicPublish("logs-fanout", "", null, messageBody);
                //    Console.WriteLine($"Mesaj Gönderilmiştir. {message}");
                //});

                #endregion

                #region [Direct Exchange]
                channel.ExchangeDeclare("logs-direct", durable: true, type: ExchangeType.Direct);

                Enum.GetNames(typeof(LogNames)).ToList().ForEach(log =>
                {
                    var queueName = $"direct-queue-{log}";
                    channel.QueueDeclare(queueName, durable: true, exclusive: false, autoDelete: false);
                    channel.QueueBind(queueName, "logs-direct", $"route-{log}", null);
                });


                Enumerable.Range(1, 50).ToList().ForEach(X =>
                {
                    LogNames log = (LogNames)new Random().Next(1, 5);
                    string message = $"log-type {log}";
                    var routeKey = $"route-{log}";
                    var messageBody = Encoding.UTF8.GetBytes(message);
                    channel.BasicPublish("logs-direct", routeKey, null, messageBody);
                    Console.WriteLine($"Log Gönderilmiştir. {message}");
                });
                #endregion





                Console.ReadLine();
            }
        }
    }
}

#region [Exchange Types]
//1. Fanout Exchange
//Consumer'lar kendi kuyruklarını oluşturur. 
//2. Direct Exchange
//3. Topic Exchange
//4. Header Exchange
#endregion