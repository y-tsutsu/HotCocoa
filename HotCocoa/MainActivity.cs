using System;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Media;

using WebSocket4Net;

namespace HotCocoa
{
    [Activity(Label = "HotCocoa", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private WebSocket ws;

        private MediaPlayer[] players = new MediaPlayer[10];

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

            var text = FindViewById<TextView>(Resource.Id.textView);
            text.Enabled = false;

            for (int i = 0; i < this.players.Length; i++)
            {
                this.players[i] = MediaPlayer.Create(this, Resource.Raw.nyanpasu);
            }

            this.ws = new WebSocket("ws://the-des-alizes.herokuapp.com/ws");
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
            var text = FindViewById<TextView>(Resource.Id.textView);
            this.RunOnUiThread(() => text.Enabled = true);
        }

        private void Ws_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            var text = FindViewById<TextView>(Resource.Id.textView);
            this.RunOnUiThread(() => text.Text = e.Message);
        }
    }
}

