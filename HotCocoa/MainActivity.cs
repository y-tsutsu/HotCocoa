using System;
using System.Threading;
using System.Net.WebSockets;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Media;

namespace HotCocoa
{
    [Activity(Label = "HotCocoa", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private ClientWebSocket ws;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            var button = FindViewById<Button>(Resource.Id.MyButton);
            button.Click += Button_Click;

            this.Connect();
        }

        private async void Button_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.ws == null) { return; }
                if (this.ws.State != WebSocketState.Open) { return; }

                MediaPlayer.Create(this, Resource.Raw.nyanpasu).Start();

                var buff = new ArraySegment<byte>(System.Text.Encoding.UTF8.GetBytes("にゃんぱすー"));
                await this.ws.SendAsync(buff, WebSocketMessageType.Text, true, CancellationToken.None);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("***** send error!! *****");
                System.Diagnostics.Debug.WriteLine("***** {0} *****", ex.Message);
            }
        }

        private async void Connect()
        {
            try
            {
                if (this.ws == null)
                {
                    this.ws = new ClientWebSocket();
                }

                if (this.ws.State != WebSocketState.Open)
                {
                    await this.ws.ConnectAsync(new Uri("ws://the-des-alizes.herokuapp.com/ws"), CancellationToken.None);

                    while (this.ws.State == WebSocketState.Open)
                    {
                        var buff = new ArraySegment<byte>(new byte[256]);
                        var ret = await this.ws.ReceiveAsync(buff, CancellationToken.None);
                        var text = FindViewById<TextView>(Resource.Id.textView);
                        text.Text = System.Text.Encoding.UTF8.GetString(buff.Take(ret.Count).ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("***** connect error!! *****");
                System.Diagnostics.Debug.WriteLine("***** {0} *****", ex.Message);
            }
        }
    }
}

