using System;
using System.Text;
using System.Collections.Generic;

using Android.App;
using Android.Nfc;
using Android.OS;
using Android.Widget;
using Android.Content;
using Parser;
using RxTap.LocalData;

namespace RxTap
{
    //This lets us define this as an activity that is triggered when an nfc is discovered and it has our tag on it
    [Activity, IntentFilter(new[] { "android.nfc.action.NDEF_DISCOVERED" },
        DataMimeType = MainActivity.ViewPrescMimeType,
        Categories = new[] { "android.intent.category.DEFAULT" })]
    class DisplayTap : Activity
    {
        string rawPrescriptionData;
        ParserData parse;

        //onCreate for our new view
        protected override void OnCreate(Bundle savedInstanceState)
        {
            //just opens up the window for adding prescription
            base.OnCreate(savedInstanceState);

            

            //grabs the text field for testing
            

            //get the intent of the chip, or no intent if it didnt work
            var intentType = Intent.Type ?? String.Empty;

            if (MainActivity.ViewPrescMimeType.Equals(intentType))
            {
                // Get the string that was written to the NFC tag, and display it.
                var rawMessages = Intent.GetParcelableArrayExtra(NfcAdapter.ExtraNdefMessages);
                var msg = (NdefMessage)rawMessages[0];
                var prescRecord = msg.GetRecords()[0];
                var prescData = Encoding.ASCII.GetString(prescRecord.GetPayload());

                rawPrescriptionData = prescData;
                parse = new ParserData(prescData);

                /*
                 * TODO: CHECK IF PATIENT NAME IN NFC DATA MATCHES THE USERNAME (WE SHOULD PROBS ALSO SPLIT THIS INTO FIRST NAME, LAST NAME)
                 * -IF NAME MATCHES
                 *      -IF PRESCRIPTION DOESN'T EXIST IN APP
                 *          -PROMPT USER IF THEY'D LIKE TO ADD PRESCRIPTION
                 *      -ELSE IF PRESCRIPTION DOES EXIST IN APP
                 *          -ADD DOSAGE
                 * -ELSE IF NAME DOESN'T MATCH
                 *      -NOTIFY USER THIS IS NOT THEIR PRESCRIPTION
                 * */

                string greetingText = "";

                if (parse.patientName.Equals(LocalDataManager.Instance.GetUsername()))
                {
                    if (LocalDataManager.Instance.DoesPrescriptionExist(parse.prescriptionID))
                    {
                        //check if dosage is available for today, if yes, show accept screen
                        if (LocalDataManager.Instance.IsThereAvailableDoses(parse))
                        {
                            SetTapView();
                            //prompt user if they are taking a dose now
                            greetingText = "Hello " + LocalDataManager.Instance.GetUsername() + ", you're able to take a dose. Are you taking it now?";
                        }
                        else
                        {
                            SetTapViewBad();
                            //If there are no more available doses for today
                            greetingText = "You should not take any more doses today.";
                        }

                        //decrement dosage counter


                        //create a TakenDosage

                        //if not
                        TextView textGreeting;
                        TextView textDose;
                        textGreeting = FindViewById<TextView>(Resource.Id.textView1);
                        textDose = FindViewById<TextView>(Resource.Id.textView2);
                        textGreeting.Text = greetingText;
                        textDose.Text = parse.strengthQty + parse.strengthUnit + " " + parse.brandName + " (" + parse.genericName + ")";
                    }
                    else
                    {
                        DisplayNewPrescription();
                        /*
                        //If patient name in chip data doesn't match user's name
                        greetingText = "This is not your prescription.";
                        img.SetImageResource(Resource.Drawable.imgReject);
                        layout.SetBackgroundResource(Resource.Color.clrRejectedRed);
                        */
                    }
                }
                else
                {
                    SetTapViewBad();
                    //If there are no more available doses for today
                    greetingText = "This is not your prescription.";

                    //if not
                    TextView textGreeting;
                    TextView textDose;
                    textGreeting = FindViewById<TextView>(Resource.Id.textView1);
                    textDose = FindViewById<TextView>(Resource.Id.textView2);
                    textGreeting.Text = greetingText;
                    textDose.Text = parse.strengthQty + parse.strengthUnit + " " + parse.brandName + " (" + parse.genericName + ")";
                }
                
                

            }
        }

        void SetTapView()
        {
            SetContentView(Resource.Layout.viewTap);

            //click handler
            Button tapOkBtn = FindViewById<Button>(Resource.Id.tapOkBtn);
            tapOkBtn.Click += ConfirmTap;
            Button tapNoBtn = FindViewById<Button>(Resource.Id.tapNoBtn);
            tapNoBtn.Click += CancelTap;
            
        }

