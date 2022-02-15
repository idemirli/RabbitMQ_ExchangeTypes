using RabbitMQ.Client;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

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
                //channel.ExchangeDeclare("logs-direct", durable: true, type: ExchangeType.Direct);

                //Enum.GetNames(typeof(LogNames)).ToList().ForEach(log =>
                //{
                //    var queueName = $"direct-queue-{log}";
                //    channel.QueueDeclare(queueName, durable: true, exclusive: false, autoDelete: false);
                //    channel.QueueBind(queueName, "logs-direct", $"route-{log}", null);
                //});


                //Enumerable.Range(1, 50).ToList().ForEach(X =>
                //{
                //    LogNames log = (LogNames)new Random().Next(1, 5);
                //    string message = $"log-type {log}";
                //    var routeKey = $"route-{log}";
                //    var messageBody = Encoding.UTF8.GetBytes(message);
                //    channel.BasicPublish("logs-direct", routeKey, null, messageBody);
                //    Console.WriteLine($"Log Gönderilmiştir. {message}");
                //});
                #endregion

                #region [Topic Exchange]
                //Kuyruklar consumer tarafından oluşturulur.
                //Publisher tarafından gelen routing Key tarafından ilgili consumer
                //gelen route'a göre kuyruk oluşturup dinleme işini yapabilir.
                //Gelen root örneğin : critical.error.warning şeklinde olabilir.

                //Akış ; 50 mesaj iletiyorum.Random olarak log Name'e göre route ve mesaj oluşturuyorum.
                //Bunları Publish edip yolluyorum.

                //channel.ExchangeDeclare("logs-topic", durable: true, type: ExchangeType.Topic);
                //Random rnd = new Random();
                //Enumerable.Range(1, 50).ToList().ForEach(X =>
                //{
                //    LogNames log1 = (LogNames)rnd.Next(1, 5);
                //    LogNames log2 = (LogNames)rnd.Next(1, 5);
                //    LogNames log3 = (LogNames)rnd.Next(1, 5);
                //    var routeKey = $"{log1}.{log2}.{log3}";
                //    string message = $"log-type : {log1}-{log2}-{log3}";
                //    var messageBody = Encoding.UTF8.GetBytes(message);
                //    channel.BasicPublish("logs-topic", routeKey, null, messageBody);
                //    Console.WriteLine($"Log Gönderilmiştir. {message}");
                //});



                #endregion

                #region [Header Exchange]

                #region [AÇIKLAMA]
                //Route işlemleri ilgili mesajın Header'ında gönderilir (Key-value şeklinde)
                //header => format=pdf ,  shape=a4 olsun.
                //x-match=any : Herhangi bir değeri sağlıyorsa kuyruğa mesaj gelir ve okuyabilirim
                //x-match=all : Bütün değerleri sağlaması lazım.
                #endregion


                //channel.ExchangeDeclare("header-exchange", durable: true, type: ExchangeType.Headers);
                //Dictionary<string, object> headers = new Dictionary<string, object>();
                //headers.Add("format", "pdf");
                //headers.Add("shape", "a4");

                //headers.Add("shape2", "a4");  //Eğer consumer tarafında x-match:any dersem en az 1 property sağlandığı için mesajı yine gönderecek.

                //routekey Boş gönderiyorum
                //IBasicProperty oluşturacağım.

                //var properties = channel.CreateBasicProperties();
                //properties.Headers = headers;
                //channel.BasicPublish("header-exchange", string.Empty, properties, Encoding.UTF8.GetBytes("header mesajım"));
                #endregion

                #region [Mesajları Kalıcı Hale Getirmek (Örnek : Header Exchange)]

                //#region [AÇIKLAMA]
                ////Mesajları kalıcı hale getirmek için Properties oluşturuyoruz.
                ////Oluşturduğumuz properties'in Persistent alanını true olarak set ediyoruz.
                //#endregion

                //channel.ExchangeDeclare("header-exchange", durable: true, type: ExchangeType.Headers);
                //Dictionary<string, object> headers = new Dictionary<string, object>();
                //headers.Add("format", "pdf");
                //headers.Add("shape", "a4");

                //var properties = channel.CreateBasicProperties();
                //properties.Headers = headers;
                //properties.Persistent = true; // Mesajları kalıcı hale getirmek için
                //channel.BasicPublish("header-exchange", string.Empty, properties, Encoding.UTF8.GetBytes("header mesajım"));
                #endregion

                #region [Complex Type'ları Mesaj Olarak İletmek (Örnek : Header Exchange)]
                channel.ExchangeDeclare("header-exchange", durable: true, type: ExchangeType.Headers);
                Dictionary<string, object> headers = new Dictionary<string, object>();
                headers.Add("format", "pdf");
                headers.Add("shape", "a4");

                var properties = channel.CreateBasicProperties();
                properties.Headers = headers;
                properties.Persistent = true; // Mesajları kalıcı hale getirmek için

                var car = new Car() { id=1, name="Toyota", price=150000, year=2012 };
                var carJsonSerialize = JsonSerializer.Serialize(car);
                channel.BasicPublish("header-exchange", string.Empty, properties, Encoding.UTF8.GetBytes(carJsonSerialize));
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