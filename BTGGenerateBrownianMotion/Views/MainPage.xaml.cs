using BTGGenerateBrownianMotion.ViewModels;

namespace BTGGenerateBrownianMotion
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();
            BindingContext = new MainViewModel(RefreshChart);
        }
        private void RefreshChart()
        {
            ChartView.Invalidate(); 
        }

    }
}
