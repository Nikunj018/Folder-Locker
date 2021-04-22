using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using InTheHand;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Ports;
using InTheHand.Net.Sockets;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;

using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;

using System.Data.Odbc;

namespace Folder_Locker
{
    public partial class Form2 : Form
    {
        DirectoryInfo ch;
        List<string> items;

        // string[,] data = new string[100, 4];
        String[] data = new String[4];
        string password_result = null;
        string zipFolderPath = null;
        string FolderPathToZip = null;

        public Form2()
        {
            items = new List<string>();
            InitializeComponent();
            button1.Enabled = false;
        }
        IFirebaseConfig ifc = new FirebaseConfig()
        {
            AuthSecret = "Authkey",
            BasePath = "base path"
        };
        IFirebaseClient client;

        private void Form2_Load(object sender, EventArgs e)
        {

            Thread bluetoothServerThread = new Thread(new ThreadStart(ServerConnectThread));
            if(bluetoothServerThread ==null)
            {

            }
            
            bluetoothServerThread.Start();
           
        }

        Guid mUUID = new Guid("00001101-0000-1000-8000-00805F9834FB");

        private void ServerConnectThread()
        {
            BluetoothListener blueListner = new BluetoothListener(mUUID);
            
            blueListner.Start();
            startScan();

        }
        private void startScan()
        {
            listBox1.DataSource = null;
            //listBox1.Items.Clear();
            items.Clear();
            Thread bluetoothScanThread = new Thread(new ThreadStart(scan));
            bluetoothScanThread.Start();
        }
        BluetoothDeviceInfo[] devices;
        private void scan()
        {

            updateUI("Starting Scan...");
            BluetoothClient client = new BluetoothClient();
            devices = client.DiscoverDevicesInRange();
            updateUI("Scan Complete.");
            updateUI(devices.Length.ToString() + " device discovered.");
            if (devices.Length != 0)
            {
                updateUI("To select device , Double click on it.");
            }
            
            foreach (BluetoothDeviceInfo d in devices)
            {
                items.Add(d.DeviceName);
            }

            updateDeviceList();

        }

        private void updateUI(string message)
        {
            Func<int> del = delegate ()
            {
                tbOutput.AppendText(message + System.Environment.NewLine);
                return 0;
            };
            Invoke(del);
        }

        private void updateUI2(string message)
        {
            Func<int> del = delegate ()
            {
                tb2Output.AppendText(message + System.Environment.NewLine);
                return 0;
            };
            Invoke(del);
        }
        private void updateDeviceList()
        {
            Func<int> del = delegate ()
            {
                listBox1.DataSource = items;
                return 0;
            };
            Invoke(del);
        }
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }


        BluetoothDeviceInfo deviceInfo;


        Form_password f_password = new Form_password();

       

        private void button1_MouseHover(object sender, EventArgs e)
        {
            //updateUI("Press OK to Encrypt Selected folder.");
        }

        public void listBox1_DoubleClick(object sender, EventArgs e)
        {
            deviceInfo = devices.ElementAt(listBox1.SelectedIndex);
            updateUI(deviceInfo.DeviceName + "  was selected.");

            client = new FireSharp.FirebaseClient(ifc);
            if (client == null)
            {
                MessageBox.Show("there was some internet error");
            }
            var res = client.Get(@"fileData/" + deviceInfo.DeviceAddress.ToString());

            
            fileData fd = res.ResultAs<fileData>();

            if (fd != null)
            {

                MessageBox.Show("You have already resisterd one folder for this device!\n\nplease " +
                    "decrypt first then use this device for encryption of another folder.");
                return;

            }

            //OpenFileDialog ofd = new OpenFileDialog();
            FolderBrowserDialog ofd = new FolderBrowserDialog();
            ofd.Description = "Select your folder";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                updateUI(ofd.SelectedPath + "  Folder selected.");

                FolderPathToZip = ofd.SelectedPath.Trim();
                //to store the value of folderapth   

            }
            //String path = ofd.SelectedPath;
            /*String path = @"C:\Users\Nikunj\Desktop\test1";
            Thread thread = new Thread(t =>
            {
                using (Ionic.Zip.ZipFile zip = new Ionic.Zip.ZipFile())
                {
                    zip.AddDirectory(path);
                    System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(path);
                    // zip.SaveProgress += Zip_SaveProgress;
                    zip.Save(string.Format("{0}{1}.zip", di.Parent.FullName, di.Name));

                }
            })
            { IsBackground = true };
            thread.Start();
            */


            /* f_password.ShowDialog();

             string deviceName = deviceInfo.DeviceName;
             string folderName =ofd.SelectedPath ;


             Form_password f_p = new Form_password();
             string Final_password = null;
             Final_password= f_p.returnValue();
             updateUI(Final_password + " Set Sucessfully");
             textBox1.Text = folderName;
                 MessageBox.Show(Final_password);
             */
            using (Form_password f_password = new Form_password())
            {
                f_password.ShowDialog();

               password_result = f_password.returnValue();
                if (password_result.Trim() == string.Empty)
                {
                    MessageBox.Show("This Field cannot left empty!");
                    return; // return because we don't want to run normal code of buton click
                }
                // MessageBox.Show(password_result);

                updateUI(password_result + " Set Sucessfully");    
            }
            
                for (int j = 0; j < 4; j++)
                {
                    data[j] = null;
                }
           
            data[0] = deviceInfo.DeviceAddress.ToString() ;
            // data[1, key] = ofd.SelectedPath;
            //data[1] = ofd.SelectedPath;
            data[1] = FolderPathToZip+".zip";
            data[2] = password_result;
            data[3] = deviceInfo.DeviceName;

