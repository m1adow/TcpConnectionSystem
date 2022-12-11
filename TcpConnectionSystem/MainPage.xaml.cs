using TcpConnectionSystem.ViewModels;

namespace TcpConnectionSystem;

public partial class MainPage : ContentPage
{
    public MainPage()
	{
		this.BindingContext = MauiProgram.GetService<MainViewModel>();

        InitializeComponent();
	}
}