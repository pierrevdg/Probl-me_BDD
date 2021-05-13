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
        #region Gestion des pièces et des vélos
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
        static void CreationVelo(string numModele, MySqlConnection maConnexion)
        {
            
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
            SuppresionFournisseur("55555555555555", maConnexion);
            Console.ReadKey();
        }
    }
}
