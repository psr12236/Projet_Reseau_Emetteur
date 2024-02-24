using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

class TcpSocketListener
{
    public static void StartListener()
    {
        // Création du socket d'écoute
        Socket listenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        // Liaison du socket à l'adresse IP et au port
        IPAddress ipAddress = IPAddress.Any; // Écoute sur toutes les interfaces réseau
        int port = 11000; // Port d'écoute
        IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port);

        try
        {
            listenerSocket.Bind(localEndPoint);
            listenerSocket.Listen(10); // Définir la longueur de la file d'attente des connexions entrantes

            Console.WriteLine("En attente de connexions...");
            while (true)
            {
                // Accepter une connexion entrante
                Socket handlerSocket = listenerSocket.Accept();

                // Traiter la connexion ici (recevoir/envoyer des données)

                // Fermer la connexion
                handlerSocket.Shutdown(SocketShutdown.Both);
                handlerSocket.Close();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }
}

