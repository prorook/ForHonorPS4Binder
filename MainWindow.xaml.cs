using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace ForHonorPS4Binder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Dictionary<string, string> buttonConfigDict;
        private Dictionary<string, Button> buttonControlsDict;
        private Dictionary<string, string> buttonActionDict;
        private Button popupButtonSelected;
        private string filename;
        // Read the old file.
        private string[] oldFileLines;

        /*
        [PLAYSTATION(R)4 CONTROLLER]
        BTN_A=DI_BTN_2
        BTN_B=DI_BTN_3
        BTN_BACK=DI_BTN_14
        BTN_LBUMP=DI_BTN_5
        BTN_LSTICK=DI_BTN_11
        BTN_LTRIGGER=DI_AXIS_RX,OFFSET_UP
        BTN_RBUMP=DI_BTN_6
        BTN_RSTICK=DI_BTN_12
        BTN_RTRIGGER=DI_AXIS_RY,INVERT,OFFSET_UP
        BTN_START=DI_BTN_10
        BTN_X=DI_BTN_1
        BTN_Y=DI_BTN_4
        DEVTYPE=PS4GAMEPAD
        DPAD_DOWN=DI_POV_0_DOWN
        DPAD_LEFT=DI_POV_0_LEFT
        DPAD_RIGHT=DI_POV_0_RIGHT
        DPAD_UP=DI_POV_0_UP
        NAMEPATTERN=PLAYSTATION
        THUMB_LX=DI_AXIS_TX
        THUMB_LY=DI_AXIS_TY
        THUMB_RX=DI_AXIS_TZ
        THUMB_RY=DI_AXIS_RZ,INVERT

            square = x
            x = A
            circle = b
            triangle = Y
         */

        public string actionGrab = "DI_BTN_1";
        public string actionDodge = "DI_BTN_2";
        public string actionCancel = "DI_BTN_3";
        public string actionRevenge = "DI_BTN_4";
        public string actionChat = "DI_BTN_5";
        public string actionTarget = "DI_AXIS_RX,OFFSET_UP";
        public string actionLightAtk = "DI_BTN_6";
        public string actionHeavyAtk = "DI_AXIS_RY,INVERT,OFFSET_UP";
        public string actionCameraReset = "DI_BTN_12";
        public string actionSprint = "DI_BTN_11";

        public string buttonX = "BTN_A";
        public string buttonCircle = "BTN_B";
        public string buttonTriangle = "BTN_Y";
        public string buttonSquare = "BTN_X";
        public string buttonLBump = "BTN_LBUMP";
        public string buttonRBump = "BTN_RBUMP";
        public string buttonRTrigger = "BTN_RTRIGGER";
        public string buttonLTrigger = "BTN_LTRIGGER";
        public string buttonL3 = "BTN_LSTICK";
        public string buttonR3 = "BTN_RSTICK";

        public MainWindow()
        {
            InitializeComponent();
            buttonConfigDict = new Dictionary<string, string>();
            buttonControlsDict = new Dictionary<string, Button>();
            buttonControlsDict.Add(buttonX, btn_X);
            buttonControlsDict.Add(buttonCircle, btn_circle);
            buttonControlsDict.Add(buttonTriangle, btn_triangle);
            buttonControlsDict.Add(buttonSquare, btn_square);
            buttonControlsDict.Add(buttonLBump, btn_L1);
            buttonControlsDict.Add(buttonRBump, btn_R1);
            buttonControlsDict.Add(buttonRTrigger, btn_R2);
            buttonControlsDict.Add(buttonLTrigger, btn_L2);
            buttonControlsDict.Add(buttonL3, btn_L3);
            buttonControlsDict.Add(buttonR3, btn_R3);

            buttonActionDict = new Dictionary<string, string>();
            buttonActionDict.Add("DI_BTN_1", "Grab");
            buttonActionDict.Add("DI_BTN_2", "Dodge");
            buttonActionDict.Add("DI_BTN_3", "Cancel");
            buttonActionDict.Add("DI_BTN_4", "Revenge");
            buttonActionDict.Add("DI_BTN_5", "Chat");
            buttonActionDict.Add("DI_AXIS_RX,OFFSET_UP", "Target");
            buttonActionDict.Add("DI_BTN_6", "LightAtk");
            buttonActionDict.Add("DI_AXIS_RY,INVERT,OFFSET_UP", "HeavyAtk");
            buttonActionDict.Add("DI_BTN_12","CameraReset");
            buttonActionDict.Add("DI_BTN_11", "Sprint");

            foreach (KeyValuePair<string, string> action in buttonActionDict)
            {
                ActionListView.Items.Add(action.Value);
            }
        }   

        private void btn_LoadConfig_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension 
            dlg.DefaultExt = "legacygamepads.ini";
            dlg.Filter = "legacygamepads.ini (*.ini)|legacygamepads.ini";

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                filename = dlg.FileName;
                this.LoadConfig(filename);
            }
        }

        public void LoadConfig(string filename)
        {
            bool ps4found = false;
            oldFileLines = File.ReadAllLines(filename);

            // Open the text file using a stream reader.
            using (StreamReader sr = new StreamReader(filename))
            {
                string line;

                while ((line = sr.ReadLine()) != null)
                {
                    if (ps4found == true && string.IsNullOrEmpty(line))
                    {
                        break;
                    }

                    if (ps4found == true)
                    {
                        string buttonStr = line.Split('=')[0];
                        string actionStr = line.Split('=')[1];

                        if (!string.IsNullOrEmpty(buttonStr) && !string.IsNullOrWhiteSpace(actionStr))
                        {
                            if (buttonConfigDict.ContainsKey(buttonStr))
                            {
                                buttonConfigDict[buttonStr] = actionStr;
                            }
                            else
                            {
                                buttonConfigDict.Add(buttonStr, actionStr);
                            }
                        }
                    }

                    if (line == "[PLAYSTATION(R)4 CONTROLLER]")
                    {
                        ps4found = true;
                    }
                }

                if (ps4found)
                {
                    this.setButtonText();
                }
            }
        }

        public void setButtonText()
        {
            foreach (KeyValuePair<string, string> entry in buttonConfigDict)
            {
                string buttonStr = entry.Key;
                string actionStr = entry.Value;

                if (buttonControlsDict.ContainsKey(buttonStr))
                {
                    Button button = buttonControlsDict[buttonStr];
                    button.ToolTip = buttonActionDict[actionStr];
                }
            }
        }

        private void btn_X_Click(object sender, RoutedEventArgs e)
        {
            popupButtonSelected = (Button)sender;
            if (buttonConfigDict.Count > 0)
            {
                PopupActionSelect.IsOpen = true;
            }
        }

        private void btn_square_Click(object sender, RoutedEventArgs e)
        {
            popupButtonSelected = (Button)sender;
            if(buttonConfigDict.Count > 0)
            {
                PopupActionSelect.IsOpen = true;
            }
        }

        private void ActionListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListView listView = (ListView)sender;

            popupButtonSelected.ToolTip = listView.SelectedItem;
            var buttonStr = buttonControlsDict.FirstOrDefault(x => x.Value == popupButtonSelected).Key;
            var actionStr = buttonActionDict.FirstOrDefault(x => x.Value == (string)listView.SelectedItem).Key;

            if(buttonConfigDict.ContainsKey(buttonStr))
            {
                buttonConfigDict[buttonStr] = actionStr;
            }
            else
            {
                buttonConfigDict.Add(buttonStr, actionStr);
            }

            PopupActionSelect.IsOpen = false;
        }

        private void btn_L3_Click(object sender, RoutedEventArgs e)
        {
            popupButtonSelected = (Button)sender;
            if (buttonConfigDict.Count > 0)
            {
                PopupActionSelect.IsOpen = true;
            }
        }

        private void btn_R3_Click(object sender, RoutedEventArgs e)
        {
            popupButtonSelected = (Button)sender;
            if (buttonConfigDict.Count > 0)
            {
                PopupActionSelect.IsOpen = true;
            }
        }

        private void btn_triangle_Click(object sender, RoutedEventArgs e)
        {
            popupButtonSelected = (Button)sender;
            if (buttonConfigDict.Count > 0)
            {
                PopupActionSelect.IsOpen = true;
            }
        }

        private void btn_circle_Click(object sender, RoutedEventArgs e)
        {
            popupButtonSelected = (Button)sender;
            if (buttonConfigDict.Count > 0)
            {
                PopupActionSelect.IsOpen = true;
            }
        }

        private void btn_R2_Click(object sender, RoutedEventArgs e)
        {
            popupButtonSelected = (Button)sender;
            if (buttonConfigDict.Count > 0)
            {
                PopupActionSelect.IsOpen = true;
            }
        }

        private void btn_L2_Click(object sender, RoutedEventArgs e)
        {
            popupButtonSelected = (Button)sender;
            if (buttonConfigDict.Count > 0)
            {
                PopupActionSelect.IsOpen = true;
            }
        }

        private void btn_L1_Click(object sender, RoutedEventArgs e)
        {
            popupButtonSelected = (Button)sender;
            if (buttonConfigDict.Count > 0)
            {
                PopupActionSelect.IsOpen = true;
            }
        }

        private void btn_R1_Click(object sender, RoutedEventArgs e)
        {
            popupButtonSelected = (Button)sender;
            if (buttonConfigDict.Count > 0)
            {
                PopupActionSelect.IsOpen = true;
            }
        }

        private void btn_SaveConfig_Click(object sender, RoutedEventArgs e)
        {
            bool deleteUntilBlankSpace = false;
            if (!string.IsNullOrEmpty(filename) && buttonConfigDict.Count > 0)
            {
                // Write the new file over the old file.
                using (StreamWriter writer = new StreamWriter(filename))
                {
                    foreach (string currentLine in oldFileLines)
                    {
                        if (currentLine == "[PLAYSTATION(R)4 CONTROLLER]")
                        {
                            writer.WriteLine(currentLine);
                            foreach (KeyValuePair<string, string> buttonConfig in buttonConfigDict)
                            {
                                writer.WriteLine(string.Join("=", buttonConfig.Key, buttonConfig.Value));
                            }
                            deleteUntilBlankSpace = true;
                        }
                        else
                        {
                            if (deleteUntilBlankSpace == true)
                            {
                                if(currentLine == "")
                                {
                                    deleteUntilBlankSpace = false;
                                    writer.WriteLine();
                                }
                                continue;
                            }
                            else
                            {
                                writer.WriteLine(currentLine);
                            }
                        }
                    }
                }
            }
        }
    }
}
