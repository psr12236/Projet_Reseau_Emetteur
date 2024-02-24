using System;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct Paquet
{
    //Format du paquet
   
    public ushort PacketTotalSize; // Taille totale des données - 16 bits
    public ushort PacketSequenceNumber; // Le numéro de séquence du paquet - 16 bits
    public byte Flags; // 8 bits pour les flags (4 utilisés ici) : Les flags du paquets - SYN - 1 bit / ACK - 1 bit / FIN - 1 bit / RST - 1 bit
    public int DataSize; // Taille des données - 32 bits
    public byte[] Data; // Les données - variable

    public bool IsAck; // Indique si le paquet est un ACK
    public ushort AckFor; // Numéro de séquence du paquet pour lequel cet ACK est envoyé

    // Définir un flag spécifique (activé ou non)
    public void SetFlag(int position, bool value)
    {
        if (value) //si value est vrai (true)
        {
            Flags |= (byte)(1 << position); // Active le flag
        }
        else
        {
            Flags &= (byte)~(1 << position); // Désactive le flag
        }
    }

    // Obtenir un flag spécifique (si activé ou non)
    public bool GetFlag(int position)
    {
        return (Flags & (1 << position)) != 0;
    }

    # region Propriétés pour chaque flag pour faciliter l'accès

    public bool Syn
    {
        get => GetFlag(0);
        set => SetFlag(0, value);
    }

    public bool Ack
    {
        get => GetFlag(1);
        set => SetFlag(1, value);
    }

    public bool Fin
    {
        get => GetFlag(2);
        set => SetFlag(2, value);
    }

    public bool Rst
    {
        get => GetFlag(3);
        set => SetFlag(3, value);
    }

    #endregion

    public static Paquet CreatePaquet(ushort packetSequenceNumber, bool syn, bool ack, bool fin, bool rst,int DataSize, byte[] data)
    {
        Paquet paquet = new Paquet
        {
            PacketSequenceNumber = packetSequenceNumber,
            Syn = syn,
            Ack = ack,
            Fin = fin,
            Rst = rst,
            DataSize = DataSize,
            Data = data,
            PacketTotalSize = (ushort)(5 + (data != null ? data.Length : 0))
        };

        return paquet;
    }



}
