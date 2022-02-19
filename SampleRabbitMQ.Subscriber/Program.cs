using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
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
                //Eğer Subscriber düşünce kuyrukta gitsin istiyorsak bunu kullanmamıza gerek yok 
                #endregion


                /* channel.QueueBind(randomQueueName, "logs-fanout", "", null);*/ //Bu kuyruğu exchange'e bind edeceğim.(yeni kuyruk oluşturmuyorum.)

                //channel.BasicConsume(randomQueueName, false, consumer);
                //Console.WriteLine("Loglar dinleniyor.");

                #endregion

                #region [Direct Exchange]
                //var queueName = "direct-queue-critical";
                //channel.BasicConsume(queueName, false, consumer);
                //Console.WriteLine("Loglar dinleniyor.");
                #endregion

                #region [Topic Exchange]
                //optinal //channel.ExchangeDeclare("logs-topic", durable: true, type: ExchangeType.Topic); //Publisher tarafında oluşturduğum için, burda da aynı değerlerle oluşturursam hata almam.

                //var queueName = channel.QueueDeclare().QueueName;
                //var routeKey = "*.error.*";
                //channel.QueueBind(queueName, "logs-topic", routeKey);
                //channel.BasicConsume(queueName, false, consumer);
                //Console.WriteLine("Loglar dinleniyor");
                #endregion

                #region [Header Exchange]

                //var queueName = channel.QueueDeclare().QueueName;
                //Dictionary<string, object> headers = new Dictionary<string, object>();
                //headers.Add("format", "pdf");
                //headers.Add("shape", "a4");
                //headers.Add("x-match", "any");
                //channel.QueueBind(queueName, "header-exchange", string.Empty, headers);
                //channel.BasicConsume(queueName, false, consumer);
                //Console.WriteLine("Loglar dinleniyor");
                #endregion

                #region [Mesajları Kalıcı Hale Getirmek (Örnek : Header Exchange)]
                //Puplisher tarafında ayar yapıldı.
                //var queueName = channel.QueueDeclare().QueueName;
                //Dictionary<string, object> headers = new Dictionary<string, object>();
                //headers.Add("format", "pdf");
                //headers.Add("shape", "a4");
                //headers.Add("x-match", "any");
                //channel.QueueBind(queueName, "header-exchange", string.Empty, headers);
                //channel.BasicConsume(queueName, false, consumer);
                //Console.WriteLine("Loglar dinleniyor");
                #endregion

                #region [Complex Type'ları Mesaj Olarak İletmek (Örnek : Header Exchange)]
                var queueName = channel.QueueDeclare().QueueName;
                Dictionary<string, object> headers = new Dictionary<string, object>();
                headers.Add("format", "pdf");
                headers.Add("shape", "a4");
                headers.Add("x-match", "any");
                channel.QueueBind(queueName, "header-exchange", string.Empty, headers);
                channel.BasicConsume(queueName, false, consumer);
                Console.WriteLine("Loglar dinleniyor");
                #endregion

                //


                consumer.Received += (object sender, BasicDeliverEventArgs e) => //RabbitMQ subscriber'a mesaj geldiğinde bu event çalışır.
                {
                    var message = Encoding.UTF8.GetString(e.Body.ToArray());

                    #region [Complext Type mesajları almak için]
                    //Car car = JsonSerializer.Deserialize<Car>(message);
                    //Console.Write($"Gelen mesaj :{car.name}-{car.price}");
                    #endregion

                    Thread.Sleep(1000);
                    Console.WriteLine("Gelen Mesaj: " + message);

                    channel.BasicAck(e.DeliveryTag, false); //RabbitMQ dan gelen mesajı DeliveryTag ile aldığımızı belirtiyoruz. Amaç : Mesajını okunduğunu belirtip ondan sonra silme işlemini yapmak.
                };

                Console.ReadLine();
            }
        }
    }
}
