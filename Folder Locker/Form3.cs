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
    public partial class Form3 : Form
    {
        List<string> items;
        String[] data = new String[4];
        string password_result = null;
        string zipFolderPath = null;

        public Form3()
        {
            
            items = new List<string>();
            InitializeComponent();
            button1.Enabled = false;
        }
        IFirebaseConfig ifc = new FirebaseConfig()
        {
            AuthSecret = "ekqr50oR9QBSOgkiebtXlp4Z98Ca4WLOlejqKcM0",
            BasePath = "https://folder-locker-7f20a-default-rtdb.firebaseio.com/"
        };
        IFirebaseClient client;

        private void Form3_Load(object sender, EventArgs e)
        {
            Thread bluetoothServerThread = new Thread(new ThreadStart(ServerConnectThread));
            bluetoothServerThread.Start();
        }
        private void button1_MouseHover(object sender, EventArgs e)
        {
           // updateUI("Press OK to decrypt folder.");
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
            if(devices.Length != 0)
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
        BluetoothDeviceInfo deviceInfo;

        public void listBox1_DoubleClick(object sender, EventArgs e)
        {
            deviceInfo = devices.ElementAt(listBox1.SelectedIndex);
            updateUI(deviceInfo.DeviceName + "  was selected.");

            data[0] = deviceInfo.DeviceAddress.ToString();
            // data[1] = zipFolderPath;
            // data[2] = password_result;
            //  data[3] = deviceInfo.DeviceName;
            client = new FireSharp.FirebaseClient(ifc);
            if (client == null)
            {
                MessageBox.Show("there was some internet error");
            }

            var res = client.Get(@"fileData/" + data[0]);
            // var set = client.Set(@"fileData/" + data[key, 0], fd);

            fileData fd = res.ResultAs<fileData>();

            if(fd == null)
            {
                MessageBox.Show("Selected Device is not registered!");
            }
            else
            {
                data[1] = fd.FolderPath;
                data[2] = fd.Password;
                data[3] = fd.DeviceName;
               // button1.Enabled = true;
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

                }

                if(password_result == data[2])
                {
                    updateUI2("Device Address : "+data[0]);
                    updateUI2("Encrypted Folder : " + data[1].Remove(data[1].Length - 4, 4));
                    updateUI2("Password : " +data[2]);
                    updateUI2("Device Name : "+ data[3]);
                    button1.Enabled = true;
                    updateUI("Press OK to decrypt folder.");
                }
                else
                {
                    MessageBox.Show("Please Enter Correct password!");
                    return;
                }

            }   

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                // ch = new DirectoryInfo(data[key, 1]);
                // ch.Attributes = FileAttributes.Hidden;
                // File.SetAttributes(ofd.SelectedPath, FileAttributes.Hidden);
                // File.SetAttributes(ofd.SelectedPath, FileAttributes.System);



                // string temp = @"C:\Users\Nikunj\Desktop\test1.aes";
                // FileEncrypt(data[key, 1], data[key, 2]);
                FileDecrypt(data[1].Remove(data[1].Length - 4, 4) + ".aes",data[1], data[2]);
                File.Delete(data[1].Remove(data[1].Length - 4, 4) + ".aes");
              
                var set = client.Delete(@"fileData/" + data[0]);

                //  ZipFile.CreateFromDirectory(FolderPathToZip, ZipFileName);
                ZipFile.ExtractToDirectory(data[1], data[1].Remove(data[1].Length - 4, 4));

               // MessageBox.Show(data[1]);
                File.Delete(data[1]);

                //  MessageBox.Show("ZIP file extracted successfully!");

                MessageBox.Show("Folder decrypted successfully!");

                // data store in database
                /*
                  fileData fd = new fileData()
                  {
                      DeviceAddress = data[key, 0],
                      FolderPath = data[key, 1],
                      Password = data[key, 2],
                      DeviceName = data[key, 3]
                  };

                  var set = client.Set(@"fileData/" + data[key, 0], fd);
                  */
            }

            catch (Exception ex)
              {
                  MessageBox.Show("Error occur due to " + ex);
              }

              this.Close();

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
        private void FileDecrypt(string inputFile, string outputFile, string password)
        {
            byte[] passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);
            byte[] salt = new byte[32];

            FileStream fsCrypt = new FileStream(inputFile, FileMode.Open);
            fsCrypt.Read(salt, 0, salt.Length);

            RijndaelManaged AES = new RijndaelManaged();
            AES.KeySize = 256;
            AES.BlockSize = 128;
            var key = new Rfc2898DeriveBytes(passwordBytes, salt, 50000);
            AES.Key = key.GetBytes(AES.KeySize / 8);
            AES.IV = key.GetBytes(AES.BlockSize / 8);
            AES.Padding = PaddingMode.PKCS7;
            AES.Mode = CipherMode.CFB;

            CryptoStream cs = new CryptoStream(fsCrypt, AES.CreateDecryptor(), CryptoStreamMode.Read);

            FileStream fsOut = new FileStream(outputFile, FileMode.Create);

            int read;
            byte[] buffer = new byte[1048576];

            try
            {
                while ((read = cs.Read(buffer, 0, buffer.Length)) > 0)
                {
                    Application.DoEvents();
                    fsOut.Write(buffer, 0, read);
                }
            }
            catch (CryptographicException ex_CryptographicException)
            {
                Console.WriteLine("CryptographicException error: " + ex_CryptographicException.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            try
            {
                cs.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error by closing CryptoStream: " + ex.Message);
            }
            finally
            {
                fsOut.Close();
                fsCrypt.Close();
            }
        }
    }
 }

