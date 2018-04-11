using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Support.V7.Widget;
using System.Collections.Generic;

namespace RecyclerViewer
{
    public class MedHistoryItem
    {
        public String strMedName;
        public Dosage objDosage;
        public int intTotalDoses;
        public int intCompletedDoses;
        public int intMissedDoses;

        public String MedName
        {
            get { return strMedName; }
        }

        public String Dosage
        {
            get { return objDosage.amount.ToString() + objDosage.unit; }
        }

        public int totalDoses
        {
            get { return intTotalDoses; }
        }
        public int completedDoses
        {
            get { return intCompletedDoses; }
        }
        public int missedDoses
        {
            get { return intMissedDoses; }
        }

    }

    public class Dosage
    {
        public double amount;
        public String unit;
        public Dosage(double amount, String unit){
            this.amount = amount;
            this.unit = unit;
        }
    }

    public class MedHistoryCollection
    {
        static MedHistoryItem[] testMeds = {
            new MedHistoryItem {strMedName="Tylenol",objDosage=new Dosage(500,"mg"),intTotalDoses=2,intCompletedDoses=2,intMissedDoses=0},
            new MedHistoryItem {strMedName="Advil",objDosage=new Dosage(1,"g"),intTotalDoses=5,intCompletedDoses=3,intMissedDoses=0},
            new MedHistoryItem {strMedName="Nyquil",objDosage=new Dosage(250,"mg"),intTotalDoses=5,intCompletedDoses=2,intMissedDoses=1}
            };

        private MedHistoryItem[] MedHistoryItems;

        // Create an instance copy of the built-in photo list and
        // create the random number generator:
        public MedHistoryCollection()
        {
            MedHistoryItems = testMeds;
        }

        // Return the number of photos in the photo album:
        public int Size
        {
            get { return MedHistoryItems.Length; }
        }

        // Indexer (read only) for accessing a photo:
        public MedHistoryItem this[int i]
        {
            get { return MedHistoryItems[i]; }
        }
    }
}