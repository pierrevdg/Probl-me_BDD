using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Problème
{

    class Program
    {
        //Cette méthode permet de passer d'une date JJ/MM/AAAA à AAAA-MM-JJ
        static string ConversionDate(string date)
        {
            date = date.Substring(0, 10); // pour ne pas récupérer l'heure
            string newDate = "";
            for (int i = 6; i < date.Length; i++)
            {
                newDate += date[i];
            }
            newDate += '-';
            newDate += date[3];
            newDate += date[4];
            newDate += '-';
            newDate += date[0];
            newDate += date[1];
            return newDate;
        }
        static int SaisieNombre()
        {
            int nombre = Convert.ToInt32(Console.ReadLine());
            return nombre;
        }
        // PLUTOT QUE SUPPRESSION PIECE FAIRE MODIFICATION PIECE OU ON AJOUTE LE NUMERO DE LA COMMANDE
        #region Gestion des pièces
        //La fonction prend en argument le numéro de pièce 
        static void InsertionPiece(string numP, MySqlConnection maConnexion)
        {
            #region Récupération du nombre de pièces qui donnera le numéro d'inventaire
            string requete1 = "SELECT COUNT(*) FROM piece;";
            MySqlCommand command1 = maConnexion.CreateCommand();
            command1.CommandText = requete1;
            MySqlDataReader reader1 = command1.ExecuteReader();
            int numI = 0;
            while (reader1.Read())
            {
                numI = Convert.ToInt32(reader1[0]);
            }
            reader1.Close();
            #endregion
            #region Selection de nomP, dateI_p, dateD_p
            MySqlParameter numeroP = new MySqlParameter("@nump", MySqlDbType.VarChar);
            numeroP.Value = numP;
            string requete2 = "SELECT nomP, dateI_p, dateD_p FROM piece WHERE numP = @nump;";
            MySqlCommand command2 = maConnexion.CreateCommand();
            command2.CommandText = requete2;
            command2.Parameters.Add(numeroP);
            MySqlDataReader reader2 = command2.ExecuteReader();

            string[] valueString = new string[reader2.FieldCount];
            while (reader2.Read())
            {
                for (int i = 0; i < reader2.FieldCount; i++)
                {
                    valueString[i] = reader2.GetValue(i).ToString();
                }
                Console.WriteLine();
            }
            reader2.Close();
            #endregion
            #region Insertion 
            string insertTable = "Insert into Piece values (" + numI + ",'"
                                                              + numP + "','"
                                                              + valueString[0] + "',"
                                                              + "null" + ","
                                                              + "null" + ",'"
                                                              + ConversionDate(valueString[1]) + "','"
                                                              + ConversionDate(valueString[2]) + "',"
                                                              + "null" + ","
                                                              + "null" + ");";
            MySqlCommand command3 = maConnexion.CreateCommand();
            command3.CommandText = insertTable;
            try
            {
                command3.ExecuteNonQuery();
            }
            catch (MySqlException e)
            {
                Console.WriteLine(" ErreurConnexion : " + e.ToString());
                Console.ReadLine();
                return;
            }
            #endregion
        }
        static void SuppressionPiece(string numP, MySqlConnection maConnexion)
        {
            MySqlParameter numeroP = new MySqlParameter("@nump", MySqlDbType.VarChar);
            numeroP.Value = numP;
            // numI_p > 63 car lorsque numI_p <= 63 il s'agit du catalogue et non du stock
            string requete = "DELETE FROM piece WHERE numP = @numP AND numI_p > 63 LIMIT 1;";
            MySqlCommand command = maConnexion.CreateCommand();
            command.CommandText = requete;
            command.Parameters.Add(numeroP);
            command.ExecuteReader();
            command.Dispose();
        }
        static void ModificationPiece(string colonne, string nouvelleValeur, string numP, MySqlConnection maConnexion)
        {
            MySqlParameter nouvelleValeur_2 = new MySqlParameter("@nouvelleValeur", MySqlDbType.VarChar);
            nouvelleValeur_2.Value = nouvelleValeur;
            MySqlParameter numP_2 = new MySqlParameter("@numP", MySqlDbType.VarChar);
            numP_2.Value = numP;
            string requete = "UPDATE piece SET " + colonne + " = @nouvelleValeur WHERE numP= @numP AND numI_p > 63 LIMIT 1;";
            //MySqlCommand command = maConnexion.CreateCommand();
            //command.CommandText = requete;
            //command.Parameters.Add(nouvelleValeur_2);
            //command.Parameters.Add(numP_2);
            //command.ExecuteReader();
            MySqlCommand command = maConnexion.CreateCommand();
            command.CommandText = requete;
            try
            {
                command.ExecuteNonQuery();
            }
            catch (MySqlException e)
            {
                Console.WriteLine(" ErreurConnexion : " + e.ToString());
                Console.ReadLine();
                return;
            }
        }
        #endregion
        #region Gestion des velos
        static void SuppressionVelo(string numModele, MySqlConnection maConnexion)
        {
            MySqlParameter numeroModele = new MySqlParameter("@numModele", MySqlDbType.VarChar);
            numeroModele.Value = numModele;
            // numI > 14 car lorsque numI_p <= 14 il s'agit du catalogue et non du stock
            string requete = "DELETE FROM bicyclette WHERE numModele = @numModele AND numI > 14;";
            MySqlCommand command = maConnexion.CreateCommand();
            command.CommandText = requete;
            command.Parameters.Add(numeroModele);
            command.ExecuteReader();
        }
        // A faire
        static void CreationVelo(string numModele, MySqlConnection maConnexion)
        {

        }
        static void ModificationVelo(string colonne, string nouvelleValeur, string numModele, MySqlConnection maConnexion)
        {
            MySqlParameter nouvelleValeur_2 = new MySqlParameter("@nouvelleValeur", MySqlDbType.VarChar);
            nouvelleValeur_2.Value = nouvelleValeur;
            MySqlParameter numModele_2 = new MySqlParameter("@numModele", MySqlDbType.VarChar);
            numModele_2.Value = numModele;
            string requete = "UPDATE bicyclette SET " + colonne + " = @nouvelleValeur WHERE numModele= @numModele;";
            MySqlCommand command = maConnexion.CreateCommand();
            command.CommandText = requete;
            command.Parameters.Add(nouvelleValeur_2);
            command.Parameters.Add(numModele_2);
            command.ExecuteReader();
        }
        #endregion
        #region Gestion des clients en général
        // La fonction suppresionClient permet de supprimer un client et donc de supprimer à la fois une personne et une boutique
        static void SuppressionClient(int numClient, MySqlConnection maConnexion)
        {
            MySqlParameter numClient_2 = new MySqlParameter("@numClient", MySqlDbType.VarChar);
            numClient_2.Value = numClient;
            string requete = "DELETE FROM client WHERE numClient = @numClient;";
            MySqlCommand command = maConnexion.CreateCommand();
            command.CommandText = requete;
            command.Parameters.Add(numClient_2);
            command.ExecuteReader();
        }
        static void ModificationClient(string colonne, string nouvelleValeur, int numClient, MySqlConnection maConnexion)
        {
            MySqlParameter nouvelleValeur_2 = new MySqlParameter("@nouvelleValeur", MySqlDbType.VarChar);
            nouvelleValeur_2.Value = nouvelleValeur;
            MySqlParameter numClient_2 = new MySqlParameter("@numClient", MySqlDbType.Int32);
            numClient_2.Value = numClient;
            string requete = "UPDATE client SET " + colonne + " = @nouvelleValeur WHERE numClient= @numClient;";
            MySqlCommand command = maConnexion.CreateCommand();
            command.CommandText = requete;
            command.Parameters.Add(nouvelleValeur_2);
            command.Parameters.Add(numClient_2);
            command.ExecuteReader();
        }
        #endregion
        #region Gestion des clients particuliers
        static void CreationPersonne(string adresse, string ville, string codePostal, string province, string nom, string prenom, string tel_p, string courriel_p, int numFidelio, MySqlConnection maConnexion)
        {
            #region Récupération du nombre de clients qui donnera le numéro de client
            string requete1 = "SELECT COUNT(*) FROM client;";
            MySqlCommand command1 = maConnexion.CreateCommand();
            command1.CommandText = requete1;
            MySqlDataReader reader1 = command1.ExecuteReader();
            int numClient = 0;
            while (reader1.Read())
            {
                numClient = Convert.ToInt32(reader1[0]) + 1;
            }
            reader1.Close();
            #endregion
            #region Creation du client et de la personne
            MySqlParameter adresse_2 = new MySqlParameter("@adresse", MySqlDbType.VarChar);
            adresse_2.Value = adresse;
            MySqlParameter ville_2 = new MySqlParameter("@ville", MySqlDbType.VarChar);
            ville_2.Value = ville;
            MySqlParameter codePostal_2 = new MySqlParameter("@codePostal", MySqlDbType.VarChar);
            codePostal_2.Value = codePostal;
            MySqlParameter province_2 = new MySqlParameter("@province", MySqlDbType.VarChar);
            province_2.Value = province;
            MySqlParameter nom_2 = new MySqlParameter("@nom", MySqlDbType.VarChar);
            nom_2.Value = nom;
            MySqlParameter prenom_2 = new MySqlParameter("@prenom", MySqlDbType.VarChar);
            prenom_2.Value = prenom;
            MySqlParameter tel_p_2 = new MySqlParameter("@tel_p", MySqlDbType.VarChar);
            tel_p_2.Value = tel_p;
            MySqlParameter courriel_p_2 = new MySqlParameter("@courriel_p", MySqlDbType.VarChar);
            courriel_p_2.Value = courriel_p;
            MySqlParameter numFidelio_2 = new MySqlParameter("@numFidelio", MySqlDbType.Int32);
            numFidelio_2.Value = numFidelio;
            string requete2 = "INSERT INTO client VALUES (" + numClient + ",@adresse,@ville,@codePostal,@province);"
                          + "\nINSERT INTO Personne VALUES (@nom,@prenom," + numClient + ",@tel_p,@courriel_p,@numFidelio,null,null);";
            MySqlCommand command2 = maConnexion.CreateCommand();
            command2.CommandText = requete2;
            command2.Parameters.Add(adresse_2);
            command2.Parameters.Add(ville_2);
            command2.Parameters.Add(codePostal_2);
            command2.Parameters.Add(province_2);
            command2.Parameters.Add(nom_2);
            command2.Parameters.Add(prenom_2);
            command2.Parameters.Add(tel_p_2);
            command2.Parameters.Add(courriel_p_2);
            command2.Parameters.Add(numFidelio_2);
            command2.ExecuteReader();
            #endregion
        }
        // Pour modification personne il faut ajouter le fait que quand on a la colonne = Fidélio alors il faut aussi modfié les dates
        static void ModificationPersonne(string colonne, string nouvelleValeur, string nom, MySqlConnection maConnexion)
        {
            MySqlParameter nouvelleValeur_2 = new MySqlParameter("@nouvelleValeur", MySqlDbType.VarChar);
            nouvelleValeur_2.Value = nouvelleValeur;
            MySqlParameter nom_2 = new MySqlParameter("@nom", MySqlDbType.VarChar);
            nom_2.Value = nom;
            string requete = "UPDATE personne SET " + colonne + " = @nouvelleValeur WHERE nom= @nom;";
            MySqlCommand command = maConnexion.CreateCommand();
            command.CommandText = requete;
            command.Parameters.Add(nouvelleValeur_2);
            command.Parameters.Add(nom_2);
            command.ExecuteReader();
        }
        #endregion
        #region Gestion des Boutiques
        static void CreationBoutique(string adresse, string ville, string codePostal, string province, string nom_b, string tel_b, string courriel_b, string nomContact, int remise, MySqlConnection maConnexion)
        {
            #region Récupération du nombre de clients qui donnera le numéro de client
            string requete1 = "SELECT COUNT(*) FROM client;";
            MySqlCommand command1 = maConnexion.CreateCommand();
            command1.CommandText = requete1;
            MySqlDataReader reader1 = command1.ExecuteReader();
            int numClient = 0;
            while (reader1.Read())
            {
                numClient = Convert.ToInt32(reader1[0]) + 1;
            }
            reader1.Close();
            #endregion
            #region Remplissage des coordonnées de la personne
            MySqlParameter adresse_2 = new MySqlParameter("@adresse", MySqlDbType.VarChar);
            adresse_2.Value = adresse;
            MySqlParameter ville_2 = new MySqlParameter("@ville", MySqlDbType.VarChar);
            ville_2.Value = ville;
            MySqlParameter codePostal_2 = new MySqlParameter("@codePostal", MySqlDbType.VarChar);
            codePostal_2.Value = codePostal;
            MySqlParameter province_2 = new MySqlParameter("@province", MySqlDbType.VarChar);
            province_2.Value = province;
            MySqlParameter nom_b_2 = new MySqlParameter("@nom_b", MySqlDbType.VarChar);
            nom_b_2.Value = nom_b;
            MySqlParameter tel_b_2 = new MySqlParameter("@tel_b", MySqlDbType.VarChar);
            tel_b_2.Value = tel_b;
            MySqlParameter courriel_b_2 = new MySqlParameter("@courriel_b", MySqlDbType.VarChar);
            courriel_b_2.Value = courriel_b;
            MySqlParameter nomContact_2 = new MySqlParameter("@nomContact", MySqlDbType.VarChar);
            nomContact_2.Value = nomContact;
            MySqlParameter remise_2 = new MySqlParameter("@remise", MySqlDbType.Int32);
            remise_2.Value = remise;
            string requete2 = "INSERT INTO client VALUES (" + numClient + ",@adresse,@ville,@codePostal,@province);"
                          + "\nINSERT INTO boutique VALUES (@nom_b,@tel_b,@courriel_b,@nomContact,@remise," + numClient + ");";
            // A VERIFIER (numClient?)
            MySqlCommand command2 = maConnexion.CreateCommand();
            command2.CommandText = requete2;
            command2.Parameters.Add(adresse_2);
            command2.Parameters.Add(ville_2);
            command2.Parameters.Add(codePostal_2);
            command2.Parameters.Add(province_2);
            command2.Parameters.Add(nom_b_2);
            command2.Parameters.Add(tel_b_2);
            command2.Parameters.Add(courriel_b_2);
            command2.Parameters.Add(nomContact_2);
            command2.Parameters.Add(remise_2);
            command2.ExecuteReader();
            #endregion
        }
        static void ModificationBoutique(string colonne, string nouvelleValeur, string nom_b, MySqlConnection maConnexion)
        {
            MySqlParameter nouvelleValeur_2 = new MySqlParameter("@nouvelleValeur", MySqlDbType.VarChar);
            nouvelleValeur_2.Value = nouvelleValeur;
            MySqlParameter nom_b_2 = new MySqlParameter("@nom_b", MySqlDbType.VarChar);
            nom_b_2.Value = nom_b;
            string requete = "UPDATE boutique SET " + colonne + " = @nouvelleValeur WHERE nom_b = @nom_b;";
            MySqlCommand command = maConnexion.CreateCommand();
            command.CommandText = requete;
            command.Parameters.Add(nouvelleValeur_2);
            command.Parameters.Add(nom_b_2);
            command.ExecuteReader();
        }
        #endregion
        #region Gestion des fournisseurs
        static void CreationFournisseur(string numSiret, string nomE, string contact, string adresse_F, int libelle, MySqlConnection maConnexion)
        {
            MySqlParameter numSiret_2 = new MySqlParameter("@numSiret", MySqlDbType.VarChar);
            numSiret_2.Value = numSiret;
            MySqlParameter nomE_2 = new MySqlParameter("@nomE", MySqlDbType.VarChar);
            nomE_2.Value = nomE;
            MySqlParameter contact_2 = new MySqlParameter("@contact", MySqlDbType.VarChar);
            contact_2.Value = contact;
            MySqlParameter adresse_F_2 = new MySqlParameter("@adresse_F", MySqlDbType.VarChar);
            adresse_F_2.Value = adresse_F;
            MySqlParameter libelle_2 = new MySqlParameter("@libelle", MySqlDbType.Int32);
            libelle_2.Value = libelle;
            string requete = "INSERT INTO fournisseur VALUES (@numSiret,@nomE,@contact,@adresse_F,@libelle);";
            MySqlCommand command = maConnexion.CreateCommand();
            command.CommandText = requete;
            command.Parameters.Add(numSiret_2);
            command.Parameters.Add(nomE_2);
            command.Parameters.Add(contact_2);
            command.Parameters.Add(adresse_F_2);
            command.Parameters.Add(libelle_2);
            command.ExecuteReader();
        }
        static void SuppresionFournisseur(string numSiret, MySqlConnection maConnexion)
        {
            MySqlParameter numSiret_2 = new MySqlParameter("@numSiret", MySqlDbType.VarChar);
            numSiret_2.Value = numSiret;
            string requete = "DELETE FROM fournisseur WHERE numSiret=@numSiret;";
            MySqlCommand command = maConnexion.CreateCommand();
            command.CommandText = requete;
            command.Parameters.Add(numSiret_2);
            command.ExecuteReader();
        }
        static void ModificationFournisseur(string colonne, string nouvelleValeur, string numSiret, MySqlConnection maConnexion)
        {
            MySqlParameter nouvelleValeur_2 = new MySqlParameter("@nouvelleValeur", MySqlDbType.VarChar);
            nouvelleValeur_2.Value = nouvelleValeur;
            MySqlParameter numSiret_2 = new MySqlParameter("@numSiret", MySqlDbType.VarChar);
            numSiret_2.Value = numSiret;
            string requete = "UPDATE fournisseur SET " + colonne + " = @nouvelleValeur WHERE numSiret = @numSiret;";
            MySqlCommand command = maConnexion.CreateCommand();
            command.CommandText = requete;
            command.Parameters.Add(nouvelleValeur_2);
            command.Parameters.Add(numSiret_2);
            command.ExecuteReader();
        }
        #endregion
        #region Gestion des commandes
        static void CreationCommande(int numCommande, string adresse, string ville, string codePostal, int numClient, MySqlConnection maConnexion)
        {
            MySqlParameter numCommande_2 = new MySqlParameter("@numCommande", MySqlDbType.VarChar);
            numCommande_2.Value = numCommande;
            MySqlParameter adresse_2 = new MySqlParameter("@adresse", MySqlDbType.VarChar);
            adresse_2.Value = adresse;
            MySqlParameter ville_2 = new MySqlParameter("@ville", MySqlDbType.VarChar);
            ville_2.Value = ville;
            MySqlParameter codePostal_2 = new MySqlParameter("@codePostal", MySqlDbType.VarChar);
            codePostal_2.Value = codePostal;
            MySqlParameter numClient_2 = new MySqlParameter("@numClient", MySqlDbType.VarChar);
            numClient_2.Value = numClient;
            DateTime dateActuelle = DateTime.Now;
            string dateC = ConversionDate(dateActuelle.ToString());
            DateTime dateLivraison = dateActuelle.AddDays(3); // on considère que la livraison prend 3 jours
            string dateL = ConversionDate(dateLivraison.ToString());
            string requete = "INSERT INTO Bon_de_commande VALUES (@numCommande,@adresse,'" + dateL + "',@ville,@codePostal,'" + dateC + "',@numclient);";
            MySqlCommand command = maConnexion.CreateCommand();
            command.CommandText = requete;
            command.Parameters.Add(numCommande_2);
            command.Parameters.Add(adresse_2);
            command.Parameters.Add(ville_2);
            command.Parameters.Add(codePostal_2);
            command.Parameters.Add(numClient_2);
            command.ExecuteReader();
        }
        static void SuppressionCommande(int numCommande, MySqlConnection maConnexion)
        {
            MySqlParameter numCommande_2 = new MySqlParameter("@numCommande", MySqlDbType.Int32);
            numCommande_2.Value = numCommande;
            string requete = "DELETE FROM bon_de_commande WHERE numCommande=@numCommande;";
            MySqlCommand command = maConnexion.CreateCommand();
            command.CommandText = requete;
            command.Parameters.Add(numCommande_2);
            command.ExecuteReader();
        }
        static void ModificationCommande(string colonne, string nouvelleValeur, int numCommande, MySqlConnection maConnexion)
        {
            MySqlParameter nouvelleValeur_2 = new MySqlParameter("@nouvelleValeur", MySqlDbType.VarChar);
            nouvelleValeur_2.Value = nouvelleValeur;
            MySqlParameter numCommande_2 = new MySqlParameter("@numCommande", MySqlDbType.Int32);
            numCommande_2.Value = numCommande;
            string requete = "UPDATE bon_de_commande SET " + colonne + " = @nouvelleValeur WHERE numCommande = @numCommande;";
            MySqlCommand command = maConnexion.CreateCommand();
            command.CommandText = requete;
            command.Parameters.Add(nouvelleValeur_2);
            command.Parameters.Add(numCommande_2);
            command.ExecuteReader();
        }
        #endregion
        // Problème dans les moyennes piece et vélos, il faut prendre en compte également les commandes où il y a juste des vélos commandés dans la moyenne pièce et inversement pour la moyenne vélo
        #region Moyenne du nombre de pièces par commande
        static double MoyennePieceCommande(MySqlConnection maConnexion)
        {
            string requete = "SELECT COUNT(*)/COUNT(DISTINCT p_numCommande) FROM Piece WHERE p_numCommande IS NOT NULL;";
            MySqlCommand command = maConnexion.CreateCommand();
            command.CommandText = requete;
            MySqlDataReader reader = command.ExecuteReader();
            double moyenne = 0;
            while (reader.Read())
            {
                moyenne = Convert.ToDouble(reader[0]);
            }
            reader.Close();
            return moyenne;
        }
        #endregion
        #region Moyenne du nombre de vélos par commande
        static double MoyenneVeloCommande(MySqlConnection maConnexion)
        {
            string requete = "SELECT COUNT(*)/COUNT(DISTINCT numCommande) FROM Bicyclette WHERE numCommande IS NOT NULL;";
            MySqlCommand command = maConnexion.CreateCommand();
            command.CommandText = requete;
            MySqlDataReader reader = command.ExecuteReader();
            double moyenne = 0;
            while (reader.Read())
            {
                moyenne = Convert.ToDouble(reader[0]);
            }
            reader.Close();
            return moyenne;
        }
        #endregion
        //Requête pas fonctionnel ici
        #region Moyenne des montants de commande
        static void MoyenneCommande(MySqlConnection maConnexion)
        {
            string requete = "SELECT SUM(prixV+prixP)/COUNT(DISTINCT numCommande) AS moyenne_commande FROM Bicyclette B, Piece P, Envoi E WHERE P.numI_p = E.numI_p AND numCommande IS NOT NULL AND p_numCommande IS NOT NULL;";
            MySqlCommand command = maConnexion.CreateCommand();
            command.CommandText = requete;

            MySqlDataReader reader = command.ExecuteReader();

            string[] valueString = new string[reader.FieldCount];
            //Rajouter des lignes ?
            while (reader.Read())
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    valueString[i] = reader.GetValue(i).ToString(); //VERIFIER
                    Console.Write(valueString[i] + " , ");
                }
                Console.WriteLine();
                reader.Close();
                command.Dispose();
            }
        }
        #endregion
        // La requête n'est pas fonctionnelle
        // Chacune des 2 requêtes sans l'union fonctionne mais l'union des 2 retourne simplement la première requête
        #region Meilleurs clients en fonction du nombre de pièces vendues
        static void MeilleursClients(MySqlConnection maConnexion)
        {
            string requete = "SELECT nom, COUNT(P.numI_p) AS nb_pieces FROM Piece P, Personne Pe, Bon_De_Commande B WHERE p_numCommande IS NOT NULL AND Pe.numClient = c_numClient AND B.numCommande = p_numCommande GROUP BY p_numCommande UNION SELECT nom_B, COUNT(P.numI_p) AS nb_pieces FROM Piece P, Bon_De_Commande B, Boutique Bo WHERE p_numCommande IS NOT NULL AND Bo.b_numClient = c_numClient AND B.numCommande = p_numCommande GROUP BY p_numCommande ORDER BY nb_pieces LIMIT 3;";
            MySqlCommand command = maConnexion.CreateCommand();
            command.CommandText = requete;

            MySqlDataReader reader = command.ExecuteReader();

            string[] valueString = new string[reader.FieldCount];
            //Rajouter des lignes ?
            while (reader.Read())
            {
                // Boucle à limiter car récupère 2 colonnes de la ligne
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    valueString[i] = reader.GetValue(i).ToString(); //VERIFIER
                    Console.Write(valueString[i] + " , ");
                }
                Console.WriteLine();
                reader.Close();
                command.Dispose();
            }
        }
        #endregion
        #region Liste des membres pour chaque programme + date d'expiration
        static void StatsFidelio(MySqlConnection maConnexion)
        {
            string requete = "SELECT description_F, nom, dateF FROM Personne NATURAL JOIN fidelio WHERE numFidelio != 0 ORDER BY numFidelio;";
            MySqlCommand command = maConnexion.CreateCommand();
            command.CommandText = requete;
            MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                Console.WriteLine(reader[0] + ", nom : " + reader[1] + ", date d'expiration : " + reader[2].ToString().Substring(0, 10));
            }
            reader.Close();
        }
        #endregion
        #region Quantités vendues de chaque item
        static void QuantitesVendues(MySqlConnection maConnexion)
        {
            string requete = "SELECT nomV AS article, COUNT(*) - 1 AS quantite FROM Bicyclette GROUP BY nomV, grandeur UNION SELECT numP AS article, COUNT(*)-1 AS quantite FROM Piece GROUP BY nomP, numP;";
            MySqlCommand command = maConnexion.CreateCommand();
            command.CommandText = requete;
            MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                Console.WriteLine("Article : " + reader[0] + ", ventes : " + reader[1]);
            }
            reader.Close();
        }
        #endregion
        // Trouver comment ajouter les catégories de vélo où on a 0 de quantité 
        #region Inventaire catégorie de vélos
        static void InventaireCategorieVelos(MySqlConnection maConnexion)
        {
            string requete = "SELECT DISTINCT l_produit AS ligne_produit, COUNT(*) AS quantite FROM Bicyclette WHERE numI > 15 GROUP BY l_produit;";
            MySqlCommand command = maConnexion.CreateCommand();
            command.CommandText = requete;
            MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                Console.WriteLine("Catégorie vélo : " + reader[0] + ", quantité : " + reader[1]);
            }
            reader.Close();
        }
        #endregion
        // Problème avec la requête on ne récupère pas le nom des pièces quand la quantité est égale à 0
        #region Inventaire des pièces
        static void InventairePieces(MySqlConnection maConnexion)
        {
            string requete = "SELECT DISTINCT numP AS numero_modele, COUNT(*) AS quantite FROM Piece WHERE numI_p > 63 AND p_numCommande IS NULL GROUP BY numP;";
            MySqlCommand command = maConnexion.CreateCommand();
            command.CommandText = requete;
            MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                Console.WriteLine("Piece : " + reader[0] + ", quantite : " + reader[1]);
            }
            reader.Close();
        }
        #endregion
        // Même probleème que pour les autres requêtes d'inventaire
        #region Inventaire de vélos
        static void InventaireVelos(MySqlConnection maConnexion)
        {
            string requete = "SELECT DISTINCT nomV, grandeur, COUNT(*) AS quantite FROM Bicyclette WHERE numI > 15 AND numCommande IS NULL GROUP BY nomV, grandeur;";
            MySqlCommand command = maConnexion.CreateCommand();
            command.CommandText = requete;
            MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                Console.WriteLine(reader[0] + ", taille : " + reader[1] + " , quantite : " + reader[2]);
            }
            reader.Close();
        }
        #endregion
        // Pas compris ce que devait donner comme information cette requête
        #region Inventaire fournisseur
        static void InventaireFournisseur(MySqlConnection maConnexion)
        {
            string requete = "SELECT DISTINCT numP, nomP, COUNT(*) AS quantite, contact, libelle FROM Piece P, Fournisseur F, Envoi E WHERE F.numSiret = E.numSiret AND E.numI_p = P.numI_p GROUP BY contact;";
            MySqlCommand command = maConnexion.CreateCommand();
            command.CommandText = requete;

            MySqlDataReader reader = command.ExecuteReader();

            string[] valueString = new string[reader.FieldCount];
            //Rajouter des lignes ?
            while (reader.Read())
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    valueString[i] = reader.GetValue(i).ToString(); //VERIFIER
                    Console.Write(valueString[i] + " , ");
                }
                Console.WriteLine();
                reader.Close();
                command.Dispose();
            }
        }
        #endregion
        static void Main(string[] args)
        {
            #region Ouverture de connexion
            MySqlConnection maConnexion = null;
            try
            {
                string connexionString = "SERVER=localhost;PORT=3306;" +
                                         "DATABASE=veloMax;" +
                                         "UID=root;PASSWORD=Ropie?!mlb78"; //met ton mot de passe si tu veux accéder à ta bdd sql
                                                                           // faudra d'ailleurs qu'on ajoute le mot de passe root pour rendre le problème

                maConnexion = new MySqlConnection(connexionString);
                maConnexion.Open();
            }
            catch (MySqlException e)
            {
                Console.WriteLine(" ErreurConnexion : " + e.ToString());
                return;
            }
            #endregion
            #region Menu
            ConsoleKeyInfo cki;
            Console.WindowHeight = 50;
            Console.WindowWidth = 100;
            do
            {
                Console.Clear();
                Console.WriteLine("Menu :\n"
                                 + "1. Passer une commande (pour un client)\n"
                                 + "2. Passer une commande (vers un fournisseur)\n"
                                 + "3. Ajouter un client à la base de données"
                                 + "3. Aperçu des stocks\n"
                                 + "4. Modification d'informations\n"
                                 + "5. Module statistiques\n"
                                 + "6. Démo\n"
                                 + "7. TEST\n"
                                 + "Sélectionnez l'action désirée ");
                int choix = SaisieNombre();
                //rajouter un try pour prendre que les trucs possibles à faire
                switch (choix)
                {
                    // NON FOCNTIONNEL CAR MODIFICATION PIECE N'EST PAS FOCNTIONNEL
                    #region Passer une commande (pour un client)
                    case 1:
                        bool verif = true;
                        while (verif == true)
                        {
                            #region Récupération du numéro de commande
                            string requete1 = "SELECT COUNT(*) FROM bon_de_commande;";
                            MySqlCommand command1 = maConnexion.CreateCommand();
                            command1.CommandText = requete1;
                            MySqlDataReader reader1 = command1.ExecuteReader();
                            int numCommande = 0;
                            while (reader1.Read())
                            {
                                numCommande = Convert.ToInt32(reader1[0]) + 1;
                            }
                            reader1.Close();
                            #endregion
                            Console.Clear();
                            Console.WriteLine("Commander :\n"
                                                   + "1. Une pièce\n"
                                                   + "2. Un vélo");
                            int choix1 = SaisieNombre();
                            switch (choix1)
                            {
                                case 1:
                                    // PROBLEME : ON A DEJA UN OPEN READER D'OUVERT MAIS J'ARRIVE PAS A TROUVER OU
                                    Console.Clear();
                                    Console.WriteLine("Enter un numéro de pièce :");
                                    string numP = Convert.ToString(Console.ReadLine());
                                    // Tester si la valeur entrée est bien un numéro de pièce 
                                    // On regarde si le stock est suffisant :
                                    MySqlParameter numP_2 = new MySqlParameter("@numP", MySqlDbType.VarChar);
                                    numP_2.Value = numP;
                                    string requete2 = "SELECT COUNT(*)-1 FROM Piece WHERE numP=@numP AND p_numCommande IS NULL;";
                                    MySqlCommand command2 = maConnexion.CreateCommand();
                                    command2.CommandText = requete2;
                                    command2.Parameters.Add(numP_2);
                                    MySqlDataReader reader2 = command2.ExecuteReader();
                                    int stock = 0;
                                    while (reader2.Read())
                                    {
                                        stock = Convert.ToInt32(reader2[0]);
                                    }
                                    reader2.Close();
                                    if (stock == 0)
                                    {
                                        //Passer une commande au près d'un fournisseur pour cette pièce 
                                    }
                                    else
                                    {
                                        // Le fait de demander l'adresse et tout peut-être le rajouter dans la fonction CreationCommande plutôt que dans le main
                                        Console.WriteLine("Si vous n'êtes pas encore client chez nous, penser à d'abord vous créez un compte");
                                        Console.WriteLine("Quelle est votre numéro de client :");
                                        int numClient = Convert.ToInt32(Console.ReadLine());
                                        Console.Clear();
                                        Console.WriteLine("A quelle adresse voulez-vous être livrer (numéro et rue) :");
                                        string adresse = Convert.ToString(Console.ReadLine());
                                        Console.Clear();
                                        Console.WriteLine("Dans quelle ville se trouve l'adresse ? :");
                                        string ville = Convert.ToString(Console.ReadLine());
                                        Console.Clear();
                                        Console.WriteLine("Quel est le code postal ? :");
                                        string codePostal = Convert.ToString(Console.ReadLine());
                                        Console.Clear();
                                        CreationCommande(numCommande, adresse, ville, codePostal, numClient, maConnexion);
                                        //ModificationPiece("p_numCommande", Convert.ToString(numCommande), numP, maConnexion);
                                        Console.WriteLine("Commande effectué de " + numP + " avec succès !");
                                        //Console.ReadKey();
                                        if (stock <= 1)
                                        {
                                            Console.WriteLine("Mr.Legrand, penser à racheter la pièce : " + numP);
                                            Console.ReadKey();
                                        }
                                    }
                                    break;
                                case 7:
                                    break;
                            }
                        };
                        break;
                    #endregion
                    // PAS FAIT
                    #region Passer une commande (vers un fournisseur)
                    case 2:
                        Console.Clear();
                        break;
                    #endregion
                    #region Ajouter un client à la base de données
                    case 3:
                        Console.Clear();
                        Console.WriteLine("Quel type de client êtes-vous ?\n"
                                         + "1. Un particulier\n"
                                         + "2. Une boutique\n");
                        int choix3 = SaisieNombre();
                        Console.Clear();
                        Console.WriteLine("Quelle est votre adresse ? (numéro + rue) :");
                        string adresse1 = Convert.ToString(Console.ReadLine());
                        Console.Clear();
                        Console.WriteLine("Quelle est la ville ? :");
                        string ville1 = Convert.ToString(Console.ReadLine());
                        Console.Clear();
                        Console.WriteLine("Quel est le code postal ? :");
                        string codePostal1 = Convert.ToString(Console.ReadLine());
                        Console.Clear();
                        Console.WriteLine("Quelle est la province ? :");
                        string province1 = Convert.ToString(Console.ReadLine());
                        Console.Clear();
                        Console.WriteLine("Quel est votre numéro de téléphone ? :");
                        string tel1 = Convert.ToString(Console.ReadLine());
                        Console.Clear();
                        Console.WriteLine("Quelle est votre adresse mail ? :");
                        string courriel1 = Convert.ToString(Console.ReadLine());
                        Console.Clear();
                        switch (choix3)
                        {
                            case 1:
                                Console.WriteLine("Quelle est votre nom ? :");
                                string nom1 = Convert.ToString(Console.ReadLine()).ToUpper();
                                Console.Clear();
                                Console.WriteLine("Quel est votre prénom ? :");
                                string prenom1 = Convert.ToString(Console.ReadLine());
                                Console.Clear();
                                //Ajouter le fait qu'une personne peut suivre un programme Fidelio en s'inscrivant
                                //Console.WriteLine("Désirez vous suivre un programme Fidélio");
                                CreationPersonne(adresse1, ville1, codePostal1, province1, nom1, prenom1, tel1, courriel1, 0, maConnexion);
                                Console.WriteLine("Bienvenue : " + nom1 + " " + prenom1);
                                break;
                            case 2:
                                Console.WriteLine("Quel est le nom de votre boutique ? :");
                                string nom_b1 = Convert.ToString(Console.ReadLine());
                                Console.Clear();
                                Console.WriteLine("Entrer le nom de votre contact :");
                                string contact1 = Convert.ToString(Console.ReadLine());
                                Console.Clear();
                                CreationBoutique(adresse1, ville1, codePostal1, province1, nom_b1, tel1, courriel1, contact1, 0, maConnexion);
                                Console.WriteLine("Bienvenue : " + nom_b1);
                                break;
                        }
                        break;
                        #endregion
                }
                Console.WriteLine("Tapez Escape pour retourner au menu principal");
                cki = Console.ReadKey();
            }
            while (cki.Key != ConsoleKey.Escape);
            Console.Read();
            #endregion
        }
    }
}
