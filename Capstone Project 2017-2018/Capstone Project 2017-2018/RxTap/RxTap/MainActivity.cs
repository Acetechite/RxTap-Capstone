using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;
using System;
using Android.Views;
using Android.Support.V7.Widget;
using System.Collections.Generic;
using Android.Text.Method;
using RxTap.LocalData;

namespace RxTap
{

    [Activity(Label = "RxTap", MainLauncher = true)]
    public class MainActivity : Activity
    {
        public const string ViewPrescMimeType = "application/vnd.xamarin.rxtap";
        User currentUser;
        List<Prescription> prescriptions;
        RecyclerView mRecyclerView;
        PrescriptionAdapter mAdapter;
        RecyclerView.LayoutManager mLayoutManager;
        LinearLayout[] takenAtArray = { };

        //On Application Load
        protected override void OnCreate(Bundle savedInstanceState)
        {
            //Standard
            RequestWindowFeature(WindowFeatures.NoTitle);
            base.OnCreate(savedInstanceState);

            prescriptions = LocalDataManager.Instance.GetAllPrescriptions();
            
            //TODO: DELETE LATER (HARD CODING PREFERRED TIMES FOR DEMO)
            foreach(Prescription p in prescriptions)
            {
                if(p.RXID== 9876543)
                {
                    DateTime now = DateTime.Now;
                    p.Direction.PreferredTimes.Add(new DateTime(now.Year, now.Month, now.Day, 0, 1, 0));
                }
            }

            //Creating dummy data (temporary)
            /*
            prescriptions = new List<Prescription> {
                new Prescription(
                    new Pharmacy(
                        "testPharmacy", "123 Street", "555-555-5555"
                        ),
                    new Physician(
                        "testDrFirstName",
                        "testDrLastName"
                        ),
                    12345,
                    5,
                    new DateTime(2017, 01, 25),
                    "testUser",
                    new Medication(
                        123,
                        "testManufacturer",
                        "testBrand",
                        "testGeneric",
                        500,
                        "mg",
                        20,
                        "capsules"
                        ),
                    2,
                    "daily",
                    new List<DateTime>{
                        new DateTime(9999,12,31,8,0,0),
                        new DateTime(9999,12,31,16,0,0)
                        }
                    )
                };
                */
            List<TakenDosage> testDosages = new List<TakenDosage>
            {
                new TakenDosage(
                    123,
                    12345,
                    DateTime.Today,
                    new DateTime(9999,12,31,8,0,0)
                )
            };
            currentUser = new User(LocalDataManager.Instance.GetUsername(),prescriptions,testDosages);
            
            //load the MainView when the application starts
            SetContentView(Resource.Layout.viewMain);
            loadMainScreen();
        }

        //--------------------------VIEW LOAD PROCEDURES-------------------------------------

        //Actions to take when the main view is loaded
        void loadMainScreen()
        {
            //Set the text with the username
            TextView textUserName;
            textUserName = FindViewById<TextView>(Resource.Id.txtUserName);
            textUserName.Text = "Welcome " + LocalDataManager.Instance.GetUsername();

            //Add button event handler
            Button buttonGoToHistory = FindViewById<Button>(Resource.Id.btnHistory);
            buttonGoToHistory.Click += GoToHistory;

            //Button buttonGoToTestLocalData = FindViewById<Button>(Resource.Id.localDataBtn);
            //buttonGoToTestLocalData.Click += GoToTestLocalData;

            Button buttonGoToSettings = FindViewById<Button>(Resource.Id.userSettingBtn);
            buttonGoToSettings.Click += GoToUserSettings;
        }

