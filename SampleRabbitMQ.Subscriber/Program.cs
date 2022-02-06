using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;

namespace SampleRabbitMQ.Subscriber
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.BasicQos(0, 1, false);
                var consumer = new EventingBasicConsumer(channel);
                // channel.QueueDeclare("hello-queue", true, false, false);

                #region [BasicQos Parameters Properties]
                //prefetchSize: Herhangi bir boyuttaki mesajı gönderebilirsin.
                //prefetchCount : Mesajlar kaç kaç gelsin.
                //global : PrefetchCount ne kadarsa o kadar sayıda mesajı var olan Subscriber'lara dağıtır.
                #endregion

                #region [Not Exchange]


                //channel.BasicConsume("hello-queue", false, consumer); //autoAck :True  mesaj Subscriber'a gittiğinde otomatik olarak mesajı siler.
                #endregion

                #region [Fanout Exchange]
                /* var randomQueueName = channel.QueueDeclare().QueueName;*/ //Random kuyruk ismi oluşturmk için.

                #region [Kuyruğu kalıcı hale getirmek için]
                //channel.QueueDeclare("log-database-save", true, false, false); //Kalıcı hale getirmek için kuyruk oluşturulur.
                #endregion


                /* channel.QueueBind(randomQueueName, "logs-fanout", "", null);*/ //Bu kuyruğu exchange'e bind edeceğim.(yeni kuyruk oluşturmuyorum.)

                //channel.BasicConsume(randomQueueName, false, consumer);
                //Console.WriteLine("Loglar dinleniyor.");

                #endregion

                #region [Direct Exchange]
                var queueName = "direct-queue-critical";
                channel.BasicConsume(queueName, false, consumer);
                Console.WriteLine("Loglar dinleniyor.");
                #endregion


                consumer.Received += (object sender, BasicDeliverEventArgs e) => //RabbitMQ subscriber'a mesaj geldiğinde bu event çalışır.
                {
                    var message = Encoding.UTF8.GetString(e.Body.ToArray());

                    Thread.Sleep(1000);
                    Console.WriteLine("Gelen Mesaj: " + message);

                    channel.BasicAck(e.DeliveryTag, false); //RabbitMQ dan gelen mesajı DeliveryTag ile aldığımızı belirtiyoruz. Amaç : Mesajını okunduğunu belirtip ondan sonra silme işlemini yapmak.
                }; 

                Console.ReadLine();
            }
        }
    }
}
