using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Parser;

namespace RxTap.LocalData
{
    class LocalDataManager
    {
        private static LocalDataManager instance = null;
        private static readonly object padlock = new object();
        private const string userSettingsFileName = "USER_SETTINGS.txt";
        private const string prescriptionFileName = "USER_PRESCRIPTIONS.txt";
        private const string dosageFileName = "USER_DOSAGES.txt";

        public static LocalDataManager Instance
        {
            get
            {
                lock(padlock)
                {
                    if (instance == null)
                    {
                        instance = new LocalDataManager();
                    }
                    return instance;
                }
            }
        }

        LocalDataManager()
        {

        }

        //save username
        public void SaveUser(string data)
        {
            //get filepath of prescription file located in internal storage
            string internalStoragePath = Application.Context.FilesDir.Path;
            string userSettingsPath = Path.Combine(internalStoragePath, userSettingsFileName);

            //if file doesn't exist, create it
            if (!File.Exists(userSettingsPath))
            {
                using (FileStream fileStream = File.Create(userSettingsPath))
                {
                }
            }

            //save username
            File.WriteAllText(userSettingsPath, data);
        }

        //get username
        public string GetUsername()
        {
            //get filepath of prescription file located in internal storage
            string internalStoragePath = Application.Context.FilesDir.Path;
            string userSettingsPath = Path.Combine(internalStoragePath, userSettingsFileName);

            //if file doesn't exist, create it
            if (!File.Exists(userSettingsPath))
            {
                using (FileStream fileStream = File.Create(userSettingsPath))
                {
                }
                File.WriteAllText(userSettingsPath, "User");
            }

            //save username
            return File.ReadAllText(userSettingsPath);
        }


        //retrieves the prescription ID given a string of raw prescription data (in CSV format)
        private int GetRXIDFromPrescriptionData(string prescriptionData)
        {
            ParserData newParserData = new ParserData(prescriptionData);
            return newParserData.prescriptionID;
        }

        //retrieves the prescription ID given a string of raw dosage data (in CSV format)
        private int GetRXIDFromDosageData(string dosageData)
        {
            //raw data separated by ',' character
            char delimiter = ',';
            string[] parsedData = dosageData.Split(delimiter);
            foreach (var substring in parsedData)
                System.Console.WriteLine(substring);

            return Int32.Parse(parsedData[1]);
        }

        //retrieve all prescription data
        public string[] GetAllPrescriptionData()
        {
            //get filepath of prescription file located in internal storage
            string internalStoragePath = Application.Context.FilesDir.Path;
            string prescriptionPath = Path.Combine(internalStoragePath, prescriptionFileName);

            //return null if file doesn't exist
            if (!File.Exists(prescriptionPath))
                return null;

            return File.ReadAllLines(prescriptionPath);
        }

        //returns a list of all the user's prescriptions
        public List<Prescription> GetAllPrescriptions()
        {
            List<Prescription> prescriptionList = new List<Prescription>();
            foreach(string p in GetAllPrescriptionData())
            {
                ParserData parse = new ParserData(p);
                prescriptionList.Add(parse.CreatePrescription());
            }

            return prescriptionList;
        }

        public void DeletePrescription(int pID)
        {
            if (DoesPrescriptionExist(pID))
            {
                //get filepath of prescription file located in internal storage
                string internalStoragePath = Application.Context.FilesDir.Path;
                string prescriptionPath = Path.Combine(internalStoragePath, prescriptionFileName);

                //return null if file doesn't exist
                if (!File.Exists(prescriptionPath))
                    return;

                string[] prescriptionData = File.ReadAllLines(prescriptionPath);

                //if file doesn't exist, create it
                if (File.Exists(prescriptionPath))
                {
                    File.WriteAllText(prescriptionPath, "");
                }

                foreach (string prescriptionLine in prescriptionData)
                {
                    //if prescription ID matches pID, delete prescription
                    if (GetRXIDFromPrescriptionData(prescriptionLine) != pID)
                    {
                        AddNewPrescription(prescriptionLine);
                    }
                }
            }
        }

