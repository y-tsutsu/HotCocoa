using System;
using System.Linq;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using Android.App;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Media;

using WebSocket4Net;

namespace HotCocoa
{
    [DataContract]
    public class JsonItem
    {
        [DataMember(Name = "count")]
        public int Count { get; set; }
        [DataMember(Name = "mp3")]
        public string MP3 { get; set; }
    }

    [Activity(Label = "HotCocoa", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private WebSocket ws;

        private MediaPlayer[] players;

        private const string HOST_NAME = "the-des-alizes.herokuapp.com";

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            RequestWindowFeature(WindowFeatures.NoTitle);
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            var button = FindViewById<Button>(Resource.Id.MyButton);
            button.Click += Button_Click;
            button.Enabled = false;

            var text = FindViewById<TextView>(Resource.Id.textView);
            text.Text = string.Empty;

            this.players = Enumerable.Range(0, 10).Select(_ => MediaPlayer.Create(this, Resource.Raw.nyanpasu)).ToArray();

            this.ws = new WebSocket(string.Format("wss://{0}/ws", MainActivity.HOST_NAME));
            this.ws.Opened += Ws_Opened;
            this.ws.MessageReceived += Ws_MessageReceived;

            this.ws.Open();
        }

        protected override void OnDestroy()
        {
            this.ws.Close();
            base.OnDestroy();
        }

        private void Button_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.ws == null) { return; }
                if (this.ws.State != WebSocketState.Open) { return; }

                var player = this.players.FirstOrDefault((x) => !x.IsPlaying);
                if (player != null)
                {
                    player.Start();
                }

                this.ws.Send("にゃんぱすー");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("***** send error!! *****");
                System.Diagnostics.Debug.WriteLine("***** {0} *****", ex.Message);
            }
        }

        private void Ws_Opened(object sender, EventArgs e)
        {
            var button = FindViewById<Button>(Resource.Id.MyButton);
            this.RunOnUiThread(() => button.Enabled = true);
        }

        private void Ws_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            var text = FindViewById<TextView>(Resource.Id.textView);
            using (var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(e.Message)))
            {
                var serializer = new DataContractJsonSerializer(typeof(JsonItem));
                var item = (JsonItem)serializer.ReadObject(stream);
                this.RunOnUiThread(() => text.Text = item.Count.ToString());
            }
        }
    }
}