        /*
        void loadAddScreen()
        {
            //Medication name input field
            EditText editMedicationName = FindViewById<EditText>(Resource.Id.textAddMedNameEdit);

            //Add Additional Notes button
            Button buttonToggleNotes = FindViewById<Button>(Resource.Id.buttonToggleNote);
            buttonToggleNotes.Click += ToggleNote;

            //Dosage Start Date input field
            EditText editStartDate = FindViewById<EditText>(Resource.Id.textAddDirectionStartEdit);
            editStartDate.Text = DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year;

            //Dosage End Date input field
            EditText editEndDate = FindViewById<EditText>(Resource.Id.textAddDirectionEndEdit);
            editEndDate.Text = DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + (DateTime.Now.Year+1);

            //Dosage Frequency qty field
            EditText editInstructions = FindViewById<EditText>(Resource.Id.textAddDirectionInstructionsEdit);
            editInstructions.TextChanged += instructionsChanged;

            //Return to History button
            Button buttonGoToHistory = FindViewById<Button>(Resource.Id.buttonReturnToHistory);
            buttonGoToHistory.Click += GoToHistory;
        }
        */

        //Actions to take when the medication list view is loaded
        void loadMedicationList()
        {
            //Setup recycler view with the user's medications
            mAdapter = new PrescriptionAdapter(currentUser);
            mRecyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerMedList);
            mLayoutManager = new LinearLayoutManager(this);
            mRecyclerView.SetLayoutManager(mLayoutManager);
            //Individual items event handler
            mRecyclerView.SetAdapter(mAdapter);
            mAdapter.ItemClick += OnItemClick;
            //Return to main button event handler
            Button buttonGoToMain = FindViewById<Button>(Resource.Id.buttonReturnToMain);
            buttonGoToMain.Click += GoToMain;
            /*
            //Add manually event handler
            Button buttonAddPrescription = FindViewById<Button>(Resource.Id.addPrescriptionButton);
            buttonAddPrescription.Click += GoToAdd;
            */
        }