            updateUI2("Device Address : " + data[0]);
            updateUI2("Folder path : "+data[1].Remove(data[1].Length - 4, 4));
            updateUI2("Password : "+data[2]);
            updateUI2("Device name : " + data[3]);
           
            button1.Enabled = true;
            updateUI("Press OK to Encrypt Selected folder.");
        }
        
        
        private void button1_Click(object sender, EventArgs e)
        {
            if(data[0] == null || data[1] == null || data[2] == null)
            {
                //MessageBox.Show("Some information is missing..\n please provide all information...");
                string message = "Some information is missing !\nPlease provide all information";
                string title = "Folder Locker";
                MessageBox.Show(message, title);

                data[0] = null;
                data[1] = null;
                data[2] = null;
                data[3] = null;
                this.Close();
            }

            if (data[0] != null && data[1] != null && data[2] != null)
            {

                
                try
                {
                    ch = new DirectoryInfo(data[1]);
                    // ch.Attributes = FileAttributes.Hidden;
                    // File.SetAttributes(ofd.SelectedPath, FileAttributes.Hidden);
                    // File.SetAttributes(ofd.SelectedPath, FileAttributes.System);
                   
                        client = new FireSharp.FirebaseClient(ifc);
                        if(client == null)
                        {
                            MessageBox.Show("there was some internet error");
                        }

                    // string temp = @"C:\Users\Nikunj\Desktop\test1.aes";
                    zipFolder();
                    FileEncrypt(data[1], data[2]);
                    //  FileDecrypt(temp, @"C:\Users\Nikunj\Desktop\test1.zip", data[2]);
                   

                    // data store in database
                    
                        
                        fileData fd1 = new fileData()
                        {
                            DeviceAddress = data[0],
                            FolderPath = data[1],
                            Password = data[2],
                            DeviceName = data[3]
                        };

                        var set = client.Set(@"fileData/" + data[0], fd1);
                   
                    MessageBox.Show("Folder encrypted successfully!");
                   

                }
                catch(Exception ex) {
                    MessageBox.Show("Error occur due to " + ex);
                }

                this.Close();
               
            }
         
        }
        public void zipFolder()
        {
            
            //To create unique file name with date and time with nanoseconds.  
            string ZipFileName = FolderPathToZip + ".zip";
            try
            {

                //TO create a zip file.  
                ZipFile.CreateFromDirectory(FolderPathToZip, ZipFileName);
            }
            catch (Exception)
            {
                //If system throw any exception message box will display "SOME ERROR"  
                MessageBox.Show("Some Error");
            }
            //Display successfully created message to the user.  
            //MessageBox.Show("Zip Filename : " + ZipFileName + " Created Successfully");
            //Directory.Delete(ofd.SelectedPath.Trim());
            if (Directory.Exists(FolderPathToZip))
            {
                //Delete all files from the Directory
                foreach (string file in Directory.GetFiles(FolderPathToZip))
                {
                    File.Delete(file);
                }

                //Delete a Directory
                Directory.Delete(FolderPathToZip);
                zipFolderPath = ZipFileName;
            }
        }
        private static byte[] GenerateRandomSalt()
        {
            byte[] data = new byte[32];

            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                for (int i = 0; i < 10; i++)
                {
                    // Fille the buffer with the generated data
                    rng.GetBytes(data);
                }
            }

            return data;
        }

       
        /// Encrypts a file from its path and a plain password.
        
        private void FileEncrypt(string inputFile, string password)
        {
            //http://stackoverflow.com/questions/27645527/aes-encryption-on-large-files

            //generate random salt
            byte[] salt = GenerateRandomSalt();

            //create output file name
            FileStream fsCrypt = new FileStream(inputFile.Remove(inputFile.Length-4, 4) + ".aes", FileMode.Create);

            //convert password string to byte arrray
            byte[] passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);

            //Set Rijndael symmetric encryption algorithm
            RijndaelManaged AES = new RijndaelManaged();
            AES.KeySize = 256;
            AES.BlockSize = 128;
            AES.Padding = PaddingMode.PKCS7;

            //http://stackoverflow.com/questions/2659214/why-do-i-need-to-use-the-rfc2898derivebytes-class-in-net-instead-of-directly
            //"What it does is repeatedly hash the user password along with the salt." High iteration counts.
            var key = new Rfc2898DeriveBytes(passwordBytes, salt, 50000);
            AES.Key = key.GetBytes(AES.KeySize / 8);
            AES.IV = key.GetBytes(AES.BlockSize / 8);

            //Cipher modes: http://security.stackexchange.com/questions/52665/which-is-the-best-cipher-mode-and-padding-mode-for-aes-encryption
            AES.Mode = CipherMode.CFB;

            // write salt to the begining of the output file, so in this case can be random every time
            fsCrypt.Write(salt, 0, salt.Length);

            CryptoStream cs = new CryptoStream(fsCrypt, AES.CreateEncryptor(), CryptoStreamMode.Write);

            FileStream fsIn = new FileStream(inputFile, FileMode.Open);

            //create a buffer (1mb) so only this amount will allocate in the memory and not the whole file
            byte[] buffer = new byte[1048576];
            int read;

            try
            {
                while ((read = fsIn.Read(buffer, 0, buffer.Length)) > 0)
                {
                    Application.DoEvents(); // -> for responsive GUI, using Task will be better!
                    cs.Write(buffer, 0, read);
                }

                // Close up
                fsIn.Close();

                File.Delete(zipFolderPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                cs.Close();
                fsCrypt.Close();
            }
            
        }



        public string returnPass()
        {
            string return_pass = data[2];
            return return_pass;
        }
    }
}
