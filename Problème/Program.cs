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
            //Gestion des pièces et des vélos
            #region Test fonction InsertionPiece
            InsertionPiece("S03", maConnexion);
            #endregion
            Console.ReadKey();
        }
    }
}
