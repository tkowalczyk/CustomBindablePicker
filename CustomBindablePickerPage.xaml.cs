namespace CustomBindablePicker
{
	using Xamarin.Forms;

	public partial class CustomBindablePickerPage : ContentPage
	{
		public CustomBindablePickerPage()
		{
			InitializeComponent();

			BindingContext = new CustomViewModel();
		}
	}
}