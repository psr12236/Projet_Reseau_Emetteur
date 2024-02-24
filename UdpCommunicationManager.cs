using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading;

public class UdpCommunicationManager
{
    private Socket socket;
    private Dictionary<ushort, Paquet> sentPackets; // Pour suivre les paquets envoyés qui attendent un ACK
    private Timer retransmissionTimer; // Timer pour la retransmission
    private IPEndPoint remoteEndPoint;

    public UdpCommunicationManager(string remoteIp, int remotePort)
    {
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        remoteEndPoint = new IPEndPoint(IPAddress.Parse(remoteIp), remotePort);
        sentPackets = new Dictionary<ushort, Paquet>();
    }

    public void StartRetransmissionTimer()
    {
        retransmissionTimer = new Timer(Retransmit, null, 0, 1000); // Ajustez la période selon vos besoins
    }

    private void Retransmit(object state)
    {
        var packetsToRetransmit = new List<ushort>();

        // Identifier les paquets non acquittés
        foreach (var packetEntry in sentPackets)
        {
            // Ajoutez votre logique pour déterminer si un paquet doit être retransmis,
            // par exemple, en fonction du temps écoulé depuis l'envoi.
            packetsToRetransmit.Add(packetEntry.Key);
        }

        // Retransmettre les paquets
        foreach (var sequenceNumber in packetsToRetransmit)
        {
            if (sentPackets.TryGetValue(sequenceNumber, out Paquet paquet))
            {
                Console.WriteLine($"Retransmitting packet {sequenceNumber}");
                SendPacket(paquet);
            }
        }
    }

    public void SendFile(string filePath)
    {
        byte[] fileData = File.ReadAllBytes(filePath);
        int bytesSent = 0;
        ushort packetNumber = 0;

        while (bytesSent < fileData.Length)
        {
            int bytesLeft = fileData.Length - bytesSent;
            int packetSize = Math.Min(bytesLeft, 1024); // taille maximale de 1024 octets par paquet
            byte[] packetData = new byte[packetSize];

            Array.Copy(fileData, bytesSent, packetData, 0, packetSize);
            Paquet paquet = new Paquet
            {
                PacketTotalSize = (ushort)(packetSize + 5), // +5 pour les en-têtes (PacketTotalSize, PacketSequenceNumber, Flags)
                PacketSequenceNumber = packetNumber,
                Data = packetData,
                // Configurer les autres champs si nécessaire
            };

            SendPacket(paquet);
            bytesSent += packetSize;
            packetNumber++;
        }
    }


    public void SendPacket(Paquet paquet)
    {
        var data = new DataSerialise().SerializePaquet(paquet);
        socket.SendTo(data, remoteEndPoint);

        if (!paquet.IsAck) // Si ce n'est pas un ACK, attendez un ACK en retour
        {
            sentPackets.Add(paquet.PacketSequenceNumber, paquet);
        }
    }

    public void ReceivePacket()
    {
        var buffer = new byte[1024]; // Ajustez la taille selon vos besoins
        EndPoint senderRemote = new IPEndPoint(IPAddress.Any, 0);

        while (true)
        {
            int received = socket.ReceiveFrom(buffer, ref senderRemote);
            var paquet = new DataSerialise().DeserializePaquet(buffer);

            if (paquet.IsAck)
            {
                // Traitez l'ACK reçu
                if (sentPackets.ContainsKey(paquet.AckFor))
                {
                    // ACK reçu, retirez le paquet de sentPackets
                    sentPackets.Remove(paquet.AckFor);
                }
            }
            else
            {
                // Traitez le paquet reçu et envoyez un ACK en réponse si nécessaire
                SendAck(paquet.PacketSequenceNumber, senderRemote);
            }
        }
    }

    private void SendAck(ushort packetSequenceNumber, EndPoint remote)
    {
        Paquet ackPacket = new Paquet
        {
            IsAck = true,
            AckFor = packetSequenceNumber,
            // Assurez-vous de configurer correctement les autres champs nécessaires
        };

        // Serialisez et envoyez le paquet ACK
        var ackData = new DataSerialise().SerializePaquet(ackPacket);
        socket.SendTo(ackData, remote);
    }

}
