Ce ReadMe servira pour plusieurs choses :
	
	1. Vous expliquer comment utiliser le programme
	2. Vous expliquer comment il fonctionne
	3. Nous permettre de vous expliquer comment nous avons travaillé
	4. Garder une trace écrite, intra projet de notre avancée, de comment nous avons travaillé et ce qui nous reste à faire.

Construction du programme
	
		
1. Mise en place de la classe Paquet : https://learn.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.structlayoutattribute.pack?view=net-8.0
		1. Création de la classe Paquet
		2. regarder le Format que doit prendre le paquet

		Le protocole marchera par dessus UDP et sera composé comme suit, dans cet ordre ci:
		En cas d'oublie des tailles : https://learn.microsoft.com/fr-fr/dotnet/csharp/language-reference/builtin-types/integral-numeric-types
		Taille totale des données - 16 bits (ushort pour 16 bits)
		Le numéro de séquence du paquet - 16 bits (ushort pour 16 bits)
		Les flags du paquets (SYN, ACK, FIN, RST) - 4 bits (byte pour 8 bits pas de 4 bits en C#))
		Le flag SYN - 1 bit
		Le flag ACK - 1 bit
		Le flag FIN - 1 bit
		Le flag RST - 1 bit
		Les données - variable
		Tous les nombres sont des entiers non signés.
		Un flag à 1 signifie qu'il est activé.
		
		Pas de check de corruption ici : La corruption du paquet est contrôlée par le checksum UDP, vous pouvez considérer que tous
		les paquets que vous recevez sont intacts.

		3. Getters et Setters


2. Mise en place de la classe Serialise : https://learn.microsoft.com/en-us/dotnet/standard/serialization/

	Pour pouvoir utiliser mon Paquet qui est 'lobjet contenant les données à envoyer,
	je vais le transformer en un tableau de byte pour pouvoir l'envoyer par UDP.
	Il est plutôt recommendé de le faire en Json dans ce que j'ai pu voir, mais pour respecter la demande
	du cours, nous allons essayé de le faire en Binaire.

    SerializePaquet et DeserializePaquet. 
    convertir un objet Paquet en un tableau de bytes (sérialisation)
	reconstruire un objet Paquet à partir d'un tableau de bytes (désérialisation).

    List<byte> paquetBytes = new List<byte>(); --> le conteneur de bytes du paquet

	Une fois la liste faite, j'y ajoute un à un les éléments du paquet, en respectant l'ordre de la construction du paquet.

	Taille totale des données - 16 bits (ushort pour 16 bits) --> paquetBytes.AddRange(BitConverter.GetBytes(paquet.PacketTotalSize));
	Le numéro de séquence du paquet - 16 bits (ushort pour 16 bits) -->paquetBytes.AddRange(BitConverter.GetBytes(paquet.PacketSequenceNumber));
	Les flags du paquets (SYN, ACK, FIN, RST) - 4 bits (byte pour 8 bits pas de 4 bits en C#)) --> paquetBytes.Add(flags);
	Les données - variable --> paquetBytes.AddRange(paquet.PacketData);