        //checks whether a prescription exists given its unique prescription ID
        public bool DoesPrescriptionExist(int pID)
        {
            //get filepath of prescription file located in internal storage
            string internalStoragePath = Application.Context.FilesDir.Path;
            string prescriptionPath = Path.Combine(internalStoragePath, prescriptionFileName);

            //return null if file doesn't exist
            if (!File.Exists(prescriptionPath))
                return false;

            string[] prescriptionData = File.ReadAllLines(prescriptionPath);
            foreach (string prescriptionLine in prescriptionData)
            {
                //if prescription ID matches pID, return the data of the current prescription
                if (GetRXIDFromPrescriptionData(prescriptionLine) == pID)
                {
                    ParserData newParserData = new ParserData(prescriptionLine);
                    return true;
                }
            }
            //if no matches found, return null
            return false;
        }

        //retrieves a single, unique prescription using the prescription ID
        public Prescription GetPrescription(int pID)
        {
            //get filepath of prescription file located in internal storage
            string internalStoragePath = Application.Context.FilesDir.Path;
            string prescriptionPath = Path.Combine(internalStoragePath, prescriptionFileName);

            //return null if file doesn't exist
            if (!DoesPrescriptionExist(pID))
                return null;

            string[] prescriptionData = File.ReadAllLines(prescriptionPath);
            foreach(string prescriptionLine in prescriptionData)
            {
                //if prescription ID matches pID, return the data of the current prescription
                if(GetRXIDFromPrescriptionData(prescriptionLine) == pID)
                {
                    ParserData newParserData = new ParserData(prescriptionLine);
                    return newParserData.CreatePrescription();
                }
            }
            //if no matches found, return null
            return null;
        }

        //save prescription data
        public void AddNewPrescription(string data)
        {
            //get filepath of prescription file located in internal storage
            string internalStoragePath = Application.Context.FilesDir.Path;
            string prescriptionPath = Path.Combine(internalStoragePath, prescriptionFileName);

            //if file doesn't exist, create it
            if (!File.Exists(prescriptionPath))
            {
                using(FileStream fileStream = File.Create(prescriptionPath))
                {
                }
            }

            //if this is an unregistered prescription, append prescription data as a single line
            if (!DoesPrescriptionExist(GetRXIDFromPrescriptionData(data)))
                File.AppendAllText(prescriptionPath, data + System.Environment.NewLine);
            else
                Console.WriteLine("PRESCRIPTION ALREADY EXISTS.");

            // Open the file to read from.
            //string[] readText = File.ReadAllLines(prescriptionPath);
            //Console.WriteLine("ESTHER THIS IS THE PRESCRIPTION DATA:");
            //foreach (string s in readText)
            //{
            //    Console.WriteLine(s);
            //}
        }

        //clear all of the user's prescription data
        public void ClearAllPrescriptions()
        {
            //get filepath of prescription file located in internal storage
            string internalStoragePath = Application.Context.FilesDir.Path;
            string prescriptionPath = Path.Combine(internalStoragePath, prescriptionFileName);

            //if file doesn't exist, create it
            if (File.Exists(prescriptionPath))
            {
                File.WriteAllText(prescriptionPath, "");
            }

            //TODO: DELETE LATER (HARD CODING PREFERRED TIMES FOR DEMO)
            AddNewPrescription("9876543,98135468,Johnson & Johnson,Tylenol,Acetaminophen,100,mg,100,t,Take dose or as directed,Rexall,10 Oxford Rd,5196981734,Esther,Kim,021218,1,day,99,021118,Dr. Phil");
        }

        //DOSAGE FUNCTIONS =================================================================

