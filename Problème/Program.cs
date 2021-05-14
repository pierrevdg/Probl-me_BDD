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
            string requete = "DELETE FROM piece WHERE numP = @numP AND numI_p > 63;";
            MySqlCommand command = maConnexion.CreateCommand();
            command.CommandText = requete;
            command.Parameters.Add(numeroP);
            command.ExecuteReader();
        }
        static void ModificationPiece(string colonne, string nouvelleValeur, string numP, MySqlConnection maConnexion)
        {
            MySqlParameter nouvelleValeur_2 = new MySqlParameter("@nouvelleValeur", MySqlDbType.VarChar);
            nouvelleValeur_2.Value = nouvelleValeur;
            MySqlParameter numP_2 = new MySqlParameter("@numP", MySqlDbType.VarChar);
            numP_2.Value = numP;
            string requete = "UPDATE piece SET " + colonne + " = @nouvelleValeur WHERE numP= @numP;";
            MySqlCommand command = maConnexion.CreateCommand();
            command.CommandText = requete;
            command.Parameters.Add(nouvelleValeur_2);
            command.Parameters.Add(numP_2);
            command.ExecuteReader();
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
        static void CreationPersonne(string adresse, string ville, string codePostal, string province,string nom, string prenom, string tel_p, string courriel_p, int numFidelio, MySqlConnection maConnexion) 
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
        static void CreationBoutique(string adresse, string ville, string codePostal, string province,string nom_b, string tel_b, string courriel_b, string nomContact, int remise, MySqlConnection maConnexion)
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
        static void CreationCommande(int numCommande, string adresse, string ville, string codePostal, string numClient, MySqlConnection maConnexion)
        {
            MySqlParameter numCommande_2 = new MySqlParameter("@numCommande", MySqlDbType.Int32);
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
        static void ModificationCommande(string colonne, string nouvelleValeur,int numCommande, MySqlConnection maConnexion)
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
            //ConsoleKeyInfo cki;
            //Console.WindowHeight = 50;
            //Console.WindowWidth = 100;
            //do
            //{
            //    Console.Clear();
            //    Console.WriteLine("Menu :\n"
            //                     + "1. Dessiner une ligne\n"
            //                     + "2. Dessiner une matrice\n"
            //                     + "Sélectionnez l'action désirée ");
            //    int choix = SaisieNombre();
            //    //rajouter un try pour prendre que les trucs possibles à faire
            //    switch (choix)
            //    {
            //        case 1:
            //            Console.Clear();
            //            //DessineMoiUneLigne(4);
            //            break;
            //        case 2:
            //            Console.Clear();
            //            //DessineMoiUneMatrice('X', 4);
            //            break;
            //    }
            //    Console.WriteLine("Tapez Escape pour sortir ou un numero d'exo");
            //    cki = Console.ReadKey();
            //}
            //while (cki.Key != ConsoleKey.Escape);
            //Console.Read();
            ModificationCommande("adresseL", "RIEN", 1, maConnexion);
            Console.ReadKey();
        }
    }
}
