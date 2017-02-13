using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace IoTApp
{
	public partial class DocumentDbDeviceDetailPage : ContentPage
	{

        OperationInDocumentDb oidb;
        bool t = true;
		public DocumentDbDeviceDetailPage ()
		{
            InitializeComponent();

            oidb = OperationInDocumentDb.DefaultManager;
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            await RefreshItems(true);
        }

        private async  Task RefreshItems(bool v)
        {
            using (var scope = new ActivityIndicatorScope(syncIndicator, v))
            {
                devicedataList.ItemsSource = await oidb.GetDeviceDetailsAsync();
            }
        }

        public async  void OnAdd(object sender, EventArgs e)
        {
            using (var scope = new ActivityIndicatorScope(syncIndicator,t))
            {
                devicedataList.ItemsSource = await oidb.GetDeviceDetailsAsync();
            }
               
        }

        private class ActivityIndicatorScope : IDisposable
        {
            private bool showIndicator;
            private ActivityIndicator indicator;
            private Task indicatorDelay;

            public ActivityIndicatorScope(ActivityIndicator indicator, bool showIndicator)
            {
                this.indicator = indicator;
                this.showIndicator = showIndicator;

                if (showIndicator)
                {
                    indicatorDelay = Task.Delay(2000);
                    SetIndicatorActivity(true);
                }
                else
                {
                    indicatorDelay = Task.FromResult(0);
                }
            }

            private void SetIndicatorActivity(bool isActive)
            {
                this.indicator.IsVisible = isActive;
                this.indicator.IsRunning = isActive;
            }

            public void Dispose()
            {
                if (showIndicator)
                {
                    indicatorDelay.ContinueWith(t => SetIndicatorActivity(false), TaskScheduler.FromCurrentSynchronizationContext());
                }
            }
        }

    }

    
}