        void SetTapViewBad()
        {
            SetContentView(Resource.Layout.viewTapBad);
            Button tapOkBtnBad = FindViewById<Button>(Resource.Id.tapOkBtnBad);
            tapOkBtnBad.Click += CancelTap;
        }
        void SetTapViewNew()
        {
            SetContentView(Resource.Layout.tapViewNew);
            Button tapOkBtnBad = FindViewById<Button>(Resource.Id.tapOkBtn);
            tapOkBtnBad.Click += CancelTap;
        }

        void StartMainActivity()
        {
            var intent = new Intent(this, typeof(MainActivity));
            StartActivity(intent);
        }

        void DisplayNewPrescription()
        {

            //set view
            SetContentView(Resource.Layout.viewAddNewMedDtls);

            //Add button event handlers
            Button addNewMedBtn = FindViewById<Button>(Resource.Id.addNewMedBtn);
            addNewMedBtn.Click += AddNewPrescription;

            Button cancelNewMedBtn = FindViewById<Button>(Resource.Id.cancelNewMedBtn);
            cancelNewMedBtn.Click += CancelTap;

            //Setting Medication Information
            FindViewById<TextView>(Resource.Id.textBrand).Text = parse.brandName;
            FindViewById<TextView>(Resource.Id.textDIN).Text += parse.DIN.ToString();
            FindViewById<TextView>(Resource.Id.textManufacturer).Text += parse.manufacturer;
            FindViewById<TextView>(Resource.Id.textMedName).Text += parse.genericName;
            FindViewById<TextView>(Resource.Id.textStrength).Text += parse.strengthQty.ToString() + " " + parse.strengthUnit;
            FindViewById<TextView>(Resource.Id.textDispensed).Text += parse.dispensedQty.ToString() + " " + parse.dispensedUnit;
            String additionalNotes = "";

            //If the additional notes is not empty, add them all together into a single string
            if (parse.additionalNotes != null && parse.additionalNotes.Count > 0)
            {
                foreach (String note in parse.additionalNotes)
                {
                    additionalNotes += note + "\r\n";
                }
            }
            if (additionalNotes == "")
            {
                additionalNotes = "N/A";
            }
            FindViewById<TextView>(Resource.Id.textNotes).Text += additionalNotes;

            //Setting Dosage Information
            FindViewById<TextView>(Resource.Id.textDirectionWindow).Text += parse.startDate.ToShortDateString();
            FindViewById<TextView>(Resource.Id.textDirectionInstructions).Text += "Take " + parse.frequencyQty.ToString() + " time(s) per " + parse.frequencyUnit;

            //Setting Prescription Information
            FindViewById<TextView>(Resource.Id.textRX).Text += parse.prescriptionID;
            FindViewById<TextView>(Resource.Id.textRefills).Text += parse.refills.ToString();
            FindViewById<TextView>(Resource.Id.textDateFilled).Text += parse.dateFilled.ToShortDateString();
            FindViewById<TextView>(Resource.Id.textPhysician).Text += parse.physicianFirstName + " " + parse.physicianLastName;

            //Setting Pharmacy Information
            FindViewById<TextView>(Resource.Id.textPharmacyName).Text += parse.pharmacyName;
            FindViewById<TextView>(Resource.Id.textPharmacyAddress).Text += parse.address;
            FindViewById<TextView>(Resource.Id.textPharmacyPhone).Text += parse.phoneNumber;
            
        }

        void AddNewPrescription(object sender, EventArgs args)
        {
            LocalDataManager.Instance.AddNewPrescription(rawPrescriptionData);
            SetTapViewNew();
            LoadMedicationAcceptance();
        }

        void CancelNewPrescription(object sender, EventArgs args)
        {
            Console.WriteLine("CANCEL NEW PRESCRIPTION");
        }

        void LoadMedicationAcceptance()
        {
            TextView textGreeting = FindViewById<TextView>(Resource.Id.textView1);
            textGreeting.Text = "New Prescription Added!";

            TextView textDose = FindViewById<TextView>(Resource.Id.textView2);
            textDose.Text = parse.brandName;
        }

        //CLICK EVENT HANDLERS =============================================
        void ConfirmTap(object sender, EventArgs args)
        {
            //string format is: dosageID, RXID, dateTaken, hour, minute, second

            //create temporary dosageID (don't know if we actually need this)
            string dID = "098";
            string pID = parse.prescriptionID.ToString();

            DateTime today = DateTime.Now;
            string date = today.Date.ToString();

            string h = today.Hour.ToString();
            string m = today.Minute.ToString();
            string s = today.Second.ToString();

            string dosageString = dID + "," + pID + "," + date + "," + h + "," + m + "," + s;
            LocalDataManager.Instance.AddNewTakenDosage(dosageString);

            StartMainActivity();
        }

        void CancelTap(object sender, EventArgs args)
        {
            StartMainActivity();
        }
    }
}