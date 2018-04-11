using Android.App;
using Android.Widget;
using Android.OS;
using System;
using System.Collections.Generic;
using RxTap;

namespace Parser
{
    
    public class ParserData
    {
        public int prescriptionID;
        public int DIN;
        public string manufacturer;
        public string brandName;
        public string genericName;
        public float strengthQty;
        public string strengthUnit;
        public int dispensedQty;
        public string dispensedUnit;
        public List<String> additionalNotes;
        public string pharmacyName;
        public string address;
        public string phoneNumber;
        public string physicianFirstName;
        public string physicianLastName;
        public DateTime startDate;
        public DateTime endDate;
        public int frequencyQty;
        public string frequencyUnit;
        public int refills;
        public DateTime dateFilled;
        public string patientName;

        public ParserData(string data)
        {
            additionalNotes = new List<string>();

            //raw data separated by ',' character
            char delimiter = ',';
            string[] parsedData = data.Split(delimiter);
            foreach (var substring in parsedData)
                System.Console.WriteLine(substring);

            //Prescription
            prescriptionID = Int32.Parse(parsedData[0]);

            //Medication
            DIN = Int32.Parse(parsedData[1]);
            manufacturer = parsedData[2];
            brandName = parsedData[3];
            genericName = parsedData[4];
            strengthQty = Int32.Parse(parsedData[5]);
            strengthUnit = parsedData[6];
            dispensedQty = Int32.Parse(parsedData[7]);

            //Dispensed unit
            string t = parsedData[8];
            if (t == "c")
                dispensedUnit = "capsule(s)";
            else if (t == "p")
                dispensedUnit = "pill(s)";
            else if (t == "t")
                dispensedUnit = "tablet(s)";
            else
                dispensedUnit = "NONE";

            //Additional Notes (separated by '$' character)
            string[] notesArray = parsedData[9].Split('$');
            foreach(string s in notesArray)
            {
                additionalNotes.Add(s);
            }

            //Pharmacy
            pharmacyName = parsedData[10];
            address = parsedData[11];
            phoneNumber = parsedData[12];

            //Physician
            physicianFirstName = parsedData[13];
            physicianLastName = parsedData[14];

            //Start Date
            string k = parsedData[15];
            string date = "" + k[0] + k[1] + "/" + k[2] + k[3] + "/20" + k[4] + k[5];
            startDate = Convert.ToDateTime(date);

            //End Date
            //string l = parsedData[16];
            //string date2 = "" + l[0] + l[1] + "/" + l[2] + l[3] + "/20" + l[4] + l[5];
            //endDate = Convert.ToDateTime(date2);

            //Frequency amount
            frequencyQty = Int32.Parse(parsedData[16]);
            frequencyUnit = parsedData[17];

            //Refills
            refills = Int32.Parse(parsedData[18]);

            //Date Filled
            string p = parsedData[19];
            string date3 = "" + p[0] + p[1] + "/" + p[2] + p[3] + "/20" + p[4] + p[5];
            dateFilled = Convert.ToDateTime(date3);

            //Patient name
            patientName = parsedData[20];

        }

        public Prescription CreatePrescription()
        {
            Pharmacy pharmacy = new Pharmacy(pharmacyName, address, phoneNumber);
            Physician physician = new Physician(physicianFirstName, physicianLastName);
            Medication medication = new Medication(DIN, manufacturer, brandName, genericName, strengthQty, strengthUnit, dispensedQty, dispensedUnit, additionalNotes);

            return new Prescription(pharmacy, physician, prescriptionID, refills, dateFilled, patientName, medication, frequencyQty, frequencyUnit);
        }


    }
}

