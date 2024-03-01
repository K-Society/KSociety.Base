namespace KSociety.Base.EventBusRabbitMQ.Binding
{
    public class ConnectionFactory
    {
        public string MqHostName { get; set; } = "localhost";
        public string MqUserName { get; set; } = "KSociety";
        public string MqPassword { get; set; } = "KSociety";

        public ConnectionFactory()
        {

        }
    }
}