        //Actions to take when the medication details view is loaded
        void loadMedicationDetails(Prescription prescription, User user)
        {
            //Setting Medication Information
            SetContentView(Resource.Layout.viewMedDtls);
            FindViewById<TextView>(Resource.Id.textMedName).Text =prescription.Medication.GenericName;
            FindViewById<TextView>(Resource.Id.textDIN).Text += prescription.Medication.DIN.ToString();
            FindViewById<TextView>(Resource.Id.textManufacturer).Text += prescription.Medication.Manufacturer;
            FindViewById<TextView>(Resource.Id.textBrand).Text += prescription.Medication.BrandName;
            FindViewById<TextView>(Resource.Id.textStrength).Text += prescription.Medication.StrengthQty.ToString() + " " + prescription.Medication.StrengthUnit;
            FindViewById<TextView>(Resource.Id.textDispensed).Text += prescription.Medication.DispensedQty.ToString() + " " + prescription.Medication.DispensedUnit;
            String additionalNotes = "";
            //If the additional notes is not empty, add them all together into a single string
            if (prescription.Medication.AdditionalNotes != null && prescription.Medication.AdditionalNotes.Count > 0)
            {
                foreach (String note in prescription.Medication.AdditionalNotes)
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
            FindViewById<TextView>(Resource.Id.textDirectionWindow).Text = "To be taken for: " + prescription.Direction.StartDate.ToLongDateString() + " to " + prescription.Direction.EndDate.ToLongDateString();
            FindViewById<TextView>(Resource.Id.textDirectionInstructions).Text = "Take " + prescription.Direction.FrequencyQty + " time(s) per " + prescription.Direction.FrequencyUnit;
            
            //If the preferred times isn't null, add them all to a single string
            String preferredTimes = "";
            int numTimes = prescription.Direction.PreferredTimes.Count;
            if (numTimes > 0)
            {
                for(int i = 0; i < numTimes; i++)
                {
                    preferredTimes += prescription.Direction.PreferredTimes[i].ToShortTimeString();

                    if(i != numTimes - 1)
                        preferredTimes += ", ";
                }
            }
            if (preferredTimes == "")
            {
                FindViewById<TextView>(Resource.Id.textDirectionTimePreference).Text = "No time preferences specified";
            }
            else
            {
                FindViewById<TextView>(Resource.Id.textDirectionTimePreference).Text = "To be taken at: " + preferredTimes;
            }
            

            //Setting User Doses
            LinearLayout userDoses = FindViewById<LinearLayout>(Resource.Id.linearUserDoses);
            foreach (TakenDosage dose in user.DosageLog){
                //Only display doses for the current prescription on the current date
                if (dose.RXID == prescription.RXID)
                {
                    if (dose.DateTaken.Date == DateTime.Now.Date)
                    {
                        TextView text = new TextView(this)
                        {
                            Text = "Dose taken at " + dose.TimeTaken.ToShortTimeString()
                        };
                        text.SetTextColor(Android.Graphics.Color.Black);
                        //Add each dose to the vertical layout view
                        userDoses.AddView(text);
                    }
                }
            }

            //Setting Prescription Information
            FindViewById<TextView>(Resource.Id.textRX).Text += prescription.RXID;
            FindViewById<TextView>(Resource.Id.textRefills).Text += prescription.Refills.ToString();
            FindViewById<TextView>(Resource.Id.textDateFilled).Text += prescription.DateFilled.ToShortDateString();
            FindViewById<TextView>(Resource.Id.textPhysician).Text += prescription.Physician.FirstName + " " + prescription.Physician.LastName;
            //Setting Pharmacy Information
            FindViewById<TextView>(Resource.Id.textPharmacyName).Text += prescription.Pharmacy.Name;
            FindViewById<TextView>(Resource.Id.textPharmacyAddress).Text += prescription.Pharmacy.Address;
            FindViewById<TextView>(Resource.Id.textPharmacyPhone).Text += prescription.Pharmacy.PhoneNumber;

            Button buttonGoToHistory = FindViewById<Button>(Resource.Id.buttonReturnToHistory);
            buttonGoToHistory.Click += GoToHistory;

            Button addDoseBtn = FindViewById<Button>(Resource.Id.addDoseBtn);
            addDoseBtn.Click += AddDosage;

            Button buttonDeletePresc = FindViewById<Button>(Resource.Id.deletePrescBtn);
            buttonDeletePresc.Click += DeletePrescription;
        }

        void loadTestScreen()
        {
            //Add click handler for Add Test Data button
            Button buttonAddData = FindViewById<Button>(Resource.Id.addDataBtn);
            buttonAddData.Click += AddTestLocalData;

            //Add click handler for Load Test Data button
            Button buttonLoadData = FindViewById<Button>(Resource.Id.loadDataBtn);
            buttonLoadData.Click += LoadTestLocalData;

            //Add click handler for Clear Test Data button
            Button buttonClearData = FindViewById<Button>(Resource.Id.clearDataBtn);
            buttonClearData.Click += ClearTestLocalData;

            //Add click handler for Clear Test Data button
            Button clearDosagebtn = FindViewById<Button>(Resource.Id.clearDosagebtn);
            clearDosagebtn.Click += ClearDosageData;

            TextView testDataDisplay = FindViewById<TextView>(Resource.Id.testDataDisplay);
            testDataDisplay.MovementMethod = new ScrollingMovementMethod(); //set text view to be scrollable

        }

        void loadSettingsScreen()
        {
            //display username
            TextView userText = FindViewById<TextView>(Resource.Id.userText);
            userText.Text = LocalDataManager.Instance.GetUsername();

            //Add click handler for Return to menu
            Button buttonReturnToMain = FindViewById<Button>(Resource.Id.buttonReturnToMain);
            buttonReturnToMain.Click += GoToMain;

            //Edit Button handler
            Button buttonEditUser = FindViewById<Button>(Resource.Id.saveBtn);
            buttonEditUser.Click += SaveUser;

            //Test Local Data Button handler
            Button buttonGoToTestLocalData = FindViewById<Button>(Resource.Id.localDataBtn);
            buttonGoToTestLocalData.Click += GoToTestLocalData;
        }

        //--------------------------CLICK EVENTS-------------------------------------

        //History Button
        void GoToHistory(object sender, EventArgs args)
        {
            SetContentView(Resource.Layout.viewMedList);
            loadMedicationList();
        }

        //Take Dose Now Button
        void AddDosage(object sender, EventArgs args)
        {
            TextView textRX = FindViewById<TextView>(Resource.Id.textRX);
            string pID = textRX.Text.Substring(4);

            //check if dosage is available for today

            if (!LocalDataManager.Instance.IsThereAvailableDoses(Int32.Parse(pID)))
            {
                //display alert
                AlertDialog.Builder dialog = new AlertDialog.Builder(this);
                AlertDialog alert = dialog.Create();
                alert.SetTitle("Dose Not Added");
                alert.SetMessage("You should not take any more doses today.");
                alert.SetButton("OK", (c, ev) =>
                {
                    // Ok button click task  
                });
                alert.Show();
            }
            else
            {
                //string format is: dosageID, RXID, dateTaken, hour, minute, second

                //create temporary dosageID (don't know if we actually need this)
                string dID = "111";

                DateTime today = DateTime.Now;
                string date = today.Date.ToString();

                string h = today.Hour.ToString();
                string m = today.Minute.ToString();
                string s = today.Second.ToString();

                string dosageString = dID + "," + pID + "," + date + "," + h + "," + m + "," + s;
                LocalDataManager.Instance.AddNewTakenDosage(dosageString);

                //display alert
                AlertDialog.Builder dialog = new AlertDialog.Builder(this);
                AlertDialog alert = dialog.Create();
                alert.SetTitle("Dose Added!");
                alert.SetMessage("Your dosage has been recorded.");
                alert.SetButton("OK", (c, ev) =>
                {
                    // Ok button click task  
                });
                alert.Show();
            }


        }

        //Delete Prescription Button
        void DeletePrescription(object sender, EventArgs args)
        {
            TextView textRX = FindViewById<TextView>(Resource.Id.textRX);
            int rxid = Int32.Parse(textRX.Text.Substring(4));

            //delete presc by id
            LocalDataManager.Instance.DeletePrescription(rxid);

            //send back to main
            var intent = new Intent(this, typeof(MainActivity));
            StartActivity(intent);
        }

        //Main Button
        void GoToMain(object sender, EventArgs args)
        {
            SetContentView(Resource.Layout.viewMain);
            loadMainScreen();
        }
        
        //Settings Button
        void GoToUserSettings(object sender, EventArgs args)
        {
            SetContentView(Resource.Layout.viewSettings);
            loadSettingsScreen();
        }

        /*
        //Add Prescription Manually Button
        void GoToAdd(object sender, EventArgs args)
        {
            SetContentView(Resource.Layout.viewAddMed);
            loadAddScreen();
        }
        */
        
        void SaveUser(object sender, EventArgs args)
        {
            //save this to username
            EditText editText = FindViewById<EditText>(Resource.Id.editText1);
            LocalDataManager.Instance.SaveUser(editText.Text);

            //display username
            TextView userText = FindViewById<TextView>(Resource.Id.userText);
            userText.Text = LocalDataManager.Instance.GetUsername();
        }

        //TODO: DELETE THIS LATER
        void GoToTestLocalData(object sender, EventArgs args)
        {
            SetContentView(Resource.Layout.testAddLocalData);   //set the view to testAddLocalData
            loadTestScreen();
        }

        //TODO: DELETE THIS LATER
        //Add Test Data button
        void AddTestLocalData(object sender, EventArgs args)
        {
            //Raw data input field
            TextView rawData = FindViewById<TextView>(Resource.Id.rawDataInput);
            String data = rawData.Text;

            if(data != null && data != "")
            {
                LocalDataManager.Instance.AddNewPrescription(data);

                TextView testDataDisplay = FindViewById<TextView>(Resource.Id.testDataDisplay);
                testDataDisplay.Text = "Prescription Added!";
            }
            
        }

        //TODO: DELETE THIS LATER
        //Load Test Data button
        void LoadTestLocalData(object sender, EventArgs args)
        {
            TextView testDataDisplay = FindViewById<TextView>(Resource.Id.testDataDisplay);
            testDataDisplay.Text = "";
            
            string[] prescriptionData = LocalDataManager.Instance.GetAllPrescriptionData();
            if(prescriptionData==null)
            {
                testDataDisplay.Text = "NO DATA";
            }
            else
            {
                foreach (string prescription in prescriptionData)
                {
                    testDataDisplay.Text += prescription + "\n";
                }
            }
        }

        //TODO: DELETE THIS LATER
        //Clear Test Data button
        void ClearTestLocalData(object sender, EventArgs args)
        {
            LocalDataManager.Instance.ClearAllPrescriptions();

            TextView testDataDisplay = FindViewById<TextView>(Resource.Id.testDataDisplay);
            testDataDisplay.Text = "Test data cleared!";
        }

        void ClearDosageData(object sender, EventArgs args)
        {
            LocalDataManager.Instance.ClearAllTakenDosages();

            TextView testDataDisplay = FindViewById<TextView>(Resource.Id.testDataDisplay);
            testDataDisplay.Text = "Dosage data cleared!";
        }

        void instructionsChanged(object sender, EventArgs args)
        {
            Android.Text.TextChangedEventArgs arguments = (Android.Text.TextChangedEventArgs)args;
            String text = arguments.Text.ToString();
            if (text != "")
            {
                if (int.Parse(text) > 0 && int.Parse(text) < 10)
                {
                    LinearLayout MedicationInfo = FindViewById<LinearLayout>(Resource.Id.linearMedicationInfo);
                    MedicationInfo.RemoveViews(MedicationInfo.ChildCount- takenAtArray.Length, takenAtArray.Length);
                    takenAtArray = new LinearLayout[int.Parse(text)];
                    for (int i=0;i< int.Parse(text);i++)
                    {
                        LinearLayout takenAt = new LinearLayout(this);
                        takenAt.Orientation = Orientation.Horizontal;
                        TextView takenAtLabel = new TextView(this);
                        takenAtLabel.Text = "Taken at: ";
                        EditText edit = new EditText(this);
                        edit.Text = (24 - (24 / (i+1))).ToString() +":"+"00";
                        edit.InputType = Android.Text.InputTypes.ClassDatetime;
                        takenAt.AddView(takenAtLabel);
                        takenAt.AddView(edit);
                        takenAtArray[i] = takenAt;
                    }
                    foreach (LinearLayout lin in takenAtArray)
                    {
                        MedicationInfo.AddView(lin);
                    }
                }
            }
        }

        void ToggleNote(object sender, EventArgs args)
        {
            Button buttonToggleNotes = FindViewById<Button>(Resource.Id.buttonToggleNote);
            LinearLayout linyearMedInfo = FindViewById<LinearLayout>(Resource.Id.linearAddMedicationInfo);
            if (buttonToggleNotes.Text == "+")
            {
                buttonToggleNotes.Text = "-";
                EditText editNote = new EditText(this);
                editNote.Id = 55555;
                editNote.SetMinWidth(500);
                linyearMedInfo.AddView(editNote);
            }
            else
            {
                buttonToggleNotes.Text = "+";
                linyearMedInfo.RemoveView(FindViewById(55555));
            }
        }

        //Prescription History Entry
        void OnItemClick(object sender, int position)
        {
            loadMedicationDetails(prescriptions[position], currentUser);
        }
    }