        //retrieve all prescription data
        public string[] GetAllTakenDosageData()
        {
            //get filepath of prescription file located in internal storage
            string internalStoragePath = Application.Context.FilesDir.Path;
            string dosagePath = Path.Combine(internalStoragePath, dosageFileName);

            //return null if file doesn't exist
            if (!File.Exists(dosagePath))
                return null;

            return File.ReadAllLines(dosagePath);
        }

        //returns a list of all the user's prescriptions
        public List<TakenDosage> GetAllTakenDosages()
        {
            //string format is: dosageID, RXID, dateTaken, hour, minute, second

            List<TakenDosage> dosageList = new List<TakenDosage>();
            foreach (string d in GetAllTakenDosageData())
            {
                //raw data separated by ',' character
                char delimiter = ',';
                string[] parsedData = d.Split(delimiter);
                foreach (var substring in parsedData)
                    System.Console.WriteLine(substring);

                //Dosage ID
                int dosageID = Int32.Parse(parsedData[0]);

                //RXID
                int RXID = Int32.Parse(parsedData[1]);

                //Date Taken
                string p = parsedData[2];

                DateTime dateTaken = Convert.ToDateTime(p);
                

                //Time Taken
                DateTime timeTaken = new DateTime(dateTaken.Year, dateTaken.Month, dateTaken.Day, Int32.Parse(parsedData[3]), Int32.Parse(parsedData[4]), Int32.Parse(parsedData[5]));

                dosageList.Add(new TakenDosage(dosageID,RXID,dateTaken,timeTaken));
            }

            return dosageList;
        }

        //save dosage data
        public void AddNewTakenDosage(string data)
        {
            //get filepath of prescription file located in internal storage
            string internalStoragePath = Application.Context.FilesDir.Path;
            string dosagePath = Path.Combine(internalStoragePath, dosageFileName);

            //if file doesn't exist, create it
            if (!File.Exists(dosagePath))
            {
                using (FileStream fileStream = File.Create(dosagePath))
                {
                }
            }

            File.AppendAllText(dosagePath, data + System.Environment.NewLine);
        }

        public List<TakenDosage> GetTodaysDosesForPrescription(int pID)
        {
            //get filepath of prescription file located in internal storage
            string internalStoragePath = Application.Context.FilesDir.Path;
            string dosagePath = Path.Combine(internalStoragePath, dosageFileName);
            
            //return null if file doesn't exist
            if (!File.Exists(dosagePath))
                return null;

            List<TakenDosage> todaysDoses = new List<TakenDosage>();
            string[] dosageData = File.ReadAllLines(dosagePath);
            foreach (TakenDosage dose in GetAllTakenDosages())
            {
                //if prescription ID matches pID, and dateTaken matches today's date, add dosage to the list
                if (dose.RXID == pID && dose.DateTaken.Date == DateTime.Now.Date )
                {
                    todaysDoses.Add(dose);
                }
            }

            return todaysDoses;

        }

        public bool IsThereAvailableDoses(ParserData parse)
        {
            //get today's taken dosages
            List<TakenDosage> doses = GetTodaysDosesForPrescription(parse.prescriptionID);
            if (doses == null || (doses != null && doses.Count < parse.frequencyQty))
                return true;

            return false;
        }

        public bool IsThereAvailableDoses(int pID)
        {
            //get today's taken dosages
            List<TakenDosage> doses = GetTodaysDosesForPrescription(pID);
            Prescription p = GetPrescription(pID);
            if (doses == null || (doses != null && doses.Count < p.Direction.FrequencyQty))
                return true;

            return false;
        }

        //clear all of the user's prescription data
        public void ClearAllTakenDosages()
        {
            //get filepath of prescription file located in internal storage
            string internalStoragePath = Application.Context.FilesDir.Path;
            string dosagePath = Path.Combine(internalStoragePath, dosageFileName);

            //if file doesn't exist, create it
            if (File.Exists(dosagePath))
            {
                File.WriteAllText(dosagePath, "");
            }
        }

    }
}