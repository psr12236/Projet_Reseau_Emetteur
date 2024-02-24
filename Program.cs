class Program
{
    public static void Main()
    {
        string receiverIp = "192.168.0.173";
        int receiverPort = 11000;
        string filePath = @"C:\Users\marcl\source\repos\psr12236\Projet_Reseau\Sender\UDPSender\DataS\fData.txt";
            
        Console.WriteLine("Envoi de données à l'adresse IP " + receiverIp + " sur le port " + receiverPort);
        Console.WriteLine("Appuyez sur 'exit' pour arrêter d'envoyer des fichiers ou sur Entrée pour continuer.");

        while (true)
        {
            string userinput = Console.ReadLine();

            if (userinput.ToLower() == "exit")
            {
                break;
            }

            UdpCommunicationManager sender = new UdpCommunicationManager(receiverIp, receiverPort);
            sender.SendFile(filePath);

            Console.WriteLine("Le fichier a été envoyé : " + filePath);
            Console.WriteLine("Appuyez sur 'exit' pour arrêter d'envoyer des fichiers ou sur Entrée pour continuer.");
        }
    }
}