    public class PrescriptionViewHolder : RecyclerView.ViewHolder
    {
        public TextView MedicationName { get; private set; }
        public TextView Doses { get; private set; }
        public LinearLayout Background { get; private set; }

    public PrescriptionViewHolder(View itemView, Action<int> listener) : base(itemView)
        {
            MedicationName = itemView.FindViewById<TextView>(Resource.Id.txtMedTitle);
            Doses = itemView.FindViewById<TextView>(Resource.Id.txtDosage);
            Background = itemView.FindViewById<LinearLayout>(Resource.Id.linLayoutCard);
            itemView.Click += (sender, e) => listener(base.LayoutPosition);
        }
    }

    public class PrescriptionAdapter : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;

        public User mUser;

        public PrescriptionAdapter(User currentUser)
        {
            mUser = currentUser;
        }

        public override RecyclerView.ViewHolder
            OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).
                        Inflate(Resource.Layout.viewMedCard, parent, false);

            PrescriptionViewHolder vh = new PrescriptionViewHolder(itemView, OnClick);
            return vh;
        }

        public override void
            OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            PrescriptionViewHolder vh = holder as PrescriptionViewHolder;
            
            vh.MedicationName.Text = mUser.Prescriptions[position].Medication.StrengthQty 
                + " " + mUser.Prescriptions[position].Medication.StrengthUnit 
                + " " + mUser.Prescriptions[position].Medication.GenericName;

            //see how many doses of this prescription should have been taken so far today
            int dosesToday = 0;

            
            //Add up all of the doses the user should have taken today
            for (int i = 0; i < mUser.Prescriptions[position].Direction.PreferredTimes.Count; i++)
            {
                if (mUser.Prescriptions[position].Direction.PreferredTimes[i].TimeOfDay < DateTime.Now.TimeOfDay)
                    dosesToday++;
            }
            int missedDoses = CheckMissedDoses(mUser.Prescriptions[position].RXID, mUser, dosesToday);
            if (missedDoses > 0)
            {
                vh.Doses.Text = "Missed " + missedDoses + " dose(s).";
                vh.Doses.SetTextColor(Android.Graphics.Color.Red);
                vh.MedicationName.SetTextColor(Android.Graphics.Color.Red);
                vh.Background.SetBackgroundColor(new Android.Graphics.Color(255, 163, 163));
            }
            else
            {
                /*
                if(dosesToday >= mUser.Prescriptions[position].Direction.FrequencyQty)
                {
                    vh.Doses.SetTextColor(Android.Graphics.Color.Gray);
                    vh.MedicationName.SetTextColor(Android.Graphics.Color.Gray);
                    vh.Background.SetBackgroundColor(Android.Graphics.Color.LightGray);
                }
                */

                dosesToday = LocalDataManager.Instance.GetTodaysDosesForPrescription(mUser.Prescriptions[position].RXID).Count;
                vh.Doses.Text = "Completed "+ dosesToday + "/"+ mUser.Prescriptions[position].Direction.FrequencyQty + " doses.";
            }
        }

        //method used to find out if the user has missed any doses, dosesToday is the number of doses of the current medication the user SHOULD have taken
        private int CheckMissedDoses(int rxID, User user, int dosesToday)
        {
            List<TakenDosage> takenDoses = LocalDataManager.Instance.GetTodaysDosesForPrescription(rxID);

            //Remove all of the recorded doses for the user for today
            for (int i = 0; i < takenDoses.Count; i++)
            {
                //Make sure it is for today
                if (takenDoses[i].DateTaken.Date==DateTime.Now.Date) { 
                    //Make sure it is the same prescription
                    if (takenDoses[i].RXID == rxID)
                    {
                        dosesToday--;
                    }
                }
            }
            return dosesToday;
        }

        private void UpdateDoses() { 
}

        //Required procedure for RecyclerView.Adapter implementation
        public override int ItemCount
        {
            get { return mUser.Prescriptions.Count; }
        }

        // Raise an event when the item-click takes place:
        void OnClick(int position)
        {
            if (ItemClick != null)
                ItemClick(this, position);
        }
    }

}

