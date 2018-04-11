using System;

namespace ConsoleApplication1
{
    class TakenDosage
    {
        //ID for the dosage, should be assigned whenever a dosage is made
        public int dosageID
        {
            get { return dosageID; }
            set { dosageID = value; }
        }
        //ID for prescription
        public int prescriptionID
        {
            get { return prescriptionID; }
            set { prescriptionID = value; }
        }
        //The day/month/year the dosage was taken
        public DateTime dateTaken
        {
            get { return dateTaken; }
            set { dateTaken = value; }
        }
        //The hours:minutes the dosage was taken
        public DateTime timeTaken
        {
            get { return timeTaken; }
            set { timeTaken = value; }
        }

        //Constructor; fill in all data to make dosage
        public TakenDosage(int dosageID, int prescriptionID, DateTime dateTaken, DateTime timeTaken)
        {
            this.dosageID = dosageID;
            this.prescriptionID = prescriptionID;
            this.dateTaken = dateTaken;
            this.timeTaken = timeTaken;
        }
    }

    //Contains all of the information for the actual medication
    class Medication
    {
        //Some sort of  id for the medication?
        public int DIN
        {
            get { return DIN; }
            set { DIN = value; }
        }
        //Name of whoever makes the medication
        public string manufacturer
        {
            get { return manufacturer; }
            set { manufacturer = value; }
        }
        //The brandname like Advil, Tylenol
        public string brandName
        {
            get { return brandName; }
            set { brandName = value; }
        }
        //The basic name like Ibuprofen, Aceteminophen
        public string genericName
        {
            get { return genericName; }
            set { genericName = value; }
        }
        //??
        public float strengthQty
        {
            get { return strengthQty; }
            set { strengthQty = value; }
        }
        //??
        public string strengthUnit
        {
            get { return strengthUnit; }
            set { strengthUnit = value; }
        }
        //??
        public int dispensedQty
        {
            get { return dispensedQty; }
            set { dispensedQty = value; }
        }
        //??
        public string dispensedUnit
        {
            get { return dispensedUnit; }
            set { dispensedUnit = value; }
        }
        //Catch all for notes on the medication
        public string[] additionalNotes
        {
            get { return additionalNotes; }
            set { additionalNotes = value; }
        }
        //The main constructor for Medication
        public Medication(int DIN, string manufacturer, string brandName, string genericName, float strengthQty, string strengthUnit, int dispensedQty, string dispensedUnit, string[] additionalNotes)
        {
            this.DIN = DIN;
            this.manufacturer = manufacturer;
            this.brandName = brandName;
            this.genericName = genericName;
            this.strengthQty = strengthQty;
            this.strengthUnit = strengthUnit;
            this.dispensedQty = dispensedQty;
            this.dispensedUnit = dispensedUnit;
            this.additionalNotes = additionalNotes;
        }
    }

    class Pharmacy
    {
        //The name of the pharmacy
        public string name
        {
            get { return name; }
            set { name = value; }
        }
        //The address of the pharmacy
        public string address
        {
            get { return address; }
            set { address = value; }
        }
        //The phone number of the pharmacy
        public string phoneNumber
        {
            get { return phoneNumber; }
            set { phoneNumber = value; }
        }
        //constructor for pharmacy
        public Pharmacy(string name, string address, string phoneNumber)
        {
            this.name = name;
            this.address = address;
            this.phoneNumber = phoneNumber;
        }
    }

    class Physician
    {
        // the physician's first name
        public string firstName
        {
            get { return firstName; }
            set { firstName = value; }
        }
        //The Physician's Last Name
        public string lastName
        {
            get { return lastName; }
            set { lastName = value; }
        }
        //Constructor for Physician
        public Physician(string firstName,string lastName)
        {
            this.firstName = firstName;
            this.lastName = lastName;
        }
    }

    class Direction
    {
        //The date to start the medecine
        public DateTime startDate
        {
            get { return startDate; }
            set { startDate = value; }
        }
        //The date the medecine ends
        public DateTime endDate
        {
            get { return endDate; }
            set { endDate = value; }
        }
        //??
        public int frequencyQty
        {
            get { return frequencyQty; }
            set { frequencyQty = value; }
        }
        //??
        public string frequencyUnit
        {
            get { return frequencyUnit; }
            set { frequencyUnit = value; }
        }
        //Direction Constructor
        public Direction(DateTime startDate,DateTime endDate,int frequencyQty, string frequencyUnit)
        {
            this.startDate = startDate;
            this.endDate = endDate;
            this.frequencyQty = frequencyQty;
            this.frequencyUnit = frequencyUnit;
        }
    }

    class Prescription
    {
        //The pharmacy the prescription is from
        public Pharmacy pharmacy
        {
            get { return pharmacy; }
            set { pharmacy = value; }
        }
        //The physician who prescribed the medication
        public Physician physician
        {
            get { return physician; }
            set { physician = value; }
        }
        //The medical ID of the Prescription
        public int rxID
        {
            get { return rxID; }
            set { rxID = value; }
        }
        //the refills allowed on the medecine
        public int refills
        {
            get { return refills; }
            set { refills = value; }
        }
        //the date the prescription was filled
        public DateTime dateFilled
        {
            get { return dateFilled; }
            set { dateFilled = value; }
        }
        //the name of the patient
        public string patientName
        {
            get { return patientName; }
            set { patientName = value; }
        }
        //the actual medication
        public Medication medication
        {
            get { return medication; }
            set { medication = value; }
        }
        //the directions for the medication
        public Direction direction
        {
            get { return direction; }
            set { direction = value; }
        }
        //Constructor for Prescription
        public Prescription(Pharmacy pharmacy, Physician physician, int rxID, int refills, DateTime dateFilled, string patientName, Medication medication, Direction, direction)
        {
            this.pharmacy = pharmacy;
            this.physician = physician;
            this.rxID = rxID;
            this.refills = refills;
            this.dateFilled = dateFilled;
            this.patientName = patientName;
            this.medication = medication;
            this.direction = direction;
        }
    }
    class User
    {
        //the users set username
        public string username
        {
            get { return username; }
            set { username = value; }
        }
        //array of prescriptions the user has
        public Prescription[] prescriptions
        {
            get { return prescriptions; }
            set { prescriptions = value; }
        }
        //the log of dosages the user has
        public TakenDosage[] dosageLog
        {
            get { return dosageLog; }
            set { dosageLog = value; }
        }
        //the full constructor for User
        public User(string username, Prescription[] prescriptions, TakenDosage[] dosageLog)
        {
            this.username = username;
            this.prescriptions = prescriptions;
            this.dosageLog = dosageLog;
        }
        //Constructor if you only have the name of the User
        public User(string username)
        {
            this.username = username;
        }
    }

}
