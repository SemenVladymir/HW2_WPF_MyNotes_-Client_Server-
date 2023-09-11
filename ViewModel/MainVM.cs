using HW2_WPF_MyNotes__Client_Server_.Models;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace HW2_WPF_MyNotes__Client_Server_.ViewModel
{
    public class MainVM : INotifyPropertyChanged
    {
        const int PORT = 4444;
        const string IP = "127.0.0.1";
        static Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        public ObservableCollection<MyNote> MyNotes { get; set; }
        private string shortText;
        private string description;
        private MyCommand addCommand;

        public MyCommand AddCommand
        {
            get
            {
                return addCommand ??
                  (addCommand = new MyCommand(obj =>
                  {
                      if (!string.IsNullOrEmpty(ShortText))
                      {
                          MyNote note = new MyNote { description = this.Description, shortDescription = ShortText, noteDateTime = DateTime.Now };
                          SaveNote(note);
                          MyNotes.Insert(0, note);
                      }
                      else
                          MessageBox.Show("You need input some text");
                  }));
            }
        }

        public string ShortText
        {
            get { return shortText; }
            set { shortText = value; OnPropertyChanged("ShortText"); }
        }

        public string Description
        {
            get => description;
            set { description = value; OnPropertyChanged("Description"); }
        }


        public MainVM()
        {
            ReadNotes();
        }

        public void ReadNotes()
        {
            
            try
            {
                socket.Connect(IP, PORT);
                byte[] read = Encoding.Unicode.GetBytes("#READ#");
                socket.Send(read);


                //Receive all byte from Socket
                byte[] firstData = new byte[256];
                int bytes = socket.Receive(firstData);
                int equal = bytes + socket.Available;
                byte[] buffer = new byte[equal];
                if (socket.Available > 0)
                {
                    byte[] secondData = new byte[socket.Available];
                    socket.Receive(secondData);
                    buffer = firstData.Concat(secondData).ToArray();
                }
                else
                    buffer = firstData;

                //Conver received server data from byte[] to class MyNote and get command
                string strAns = Encoding.Unicode.GetString(buffer, 0, equal);
                MyNotes = JsonSerializer.Deserialize<ObservableCollection<MyNote>>(strAns);
                MyNotes = new ObservableCollection<MyNote>(MyNotes.OrderByDescending(i => i.id));

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            //socket.Disconnect(true);
        }

        public void SaveNote(MyNote note)
        {
            if (socket.Connected)
            {
                string json = JsonSerializer.Serialize(note);
                byte[] write = Encoding.Unicode.GetBytes("#WRITE#" + json);
                socket.Send(write);
            }
            else
            {
                try
                {
                    socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    socket.Connect(IP, PORT);
                    string json = JsonSerializer.Serialize(note);
                    byte[] write = Encoding.Unicode.GetBytes("#WRITE#" + json);
                    socket.Send(write);
                }
                catch (Exception ex) { }
            }
            //socket.Disconnect(true);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }


    }
}
