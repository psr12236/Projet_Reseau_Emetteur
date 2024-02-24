using System;
using System.Collections.Generic;
using System.Linq;

public class DataSerialise
{
    public byte[] SerializePaquet(Paquet paquet)
    {
        // Initialisation d'une liste pour stocker les bytes du paquet
        List<byte> paquetBytes = new List<byte>();

        // Conversion de la taille totale du paquet et du numéro de séquence en bytes et ajout à la liste
        paquetBytes.AddRange(BitConverter.GetBytes(paquet.PacketTotalSize));
        paquetBytes.AddRange(BitConverter.GetBytes(paquet.PacketSequenceNumber));

        // Initialisation d'un byte à 0 pour stocker les flags
        byte flags = 0;

        // Configuration des bits individuels basée sur les états des propriétés booléennes de l'objet paquet
        // Chaque propriété (Syn, Ack, Fin, Rst) active un bit différent dans le byte des flags
        if (paquet.Syn) flags |= 1 << 0; // Active le bit 0 si Syn est vrai
        if (paquet.Ack) flags |= 1 << 1; // Active le bit 1 si Ack est vrai
        if (paquet.Fin) flags |= 1 << 2; // Active le bit 2 si Fin est vrai
        if (paquet.Rst) flags |= 1 << 3; // Active le bit 3 si Rst est vrai

        // Ajout du byte des flags à la liste des bytes du paquet
        paquetBytes.Add(flags);

        // Si des données sont présentes, les ajouter à la liste des bytes du paquet
        if (paquet.Data != null && paquet.Data.Length > 0)
        {
            paquetBytes.AddRange(paquet.Data);
        }

        // Retourne le tableau de bytes représentant le paquet sérialisé
        return paquetBytes.ToArray();
    }

    public Paquet DeserializePaquet(byte[] data)
    {
        // Création d'un nouvel objet Paquet pour stocker les données désérialisées
        Paquet paquet = new Paquet();
        int offset = 0; // Utilisé pour suivre la position actuelle dans le tableau de bytes

        // Lecture de la taille totale et du numéro de séquence du paquet à partir des bytes et mise à jour de l'offset
        paquet.PacketTotalSize = BitConverter.ToUInt16(data, offset);
        offset += 2; // Taille de UInt16 est 2 bytes
        paquet.PacketSequenceNumber = BitConverter.ToUInt16(data, offset);
        offset += 2;

        // Lecture du byte des flags et mise à jour de l'offset
        byte flags = data[offset++];

        // Configuration des propriétés booléennes de l'objet paquet en fonction des bits du byte des flags
        paquet.Syn = (flags & (1 << 0)) != 0; // Vérifie si le bit 0 est activé
        paquet.Ack = (flags & (1 << 1)) != 0; // Vérifie si le bit 1 est activé
        paquet.Fin = (flags & (1 << 2)) != 0; // Vérifie si le bit 2 est activé
        paquet.Rst = (flags & (1 << 3)) != 0; // Vérifie si le bit 3 est activé

        // Calcul de la taille des données et extraction des données du tableau de bytes
        paquet.Data = new byte[paquet.PacketTotalSize - 5]; // 5 est la somme des bytes utilisés pour la taille, le numéro de séquence et les flags
        if (paquet.Data.Length > 0)
        {
            Array.Copy(data, 5, paquet.Data, 0, paquet.Data.Length); // Copie les données dans l'objet paquet
        }

        // Retourne l'objet paquet désérialisé
        return paquet;
    }
}
